using GestionFacturasModelo.Model.DataModel;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public interface IInvoiceLineService
    {
        IQueryable<InvoiceLine> getAvailableInvoiceLines(IQueryable<InvoiceLine> invoiceLines,
                                                         ClaimsPrincipal userClaims);
        public IQueryable<InvoiceLine> getAvailableInvoiceLines(IQueryable<InvoiceLine> invoiceLines,
                                                                ClaimsPrincipal userClaims,
                                                                int InvoiceId);
        Task<InvoiceLine> getAvailableInvoiceLine(DbSet<InvoiceLine> invoiceLines,
                                                  ClaimsPrincipal userClaims,
                                                  int id);
    }
}
