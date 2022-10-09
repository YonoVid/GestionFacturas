using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public class EnterpriseService : IEnterpriseService
    {
        public IQueryable<Enterprise>? GetAvailableEnterprises(IQueryable<Enterprise> enterprises, ClaimsPrincipal userClaims)
        {
            // Check if user is 'Administrator'
            if (!userClaims.IsInRole("Administrator"))
            {
                // Check if user identity is not null
                var identity = userClaims.Identity as ClaimsIdentity;
                if (identity != null)
                {
                   // Return all invoices with a enterprise managed by the user
                   var id = identity?.FindFirst("Id")?.Value;
                   return enterprises.Where((enterprise) => enterprise.User.Id.ToString() == id &&
                                                            !enterprise.IsDeleted);
                }
                return null;
            }
            // Return obtained enteprises, may be null
            return enterprises;
        }
        public async Task<Enterprise?> GetAvailableEnterprise(DbSet<Enterprise> enterprises, ClaimsPrincipal userClaims, int id)
        {
            // Search for the indicated enterprise
            Enterprise? result = await enterprises.FindAsync(id);

            // Check if user is 'Administrator'
            if (!userClaims.IsInRole("Administrator") && result != null)
            {
                // Get enterprise user id and check if the user making the call is the same
                var identity = userClaims.Identity as ClaimsIdentity;
                if (result.UserId.ToString() != identity?.FindFirst("Id")?.Value)
                {
                    // Return null if the user doesn't have permission
                    return null;
                }
            }
            // Return obtained enterprise, may be null
            return result;
        }

        public async Task<Enterprise> CreateEnterprise(GestionFacturasContext _context,
                                              ClaimsPrincipal userClaims,
                                              EnterpriseEditable enterpriseData)
        {
            // Create new enterprise from the provided data
            var enterprise = new Enterprise(enterpriseData);

            // Check if user should be associated to the enterprise
            if (enterpriseData.UserId != null)
            {
                // If user is not found throw a error
                enterprise.User = await _context.Users.FindAsync(enterpriseData.UserId);
                if (enterprise.User == null) { throw new KeyNotFoundException("Id de usuario no encontrado"); }
            }
            // Updated related data of the creation
            enterprise.CreatedBy = userClaims.Identity.Name;

            // Add the enterprise to the database and save the changes
            // Generated invoice data is updated with genereated id
            enterprise.Id = _context.Enterprises.Add(enterprise).Entity.Id;
            await _context.SaveChangesAsync();

            // Return created enterprise data
            return enterprise;
        }


        public async Task<Enterprise> EditEnterprise(GestionFacturasContext _context,
                                            ClaimsPrincipal userClaims,
                                            EnterpriseEditable enterpriseData,
                                            int enterpriseId)
        {
            // Search the requested enterprise
            Enterprise? enterprise = await _context.Enterprises.FindAsync(enterpriseId);

            if (enterprise == null)
            {
                // Throw error if no valid enterprise was found
                throw new KeyNotFoundException("Empresa no encontrada");
            }
            else if (enterpriseData.Name == null &&
                    enterpriseData.UserId == null)
            {
                // Throw error if not enough data is provided
                throw new InvalidOperationException("No hay suficientes datos para modificar la entidad");
            }
            // Every value included to modify is updated
            if (enterpriseData.Name != null) { enterprise.Name = enterpriseData.Name; }
            if (enterpriseData.UserId != null) { enterprise.UserId = enterpriseData.UserId; }
            
            // Updated data of the enterprise related to updation
            enterprise.UpdatedBy = userClaims.Identity.Name;
            enterprise.UpdatedDate = DateTime.Now;

            // Enterprise is updated and changes are saved
            _context.Enterprises.Update(enterprise);
            _context.Entry(enterprise).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Return updated enterprise data
            return enterprise;
        }
        public async Task<Enterprise> DeleteEnterprise(GestionFacturasContext _context,
                                                       ClaimsPrincipal userClaims,
                                                       int enterpriseId)
        {
            // Search the requested enterprise
            var enterprise = await _context.Enterprises.FindAsync(enterpriseId);

            if (enterprise == null)
            {
                // Throw error if no valid enterprise was found
                throw new KeyNotFoundException("Factura no encontrada");
            }

            // Update data of every related entity to the enterprise
            foreach(Invoice invoice in _context.Invoices.Where((Invoice row) => row.EnterpriseId == enterprise.Id))
            {
                foreach (InvoiceLine invoiceLine in _context.InvoiceLines.Where((InvoiceLine row) => row.InvoiceId == invoice.Id))
                {
                    // Delete every invoice lines of the invoice
                    _context.InvoiceLines.Update(invoiceLine);
                }
                // Updated data of the invoice related to deletion
                invoice.DeletedBy = userClaims.Identity.Name;
                invoice.DeletedDate = DateTime.Now;
                invoice.IsDeleted = true;

                // Invoice is updated
                _context.Invoices.Update(invoice);
            }
            // Updated data of the enterprise related to deletion
            enterprise.DeletedBy = userClaims.Identity.Name;
            enterprise.DeletedDate = DateTime.Now;
            enterprise.IsDeleted = true;

            // Enterprise is updated and changes are saved
            _context.Enterprises.Update(enterprise);
            await _context.SaveChangesAsync();

            // Return deleted enterprise data
            return enterprise;
        }
    }
}
