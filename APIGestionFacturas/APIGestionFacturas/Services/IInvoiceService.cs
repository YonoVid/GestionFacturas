using GestionFacturasModelo.Model.DataModel;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public interface IInvoiceService
    {
        IQueryable<Invoice> getAvailableInvoices(IQueryable<Invoice> invoices,
                                                 ClaimsPrincipal userClaims);
        Task<Invoice> getAvailableInvoice(DbSet<Invoice> invoices,
                                          ClaimsPrincipal userClaims,
                                          int id);
        IQueryable<Invoice> getAvailableEnterpriseInvoices(IQueryable<Invoice> invoices,
                                                           ClaimsPrincipal userClaims,
                                                           int enterpriseId);
    }
}
