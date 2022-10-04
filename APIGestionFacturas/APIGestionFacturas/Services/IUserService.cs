using GestionFacturasModelo.Model.DataModel;

namespace APIGestionFacturas.Services
{
    public interface IUserService
    {
        IEnumerable<Enterprise> getUserEnterprises(IQueryable<Enterprise> enterprises, int id);
        User? getUserLogin(IQueryable<User> users, UserAuthorization userLogin);

        Boolean userExists(IQueryable<User> users, UserAuthorization userLogin);
        Boolean userExists(IQueryable<User> users, User user);

    }
}
