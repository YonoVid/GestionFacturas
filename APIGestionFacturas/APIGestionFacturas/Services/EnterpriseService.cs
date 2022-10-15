using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public class EnterpriseService : IEnterpriseService
    {
        private readonly GestionFacturasContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EnterpriseService(GestionFacturasContext context,
                                 IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<Enterprise> GetAvailableEnterprises()
        {
            if (_context.Enterprises == null)
            {
                // Throw error if reference to database is null
                throw new NullReferenceException("Referencia a base de datos en nula");
            }
            // Get user claims
            var userClaims = getUserClaims();

            // Check if user is 'Administrator'
            if (!userClaims.IsInRole("Administrator"))
            {
                // Check if user identity is not null and try to get id
                var identity = userClaims.Identity as ClaimsIdentity;
                var id = identity?.FindFirst("Id")?.Value;

                if (id != null)
                {
                   // Return all invoices with a enterprise managed by the user
                   return _context.Enterprises.Where((enterprise) => enterprise.Id.ToString().Equals(id) &&
                                                                     !enterprise.IsDeleted);
                }
                // Return empty list if no valid identity founded
                return Enumerable.Empty<Enterprise>().AsQueryable(); ;
            }
            // Return obtained enteprises, may be null
            return _context.Enterprises;
        }
        public async Task<Enterprise> GetAvailableEnterprise(int id)
        {
            if (_context.Enterprises == null)
            {
                // Throw error if reference to database is null
                throw new NullReferenceException("Referencia a base de datos en nula");
            }
            // Get user claims
            var userClaims = getUserClaims();

            // Search for the indicated enterprises
            Enterprise? result = await _context.Enterprises.FindAsync(id);

            // Check if user is 'Administrator'
            if (!userClaims.IsInRole("Administrator") && result != null)
            {
                // Get enterprise user id and check if the user making the call is the same
                var identity = userClaims.Identity as ClaimsIdentity;
                if (result.UserId.ToString() != identity?.FindFirst("Id")?.Value)
                {
                    // Return null if the user doesn't have permission
                    result = null;
                }
            }
            if(result == null)
            {
                // Throw error if no valid enterprise was founded
                throw new KeyNotFoundException("Empresa no encontrada");
            }

            // Return obtained enterprise, may be null
            return result;
        }

        public async Task<Enterprise> CreateEnterprise(EnterpriseEditable enterpriseData)
        {
            if (_context.Enterprises == null || _context.Users == null)
            {
                // Throw error if reference to database is null
                throw new NullReferenceException("Referencia a base de datos en nula");
            }

            if (enterpriseData.Name == null)
            {
                // Throw error if not enough data is provided
                throw new InvalidOperationException("Faltan datos para generar la entidad"); 
            }
            // Get user claims
            var userClaims = getUserClaims();

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
            if(userClaims.Identity?.Name == null && !userClaims.IsInRole("Administrator")) 
                { throw new NullReferenceException("Identidad de petición es nula"); }
            // Identity only is allowed to be empty if user is administrator
            enterprise.CreatedBy = userClaims.Identity?.Name ?? "Admin";

            // Add the enterprise to the database and save the changes
            // Generated invoice data is updated with genereated id
            _context.Enterprises.Add(enterprise);
            await _context.SaveChangesAsync();

            // Return created enterprise data
            return enterprise;
        }


        public async Task<Enterprise> EditEnterprise(EnterpriseEditable enterpriseData,
                                                     int enterpriseId)
        {
            if (_context.Users == null)
            {
                // Throw error if reference to database is null
                throw new NullReferenceException("Referencia a base de datos en nula");
            }
            // Get user claims
            var userClaims = getUserClaims();

            // Search the requested enterprise, also checks if Enterprise context is null
            var enterprise = await GetAvailableEnterprise(enterpriseId);

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
            if (enterpriseData.UserId != null)
            {
                if(await _context.Users.FindAsync(enterpriseData.UserId) == null)
                {
                    // Throw error if no valid enterprise was found
                    throw new KeyNotFoundException("Usuario no encontrado");
                }
                enterprise.UserId = enterpriseData.UserId; 
            }

            // Updated data of the enterprise related to updation
            // Identity only is allowed to be empty if user is administrator
            enterprise.UpdatedBy = userClaims.Identity?.Name ?? "Admin";
            enterprise.UpdatedDate = DateTime.Now;

            // Enterprise is updated and changes are saved
            _context.Enterprises!.Update(enterprise);
            _context.SetModified(enterprise);
            await _context.SaveChangesAsync();

            // Return updated enterprise data
            return enterprise;
        }
        public async Task<Enterprise> DeleteEnterprise(int enterpriseId)
        {
            if (_context.Invoices == null ||
                _context.InvoiceLines == null)
            {
                // Throw error if reference to database is null
                throw new NullReferenceException("Referencia a base de datos en nula");
            }
            // Get user claims
            var userClaims = getUserClaims();

            // Search the requested enterprise, also checks if Enterprise context is null
            var enterprise = await GetAvailableEnterprise(enterpriseId);

            if (enterprise == null)
            {
                // Throw error if no valid enterprise was found
                throw new KeyNotFoundException("Empresa no encontrada");
            }

            // Update data of every related entity to the enterprise
            foreach(Invoice invoice in _context.Invoices.Where((Invoice row) => row.EnterpriseId == enterprise.Id))
            {
                // Updated data of the invoice related to deletion
                // Identity only is allowed to be empty if user is administrator
                invoice.DeletedBy = userClaims.Identity?.Name ?? "Admin";
                invoice.DeletedDate = DateTime.Now;
                invoice.IsDeleted = true;

                // Invoice is updated
                _context.Invoices.Update(invoice);
                _context.SetModified(invoice);
            }
            // Updated data of the enterprise related to deletion
            // Identity only is allowed to be empty if user is administrator
            enterprise.DeletedBy = userClaims.Identity?.Name ?? "Admin";
            enterprise.DeletedDate = DateTime.Now;
            enterprise.IsDeleted = true;

            // Enterprise is updated and changes are saved
            _context.Enterprises!.Update(enterprise);
            _context.SetModified(enterprise);
            await _context.SaveChangesAsync();

            // Return deleted enterprise data
            return enterprise;
        }

        public ClaimsPrincipal getUserClaims()
        {
            var userClaims = _httpContextAccessor?.HttpContext?.User;
            if(userClaims == null) { throw new BadHttpRequestException("Datos de usuario que realiza la solicitud no encontrados"); }

            return userClaims;
        }
    }
}
