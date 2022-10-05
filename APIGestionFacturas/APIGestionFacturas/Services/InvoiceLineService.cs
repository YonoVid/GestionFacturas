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
                    var id = identity.FindFirst("Id").Value;
                    return invoiceLines.Where((invoiceLine) => invoiceLine.Id.ToString() == id && !invoiceLine.IsDeleted);

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

                if (result.Id != idToken)
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
                var filteredInvoiceLines = invoiceLines.Where((invoiceLine) => invoiceLine.Invoice.Id == InvoiceId &&
                                                                            !invoiceLine.IsDeleted);
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
            invoiceLine.InvoiceId = invoiceId;
            invoiceLine.Invoice = invoice;

            invoiceLine.CreatedBy = userClaims.Identity.Name;

            _context.InvoiceLines.Add(invoiceLine);
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
            if (invoiceLineData.Quantity != null) { invoiceLine.Quantity = (int)invoiceLineData.Quantity; }
            if (invoiceLineData.ItemValue != null) { invoiceLine.ItemValue = (float)invoiceLineData.ItemValue; }

            invoiceLine.UpdatedBy = userClaims.Identity.Name;
            invoiceLine.UpdatedDate = DateTime.Now;

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

            invoiceLine.DeletedBy = userClaims.Identity.Name;
            invoiceLine.DeletedDate = DateTime.Now;
            invoiceLine.IsDeleted = true;

            _context.InvoiceLines.Update(invoiceLine);
            await _context.SaveChangesAsync();

            return invoiceLine;
        }
    }
}
