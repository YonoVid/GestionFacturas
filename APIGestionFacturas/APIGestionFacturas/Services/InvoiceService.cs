using GestionFacturasModelo.Model.DataModel;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public class InvoiceService: IInvoiceService
    {
        public IQueryable<Invoice> getAvailableInvoices(IQueryable<Invoice> invoices, ClaimsPrincipal userClaims)
        {
            if (!userClaims.IsInRole("Administrator"))
            {
                var identity = userClaims.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    var id = identity.FindFirst("Id").Value;
                    return invoices.Where((invoice) => invoice.Enterprise.UserId.ToString() == id && !invoice.IsDeleted);

                }
                return null;
            }
            return invoices;
        }
        public async Task<Invoice> getAvailableInvoice(DbSet<Invoice> invoices, ClaimsPrincipal userClaims, int id)
        {
            Invoice result = await invoices.FindAsync(id);

            if (!userClaims.IsInRole("Administrator") && result != null)
            {
                var identity = userClaims.Identity as ClaimsIdentity;

                int idToken = int.Parse(identity.FindFirst("Id").Value);

                if (result.Enterprise.UserId != idToken)
                {
                    return null;
                }
            }

            return result;
        }

        public IQueryable<Invoice> getAvailableEnterpriseInvoices(IQueryable<Invoice> invoices,
                                                                ClaimsPrincipal userClaims,
                                                                int enterpriseId)
        {
            if (!userClaims.IsInRole("Administrator"))
            {
                var enterpriseInvoices = invoices.Where((invoice) => invoice.Enterprise.Id == enterpriseId &&
                                                                     !invoice.IsDeleted);
                var identity = userClaims.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    var id = identity.FindFirst("Id").Value;

                    if (enterpriseInvoices.Any((invoice) => invoice.Enterprise.UserId.ToString() == id))
                    { return enterpriseInvoices; }

                }
                return null;
            }
            return invoices.Where((invoice) => invoice.Enterprise.Id == enterpriseId);
        }
    }
}
