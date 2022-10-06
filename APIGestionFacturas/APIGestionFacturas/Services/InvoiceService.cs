using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public class InvoiceService : IInvoiceService
    {
        public IQueryable<Invoice>? getAvailableInvoices(IQueryable<Invoice> invoices, ClaimsPrincipal userClaims)
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
        public async Task<Invoice?> getAvailableInvoice(DbSet<Invoice> invoices, ClaimsPrincipal userClaims, int id)
        {
            Invoice? result = await invoices.FindAsync(id);

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

        public async Task<Invoice> createInvoice(GestionFacturasContext _context,
                                                 ClaimsPrincipal userClaims,
                                                 InvoiceEditable invoiceData)
        {
            var invoice = new Invoice(invoiceData);

            Enterprise? enterprise = await _context.Enterprises.FindAsync(invoiceData.EnterpriseId);
            if (enterprise == null) { throw new KeyNotFoundException("Id de empresa no encontrado"); }
            invoice.Enterprise = enterprise;

            invoice.CreatedBy = userClaims.Identity.Name;

            invoice.Id = _context.Invoices.Add(invoice).Entity.Id;

            await _context.SaveChangesAsync();

            return invoice;
        }

        public async Task<Invoice> editInvoice(GestionFacturasContext _context,
                                                 ClaimsPrincipal userClaims,
                                                 InvoiceEditable invoiceData,
                                                 int invoiceId)
        {
            Invoice? invoice = await getAvailableInvoice(_context.Invoices, userClaims, invoiceId);

            if (invoice == null)
            {
                throw new KeyNotFoundException("Factura no encontrada");
            }
            if(invoiceData.Name == null &&
               invoiceData.TaxPercentage == null &&
               invoiceData.EnterpriseId == null &&
               invoiceData.InvoiceLines == null)
            {
                throw new InvalidOperationException("No hay suficientes datos para modificar la entidad");
            }

            if (invoiceData.Name!= null) { invoice.Name= invoiceData.Name; }
            if (invoiceData.TaxPercentage != null) { invoice.TaxPercentage = (int)invoiceData.TaxPercentage; }
            if (invoiceData.EnterpriseId != null) { invoice.EnterpriseId = (int)invoiceData.EnterpriseId; }

            invoice.UpdatedBy = userClaims.Identity.Name;
            invoice.UpdatedDate = DateTime.Now;

            _context.Invoices.Update(invoice);

            _context.Entry(invoice).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return invoice;

        }

        public async Task<Invoice> deleteInvoice(GestionFacturasContext _context,
                                                    ClaimsPrincipal userClaims,
                                                    int invoiceId)
        {
            var invoice = await getAvailableInvoice(_context.Invoices, userClaims, invoiceId);
            if (invoice == null)
            {
                throw new KeyNotFoundException("Factura no encontrada");
            }

            foreach(InvoiceLine invoiceLine in _context.InvoiceLines.Where((InvoiceLine row) => row.InvoiceId == invoice.Id))
            {
                invoiceLine.DeletedBy = userClaims.Identity.Name;
                invoiceLine.DeletedDate = DateTime.Now;
                invoiceLine.IsDeleted = true;
                _context.InvoiceLines.Update(invoiceLine);
            }

            invoice.DeletedBy = userClaims.Identity.Name;
            invoice.DeletedDate = DateTime.Now;
            invoice.IsDeleted = true;

            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            return invoice;
        }
    }
}
