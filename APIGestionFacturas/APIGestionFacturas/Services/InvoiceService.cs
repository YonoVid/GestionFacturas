using APIGestionFacturas.DataAccess;
using APIGestionFacturas.Helpers;
using DinkToPdf;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public class InvoiceService : IInvoiceService
    {
        public IQueryable<Invoice>? GetAvailableInvoices(IQueryable<Invoice> invoices, ClaimsPrincipal userClaims)
        {
            // Check if user is 'Administrator'
            if (!userClaims.IsInRole("Administrator"))
            {
                // Check if user identity is not null
                var identity = userClaims.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    // Get user id
                    var id = identity.FindFirst("Id").Value;
                    // Return all invoices with a enterprise managed by the user
                    return invoices.Where((invoice) => 
                        invoice.Enterprise.UserId.ToString() == id && !invoice.IsDeleted
                    );

                }
                // If identity
                return null;
            }
            // Return obtained invoices, may be null
            return invoices;
        }
        public async Task<Invoice?> GetAvailableInvoice(DbSet<Invoice> invoices, ClaimsPrincipal userClaims, int id)
        {
            // Search for the indicated invoice
            Invoice? result = await invoices.FindAsync(id);

            // Check if user is 'Administrator'
            if (!userClaims.IsInRole("Administrator") && result != null)
            {
                // Get enterprise user id and check if the user making the call is the same
                var identity = userClaims.Identity as ClaimsIdentity;
                if (result.Enterprise.UserId.ToString() != identity?.FindFirst("Id")?.Value)
                {
                    // Return null if the user doesn't have permission
                    return null;
                }
            }
            // Return obtained invoice, may be null
            return result;
        }

        public IQueryable<Invoice> GetAvailableEnterpriseInvoices(IQueryable<Invoice> invoices,
                                                                ClaimsPrincipal userClaims,
                                                                int enterpriseId)
        {
            // Check if user is 'Administrator'
            if (!userClaims.IsInRole("Administrator"))
            {
                // Get all the invoices from the enterprise that are not deleted
                var enterpriseInvoices = invoices.Where((invoice) => invoice.Enterprise.Id == enterpriseId &&
                                                                     !invoice.IsDeleted);
                // Check if user identity is not null
                var identity = userClaims.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    // Get user id
                    var id = identity.FindFirst("Id").Value;
                    // Check if the asociated user of the enterprise is the user making the call
                    if (enterpriseInvoices.Any((invoice) => invoice.Enterprise.UserId.ToString() == id))
                    { return enterpriseInvoices; }
                }
                // If a condition is not met return null
                return null;
            }
            // If user is 'Administrator' return all invoices from the enterprise
            return invoices.Where((invoice) => invoice.Enterprise.Id == enterpriseId);
        }

        public async Task<Invoice> CreateInvoice(GestionFacturasContext _context,
                                                 ClaimsPrincipal userClaims,
                                                 InvoiceEditable invoiceData)
        {
            if (invoiceData.Name == null ||
                invoiceData.EnterpriseId == null ||
                invoiceData.TaxPercentage == null)
            {
                throw new InvalidOperationException("Faltan datos para generar la entidad");
            }
            // Create new invoice from the provided data
            var invoice = new Invoice(invoiceData);

            // Search for the indicated enterprise
            Enterprise? enterprise = await _context.Enterprises.FindAsync(invoiceData.EnterpriseId);

            // If enterprise is not found throw a error
            if (enterprise == null) { throw new KeyNotFoundException("Id de empresa no encontrado"); }
            invoice.Enterprise = enterprise;

            // Updated related data of the creation
            invoice.CreatedBy = userClaims.Identity.Name;

            // Add the invoice to the database and save the changes
            // Generated invoice data is updated with genereated id
            invoice.Id = _context.Invoices.Add(invoice).Entity.Id;
            await _context.SaveChangesAsync();

            // Return created invoice data
            return invoice;
        }

        public async Task<Invoice> EditInvoice(GestionFacturasContext _context,
                                                 ClaimsPrincipal userClaims,
                                                 InvoiceEditable invoiceData,
                                                 int invoiceId)
        {
            // Search the requested invoice
            Invoice? invoice = await GetAvailableInvoice(_context.Invoices, userClaims, invoiceId);

            if (invoice == null)
            {
                // Throw error if no valid invoice was found
                throw new KeyNotFoundException("Factura no encontrada");
            }
            if(invoiceData.Name == null &&
               invoiceData.TaxPercentage == null &&
               invoiceData.EnterpriseId == null)
            {
                // Throw error if not enough data is provided
                throw new InvalidOperationException("No hay suficientes datos para modificar la entidad");
            }
            // Every value included to modify is updated
            if (invoiceData.Name!= null) { invoice.Name= invoiceData.Name; }
            if (invoiceData.TaxPercentage != null) { invoice.TaxPercentage = (int)invoiceData.TaxPercentage; }
            if (invoiceData.EnterpriseId != null) { invoice.EnterpriseId = (int)invoiceData.EnterpriseId; }
            
            // Updated data of the invoice related to updation
            invoice.UpdatedBy = userClaims.Identity.Name;
            invoice.UpdatedDate = DateTime.Now;

            // Invoice is updated and changes are saved
            _context.Invoices.Update(invoice);
            _context.Entry(invoice).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Return updated invoice data
            return invoice;
        }

        public async Task<Invoice> DeleteInvoice(GestionFacturasContext _context,
                                                    ClaimsPrincipal userClaims,
                                                    int invoiceId)
        {
            // Search the requested invoice
            var invoice = await GetAvailableInvoice(_context.Invoices, userClaims, invoiceId);

            if (invoice == null)
            {
                // Throw error if no valid invoice was found
                throw new KeyNotFoundException("Factura no encontrada");
            }
            // Every related line of the invoice is removed
            foreach(InvoiceLine invoiceLine in _context.InvoiceLines.Where((InvoiceLine row) => row.InvoiceId == invoice.Id))
            {
                _context.InvoiceLines.Remove(invoiceLine);
            }
            // Updated data of the invoice related to deletion
            invoice.DeletedBy = userClaims.Identity.Name;
            invoice.DeletedDate = DateTime.Now;
            invoice.IsDeleted = true;

            // Invoice is updated and changes are saved
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            // Return deleted invoice data
            return invoice;
        }

        public HtmlToPdfDocument GetInvoicePdf(Enterprise enterprise, Invoice invoice, InvoiceLine[] invoiceLines)
        {
            var globalSetting = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "Factura-" + invoice.Id,
                Out = Path.Combine(Directory.GetCurrentDirectory(),
                                   "invoiceFiles",
                                   invoice.Id + "_invoice.pdf")
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = TemplateGenerator.GetHTMLString(enterprise, invoice, invoiceLines),
                WebSettings = { DefaultEncoding = "utf-8", 
                                UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(),
                                                              "assets",
                                                              "style.css")},
                HeaderSettings = { FontName = "Arial",
                                   FontSize = 9,
                                   Right = "Página [page]/[toPage]",
                                   Line = true},
                FooterSettings  = { FontName = "Arial",
                                   FontSize = 9,
                                   Line = true,
                                   Center = "Reporte de factura"}
            };

            return new HtmlToPdfDocument
            {
                GlobalSettings = globalSetting,
                Objects = { objectSettings }
            };
        }
    }
}
