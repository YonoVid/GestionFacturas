using GestionFacturasModelo.Model.DataModel;

namespace APIGestionFacturas.Services
{
    public interface IUserService
    {
        IEnumerable<Enterprise> getUserEnterprises();
        User getUserLogin(string email, string password);

        Boolean userExists(int id);

    }
}
