using GestionFacturasModelo.Model.DataModel;

namespace APIGestionFacturas.Services
{
    public interface IUserService
    {
        IEnumerable<Enterprise> getUserEnterprises(IQueryable<User> users, int id);
        User? getUserLogin(IQueryable<User> users, UserLogin userLogin);

        Boolean userExists(IQueryable<User> users, UserLogin userLogin);
        Boolean userExists(IQueryable<User> users, User user);

    }
}
