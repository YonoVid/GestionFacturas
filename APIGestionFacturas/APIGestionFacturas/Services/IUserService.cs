using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public interface IUserService
    {
        IEnumerable<Enterprise> getUserEnterprises(IQueryable<Enterprise> enterprises, int id);
        User? getUserLogin(IQueryable<User> users, UserAuthorization userLogin);

        Boolean userExists(IQueryable<User> users, UserAuthorization userLogin);
        Boolean userExists(IQueryable<User> users, User user);

        Task<User> createUser(GestionFacturasContext _context,
                                    ClaimsPrincipal userClaims,
                                    UserEditable userData);

        Task<User> deleteUser(GestionFacturasContext _context,
                                    ClaimsPrincipal userClaims,
                                    int userId);

        Task<User> editUser(GestionFacturasContext _context,
                                  ClaimsPrincipal userClaims,
                                  UserEditable userData,
                                  int userId);
    }
}
