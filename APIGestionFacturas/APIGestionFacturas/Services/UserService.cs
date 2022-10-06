using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public class UserService : IUserService
    {

        public IEnumerable<Enterprise> getUserEnterprises(IQueryable<Enterprise> enterprises, int id)
        {

            if (enterprises.Any(enterprises => enterprises.User.Id == id))
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
        public async Task<User> createUser(GestionFacturasContext _context, ClaimsPrincipal userClaims, UserEditable userData)
        {
            var user = new User(userData);
            user.CreatedBy = userClaims.Identity.Name;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }


        public async Task<User> editUser(GestionFacturasContext _context, ClaimsPrincipal userClaims, UserEditable userData, int userId)
        {
            User? user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado");
            }
            
            if((userData.Name == null &&
                userData.Password == null &&
                userData.Email == null &&
                userData.Rol == null))
            {
                throw new InvalidOperationException("No hay suficientes datos para modificar la entidad");
            }

            //Se actualiza cada propiedad del usuario si se ha incluido en los datos recibidos
            if (userData.Name != null) { user.Name = userData.Name; }
            if (userData.Password != null) { user.Password = userData.Password; }
            if (userData.Email != null) { user.Email = userData.Email; }
            if (userData.Rol != null) { user.Rol = (UserRol)userData.Rol; }

            //Se actualizan la base de datos con los cambios
            _context.Users.Update(user);

            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();  //Se guardan los cambios de manera asíncrona

            return user;
        }
        public async Task<User> deleteUser(GestionFacturasContext _context, ClaimsPrincipal userClaims, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            user.DeletedBy = userClaims.Identity.Name;
            user.DeletedDate = DateTime.Now;
            user.IsDeleted = true;


            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
