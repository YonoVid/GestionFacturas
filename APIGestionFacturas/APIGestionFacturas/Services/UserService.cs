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

        public IEnumerable<Enterprise> GetUserEnterprises(IQueryable<Enterprise> enterprises, int id)
        {
            // Return IEnumerable with the id indicated
            return enterprises.Where(enterprise => enterprise.User.Id == id);
        }
        public User? GetUserLogin(IQueryable<User> users, UserAuthorization userLogin)
        {
            // Return user with the provided data
            return users.FirstOrDefault(user => user.Email.ToUpper() == userLogin.Email.ToUpper() &&
                                        user.Password.Equals(userLogin.Password));
        }
        public bool UserExists(IQueryable<User> users, UserAuthorization userToCheck)
        {
            // Return if any user has the email in the data provided
            return users.Any(user => user.Email.ToUpper() == userToCheck.Email.ToUpper());
        }
        public bool UserExists(IQueryable<User> users, User userToCheck)
        {
            // Return if any user has the email in the data provided
            return users.Any(user => user.Email.ToUpper() == userToCheck.Email.ToUpper());
        }
        public async Task<User> CreateUser(GestionFacturasContext _context, ClaimsPrincipal userClaims, UserEditable userData)
        {
            // Create new user from the provided data
            var user = new User(userData);

            // Updated related data of the creation
            user.CreatedBy = userClaims.Identity.Name;

            // Add the user to the database and save the changes
            // Generated user data is updated with genereated id
            user.Id = _context.Users.Add(user).Entity.Id;
            await _context.SaveChangesAsync();

            // Return created user data
            return user;
        }


        public async Task<User> EditUser(GestionFacturasContext _context, ClaimsPrincipal userClaims, UserEditable userData, int userId)
        {
            // Search the requested invoice
            User? user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                // Throw error if no valid user was found
                throw new KeyNotFoundException("Usuario no encontrado");
            }
            
            if((userData.Name == null &&
                userData.Password == null &&
                userData.Email == null &&
                userData.Rol == null))
            {
                // Throw error if not enough data is provided
                throw new InvalidOperationException("No hay suficientes datos para modificar la entidad");
            }
            // Every value included to modify is updated
            if (userData.Name != null) { user.Name = userData.Name; }
            if (userData.Password != null) { user.Password = userData.Password; }
            if (userData.Email != null) { user.Email = userData.Email; }
            if (userData.Rol != null) { user.Rol = (UserRol)userData.Rol; }

            // User is updated and changes are saved
            _context.Users.Update(user);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Return updated user data
            return user;
        }
        public async Task<User> DeleteUser(GestionFacturasContext _context, ClaimsPrincipal userClaims, int userId)
        {
            // Search the requested user
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                // Throw error if no valid user was found
                throw new KeyNotFoundException("Usuario no encontrado");
            }
            // Updated data of the user related to deletion
            user.DeletedBy = userClaims.Identity?.Name;
            user.DeletedDate = DateTime.Now;
            user.IsDeleted = true;

            // User is updated and changes are saved
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Return deleted user data
            return user;
        }
    }
}
