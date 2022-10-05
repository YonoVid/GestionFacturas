using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
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
        Task<Invoice> createInvoice(GestionFacturasContext _context,
                                    ClaimsPrincipal userClaims,
                                    InvoiceEditable invoiceData);

        Task<Invoice> deleteInvoice(GestionFacturasContext _context,
                                    ClaimsPrincipal userClaims,
                                    int invoiceId);

        Task<Invoice> editInvoice(GestionFacturasContext _context,
                                  ClaimsPrincipal userClaims,
                                  InvoiceEditable invoiceData,
                                  int invoiceId);
    }
}
