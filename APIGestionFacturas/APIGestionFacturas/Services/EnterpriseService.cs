using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public class EnterpriseService : IEnterpriseService
    {
        public IQueryable<Enterprise> getAvailableEnterprises(IQueryable<Enterprise> enterprises, ClaimsPrincipal userClaims)
        {
            if(!userClaims.IsInRole("Administrator"))
            {
                var identity = userClaims.Identity as ClaimsIdentity;
                if (identity != null)
                {
                   var id = identity.FindFirst("Id").Value;
                   return enterprises.Where((enterprise) => enterprise.User.Id.ToString() == id && !enterprise.IsDeleted);

                }
                return null;
            }
            return enterprises;
        }
        public async Task<Enterprise> getAvailableEnterprise(DbSet<Enterprise> enterprises, ClaimsPrincipal userClaims, int id)
        {

            Enterprise result = await enterprises.FindAsync(id);

            if(!userClaims.IsInRole("Administrator") && result != null)
            {
                var identity = userClaims.Identity as ClaimsIdentity;

                int idToken = int.Parse(identity.FindFirst("Id").Value);

                if (result.UserId != idToken)
                {
                    return null;
                }
            }

            return result;
        }

        public async Task<Enterprise> createEnterprise(GestionFacturasContext _context,
                                              ClaimsPrincipal userClaims,
                                              EnterpriseEditable enterpriseData)
        {
            var enterprise = new Enterprise(enterpriseData);

            if (enterpriseData.UserId != null)
            {
                enterprise.User = await _context.Users.FindAsync(enterpriseData.UserId);
                if (enterprise.User == null) { throw new KeyNotFoundException("Id de usuario no encontrado"); }
            }

            enterprise.CreatedBy = userClaims.Identity.Name;

            _context.Enterprises.Add(enterprise);
            await _context.SaveChangesAsync();

            return enterprise;
        }


        public async Task<Enterprise> editEnterprise(GestionFacturasContext _context,
                                            ClaimsPrincipal userClaims,
                                            EnterpriseEditable enterpriseData,
                                            int enterpriseId)
        {
            Enterprise? enterprise = await _context.Enterprises.FindAsync(enterpriseId);

            if (enterprise == null)
            {
                throw new KeyNotFoundException("Empresa no encontrada");
            }
            else if (enterpriseData.Name == null &&
                    enterpriseData.UserId == null)
            {
                throw new InvalidOperationException("No hay suficientes datos para modificar la entidad");
            }

            if (enterpriseData.Name != null) { enterprise.Name = enterpriseData.Name; }
            if (enterpriseData.UserId != null) { enterprise.UserId = enterpriseData.UserId; }

            enterprise.UpdatedBy = userClaims.Identity.Name;
            enterprise.UpdatedDate = DateTime.Now;

            _context.Enterprises.Update(enterprise);

            _context.Entry(enterprise).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return enterprise;
        }
        public async Task<Enterprise> deleteEnterprise(GestionFacturasContext _context,
                                                       ClaimsPrincipal userClaims,
                                                       int enterpriseId)
        {
            var enterprise = await _context.Enterprises.FindAsync(enterpriseId);
            if (enterprise == null)
            {
                throw new KeyNotFoundException("Factura no encontrada");
            }

            foreach(Invoice invoice in _context.Invoices.Where((Invoice row) => row.EnterpriseId == enterprise.Id))
            {
                foreach (InvoiceLine invoiceLine in _context.InvoiceLines.Where((InvoiceLine row) => row.InvoiceId == invoice.Id))
                {
                    _context.InvoiceLines.Update(invoiceLine);
                }
                invoice.DeletedBy = userClaims.Identity.Name;
                invoice.DeletedDate = DateTime.Now;
                invoice.IsDeleted = true;
                _context.Invoices.Update(invoice);
            }

            enterprise.DeletedBy = userClaims.Identity.Name;
            enterprise.DeletedDate = DateTime.Now;
            enterprise.IsDeleted = true;


            _context.Enterprises.Update(enterprise);
            await _context.SaveChangesAsync();

            return enterprise;
        }
    }
}
