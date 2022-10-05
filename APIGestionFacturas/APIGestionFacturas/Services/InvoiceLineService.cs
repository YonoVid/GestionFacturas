using GestionFacturasModelo.Model.DataModel;
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
    }
}
