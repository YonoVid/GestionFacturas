using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
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

        Task<InvoiceLine> createInvoiceLine(GestionFacturasContext _context,
                                            ClaimsPrincipal userClaims,
                                            InvoiceLineEditable invoiceLineData,
                                            int invoiceId = -1);

        Task<InvoiceLine> deleteInvoiceLine(GestionFacturasContext _context,
                                            ClaimsPrincipal userClaims,
                                            int invoiceLineId);

        Task<InvoiceLine> editInvoiceLine(GestionFacturasContext _context,
                                          ClaimsPrincipal userClaims,
                                          InvoiceLineEditable invoiceLineData,
                                          int invoiceLineId);
    }
}
