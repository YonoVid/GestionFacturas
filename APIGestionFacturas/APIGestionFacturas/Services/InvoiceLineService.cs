using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public class InvoiceLineService: IInvoiceLineService
    {
        public IQueryable<InvoiceLine>? GetAvailableInvoiceLines(IQueryable<InvoiceLine> invoiceLines, ClaimsPrincipal userClaims)
        {
            // Check if user is 'Administrator'
            if (!userClaims.IsInRole("Administrator"))
            {
                // Check if user identity is not null
                var identity = userClaims.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    // Return all invoices with a enterprise managed by the user
                    var tokenId = identity?.FindFirst("Id")?.Value;
                    return invoiceLines.Where((invoiceLine) => 
                        invoiceLine.Invoice.Enterprise.UserId.ToString() == tokenId
                    );
                }
                return null;
            }
            // Return obtained invoice lines, may be null
            return invoiceLines;
        }

        public async Task<InvoiceLine?> GetAvailableInvoiceLine(DbSet<InvoiceLine> invoiceLines, ClaimsPrincipal userClaims, int id)
        {
            // Search for the indicated invoice line
            InvoiceLine? result = await invoiceLines.FindAsync(id);

            // Check if user is 'Administrator'
            if (!userClaims.IsInRole("Administrator") && result != null)
            {
                // Get enterprise user id and check if the user making the call is the same
                var identity = userClaims.Identity as ClaimsIdentity;
                if (result.Invoice.Enterprise.UserId.ToString() != identity?.FindFirst("Id")?.Value)
                {
                    // Return null if the user doesn't have permission
                    return null;
                }
            }
            // Return obtained invoice line, may be null
            return result;
        }

        public IQueryable<InvoiceLine> GetAvailableInvoiceLines(IQueryable<InvoiceLine> invoiceLines,
                                                                ClaimsPrincipal userClaims,
                                                                int InvoiceId)
        {
            // Check if user is 'Administrator'
            if (!userClaims.IsInRole("Administrator"))
            {
                // Get all the invoice lines from the invoice selected
                var filteredInvoiceLines = invoiceLines.Where((invoiceLine) => invoiceLine.Invoice.Id == InvoiceId);
                
                // Check if user identity is not null
                var identity = userClaims.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    // Get user id
                    var id = identity.FindFirst("Id").Value;

                    // Check if the asociated user of the enterprise is the user making the call
                    if (filteredInvoiceLines.Any((invoiceLine) => invoiceLine.Invoice.Enterprise.UserId.ToString() == id))
                    { return filteredInvoiceLines; }

                }
                // If a condition is not met return null
                return null;
            }
            // If user is 'Administrator' return all invoice lines from the invoice
            return invoiceLines.Where((invoiceLine) => invoiceLine.Invoice.Id == InvoiceId);
        }

        public async Task<InvoiceLine> CreateInvoiceLine(GestionFacturasContext _context,
                                                         ClaimsPrincipal userClaims,
                                                         InvoiceLineEditable invoiceLineData,
                                                         int invoiceId)
        {
            // Create new invoice line from data provided
            var invoiceLine = new InvoiceLine(invoiceLineData);

            // Search indicated invoice
            Invoice? invoice = await _context.Invoices.FindAsync(invoiceId);
            if(invoice == null)
            {
                // Throw error if no valid invoice was found
                throw new KeyNotFoundException("Id de factura no encontrado");
            }
            // Updated data of the invoice related to updation
            invoice.UpdatedBy = userClaims?.Identity?.Name;
            invoice.UpdatedDate = DateTime.Now;
            _context.Invoices.Update(invoice);

            // Add data of the invoice line related to invoice
            invoiceLine.InvoiceId = invoiceId;
            invoiceLine.Invoice = invoice;

            // Add invoice line
            _context.InvoiceLines.Add(invoiceLine);
            // Update invoice
            invoice.TotalAmount += invoiceLine.ItemValue * invoiceLine.Quantity;
            _context.Invoices.Update(invoice);
            // Save changes
            await _context.SaveChangesAsync();

            // Return created invoice line data
            return invoiceLine;
        }

        public async Task<InvoiceLine> EditInvoiceLine(GestionFacturasContext _context,
                                                       ClaimsPrincipal userClaims,
                                                       InvoiceLineEditable invoiceLineData,
                                                       int invoiceLineId)
        {
            // Search the requested invoice line
            InvoiceLine? invoiceLine = await GetAvailableInvoiceLine(_context.InvoiceLines, userClaims, invoiceLineId);

            if (invoiceLine == null)
            {
                // Throw error if no valid invoice was found
                throw new InvalidOperationException("No hay suficientes datos para modificar la entidad");
            }
            if (invoiceLineData.Item == null &&
               invoiceLineData.Quantity== null &&
               invoiceLineData.ItemValue == null)
            {

                // Throw error if not enough data is provided
                throw new InvalidOperationException("No hay suficientes datos para modificar la entidad");
            }

            // Modify item if is in data
            if (invoiceLineData.Item != null) { invoiceLine.Item = invoiceLineData.Item; }
            if (invoiceLineData.ItemValue != null || invoiceLineData.Quantity != null)
            {
                // Search the invoice asociated 
                Invoice? invoice = await _context.Invoices.FindAsync(invoiceLine.InvoiceId);

                // Save old subtotal
                var oldValue = invoiceLine.Quantity * invoiceLine.ItemValue;

                // Every value included to modify is updated
                if (invoiceLineData.Quantity != null) { invoiceLine.Quantity = (int)invoiceLineData.Quantity; }
                if (invoiceLineData.ItemValue != null) { invoiceLine.ItemValue = (float)invoiceLineData.ItemValue; }

                // Check if invoice is null
                if (invoice != null)
                {
                    // Update total amount from invoice
                    invoice.TotalAmount -= oldValue;
                    invoice.TotalAmount += invoiceLine.Quantity * invoiceLine.ItemValue;

                    // Updated data of the invoice related to updation
                    invoice.UpdatedBy = userClaims.Identity.Name;
                    invoice.UpdatedDate = DateTime.Now;
                    _context.Invoices.Update(invoice);
                }

            }
            // Invoice line is updated and changes are saved
            _context.InvoiceLines.Update(invoiceLine);
            _context.Entry(invoiceLine).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Return updated invoice line data
            return invoiceLine;
        }

        public async Task<InvoiceLine> DeleteInvoiceLine(GestionFacturasContext _context,
                                                         ClaimsPrincipal userClaims,
                                                         int invoiceLineId)
        {
            // Search the requested invoice line
            var invoiceLine = await GetAvailableInvoiceLine(_context.InvoiceLines, userClaims, invoiceLineId);

            if (invoiceLine == null)
            {
                // Throw error if no valid invoice was found
                throw new KeyNotFoundException("Linea de factura no encontrada");
            }
            // Search the invoice asociated 
            Invoice? invoice = await _context.Invoices.FindAsync(invoiceLine.InvoiceId);

            // Check if invoice is null
            if(invoice != null)
            {
                // Update total amount from invoice
                invoice.TotalAmount -= invoiceLine.Quantity * invoiceLine.ItemValue;

                // Updated data of the invoice related to updation
                invoice.UpdatedBy = userClaims.Identity.Name;
                invoice.UpdatedDate = DateTime.Now;
                _context.Invoices.Update(invoice);
            }
            // Invoice line is deleted and changes are saved
            _context.InvoiceLines.Remove(invoiceLine);
            await _context.SaveChangesAsync();

            // Return deleted invoice line data
            return invoiceLine;
        }
    }
}
