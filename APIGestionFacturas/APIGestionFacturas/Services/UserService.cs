using GestionFacturasModelo.Model.DataModel;

namespace APIGestionFacturas.Services
{
    public class UserService : IUserService
    {
        public IEnumerable<Enterprise> getUserEnterprises(IQueryable<User> users, int id)
        {

            if(users.Any(user => user.Id == id))
            {
                return users.FirstOrDefault(user => user.Id == id).Enterprises;
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
