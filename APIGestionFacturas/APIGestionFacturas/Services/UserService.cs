using GestionFacturasModelo.Model.DataModel;

namespace APIGestionFacturas.Services
{
    public class UserService : IUserService
    {
        public IEnumerable<Enterprise> getUserEnterprises(IQueryable<Enterprise> enterprises, int id)
        {

            if(enterprises.Any(enterprises => enterprises.User.Id == id))
            {
                return enterprises.Where(enterprise => enterprise.User.Id == id);
            }
            return new List<Enterprise>();
        }

        public User? getUserLogin(IQueryable<User> users, UserAuthorization userLogin)
        {
            return users.FirstOrDefault(user => user.Email.ToUpper() == userLogin.Email.ToUpper() &&
                                        user.Password.Equals(userLogin.Password));
        }

        public bool userExists(IQueryable<User> users, UserAuthorization userToCheck)
        {
            return users.Any(user => user.Email.ToUpper() == userToCheck.Email.ToUpper());//_context.Users.Any(e => e.Id == id);
        }
        public bool userExists(IQueryable<User> users, User userToCheck)
        {
            return users.Any(user => user.Email.ToUpper() == userToCheck.Email.ToUpper());//_context.Users.Any(e => e.Id == id);
        }
    }
}
