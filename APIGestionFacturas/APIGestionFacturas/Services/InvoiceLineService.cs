using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public class InvoiceLineService: IInvoiceLineService
    {
        public IQueryable<InvoiceLine> getAvailableInvoiceLines(IQueryable<InvoiceLine> invoiceLines, ClaimsPrincipal userClaims)
        {
            if (!userClaims.IsInRole("Administrator"))
            {
                var identity = userClaims.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    int tokenId = int.Parse(identity.FindFirst("Id").Value);
                    return invoiceLines.Where((invoiceLine) => invoiceLine.Invoice.Enterprise.UserId == tokenId);

                }
                return null;
            }
            return invoiceLines;
        }
        public async Task<InvoiceLine> getAvailableInvoiceLine(DbSet<InvoiceLine> invoiceLines, ClaimsPrincipal userClaims, int id)
        {
            InvoiceLine result = await invoiceLines.FindAsync(id);

            if (!userClaims.IsInRole("Administrator") && result != null)
            {
                var identity = userClaims.Identity as ClaimsIdentity;

                int idToken = int.Parse(identity.FindFirst("Id").Value);

                if (result.Invoice.Enterprise.UserId != idToken)
                {
                    return null;
                }
            }

            return result;
        }
        public IQueryable<InvoiceLine> getAvailableInvoiceLines(IQueryable<InvoiceLine> invoiceLines,
                                                                ClaimsPrincipal userClaims,
                                                                int InvoiceId)
        {
            if (!userClaims.IsInRole("Administrator"))
            {
                var filteredInvoiceLines = invoiceLines.Where((invoiceLine) => invoiceLine.Invoice.Id == InvoiceId);
                var identity = userClaims.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    var id = identity.FindFirst("Id").Value;

                    if (filteredInvoiceLines.Any((invoiceLine) => invoiceLine.Invoice.Enterprise.UserId.ToString() == id))
                    { return filteredInvoiceLines; }

                }
                return null;
            }
            return invoiceLines.Where((invoiceLine) => invoiceLine.Invoice.Id == InvoiceId);
        }

        public async Task<InvoiceLine> createInvoiceLine(GestionFacturasContext _context,
                                                         ClaimsPrincipal userClaims,
                                                         InvoiceLineEditable invoiceLineData,
                                                         int invoiceId)
        {
            var invoiceLine = new InvoiceLine(invoiceLineData);

            if (invoiceLine == null) { throw new KeyNotFoundException("Id de empresa no encontrado"); }

            Invoice? invoice = await _context.Invoices.FindAsync(invoiceId);
            if(invoice == null)
            {
                throw new KeyNotFoundException("Id de factura no encontrado");
            }

            invoice.UpdatedBy = userClaims.Identity.Name;
            invoice.UpdatedDate = DateTime.Now;
            _context.Invoices.Update(invoice);


            invoiceLine.InvoiceId = invoiceId;
            invoiceLine.Invoice = invoice;
            _context.InvoiceLines.Add(invoiceLine);

            invoice.TotalAmount += (1 + invoice.TaxPercentage / 100) * (invoiceLine.ItemValue * invoiceLine.Quantity);
            _context.Invoices.Update(invoice);

            await _context.SaveChangesAsync();


            return invoiceLine;
        }


        public async Task<InvoiceLine> editInvoiceLine(GestionFacturasContext _context,
                                                       ClaimsPrincipal userClaims,
                                                       InvoiceLineEditable invoiceLineData,
                                                       int invoiceLineId)
        {
            InvoiceLine? invoiceLine = await getAvailableInvoiceLine(_context.InvoiceLines, userClaims, invoiceLineId);

            if (invoiceLine == null)
            {
                throw new InvalidOperationException("No hay suficientes datos para modificar la entidad");
            }

            if (invoiceLineData.Item != null) { invoiceLine.Item = invoiceLineData.Item; }
            if (invoiceLineData.ItemValue != null || invoiceLineData.Quantity != null)
            {
                Invoice? invoice = await _context.Invoices.FindAsync(invoiceLine.InvoiceId);

                var oldValue = invoiceLine.Quantity * invoiceLine.ItemValue;

                if (invoiceLineData.Quantity != null) { invoiceLine.Quantity = (int)invoiceLineData.Quantity; }
                if (invoiceLineData.ItemValue != null) { invoiceLine.ItemValue = (float)invoiceLineData.ItemValue; }

                if(invoice != null)
                {
                    invoice.TotalAmount -= (1 + invoice.TaxPercentage / 100) * oldValue;
                    invoice.TotalAmount += (1 + invoice.TaxPercentage / 100) * invoiceLine.Quantity * invoiceLine.ItemValue;

                    invoice.UpdatedBy = userClaims.Identity.Name;
                    invoice.UpdatedDate = DateTime.Now;
                    _context.Invoices.Update(invoice);
                }

            }

            _context.InvoiceLines.Update(invoiceLine);

            _context.Entry(invoiceLine).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return invoiceLine;
        }
        public async Task<InvoiceLine> deleteInvoiceLine(GestionFacturasContext _context,
                                                         ClaimsPrincipal userClaims,
                                                         int invoiceLineId)
        {
            var invoiceLine = await getAvailableInvoiceLine(_context.InvoiceLines, userClaims, invoiceLineId);
            if (invoiceLine == null)
            {
                throw new KeyNotFoundException("Linea de factura no encontrada");
            }

            Invoice? invoice = await _context.Invoices.FindAsync(invoiceLine.InvoiceId);

            if(invoice != null)
            {
                invoice.TotalAmount -= (1 + invoice.TaxPercentage / 100) * invoiceLine.Quantity * invoiceLine.ItemValue;

                invoice.UpdatedBy = userClaims.Identity.Name;
                invoice.UpdatedDate = DateTime.Now;
                _context.Invoices.Update(invoice);
            }

            _context.InvoiceLines.Remove(invoiceLine);
            await _context.SaveChangesAsync();

            return invoiceLine;
        }
    }
}
