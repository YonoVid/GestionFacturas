using GestionFacturasModelo.Model.DataModel;

namespace APIGestionFacturas.Services
{
    public class UserService : IUserService
    {
        public IEnumerable<Enterprise> getUserEnterprises()
        {
            throw new NotImplementedException();
        }

        public User getUserLogin(string email, string password)
        {
            throw new NotImplementedException();
        }

        public bool userExists(int id)
        {
            return false;//_context.Users.Any(e => e.Id == id);
        }
    }
}
