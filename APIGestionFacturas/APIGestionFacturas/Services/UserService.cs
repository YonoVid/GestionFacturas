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
        private readonly GestionFacturasContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(GestionFacturasContext context,
                              IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<User>> GetUsers()
        {
            // Check base expectations
            CheckBaseExpectations();

            // Return all users from the database
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUser(int id)
        {
            // Check base expectations
            CheckBaseExpectations();

            // Search for the indicated user
            User? user = await _context.Users!.FindAsync(id);

            // Check if user exist
            if (user == null)
            {
                // Throw error if no valid user was found
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            // Return obtained invoice, may be null
            return user;
        }

        public IEnumerable<Enterprise> GetUserEnterprises(int id)
        {
            if (_context.Enterprises == null)
            {
                // Throw error if reference to database is null
                throw new NullReferenceException("Referencia a base de datos en nula");
            }

            // Return IEnumerable with the id indicated
            return _context.Enterprises.Where(enterprise => enterprise.User.Id == id);
        }
        public User? GetUserLogin(UserAuthorization userLogin)
        {
            // Check base expectations
            CheckBaseExpectations();

            // Return user with the provided data
            return _context.Users!.FirstOrDefault(user => user.Email.ToUpper() == userLogin.Email.ToUpper() &&
                                                 user.Password.Equals(userLogin.Password));
        }

        public async Task<User> RegisterUser(UserAuthorization userData)
        {
            // Check base expectations
            var userClaims = CheckBaseExpectations();

            if (userData.Name == null ||
                userData.Email == null ||
                userData.Password == null)
            {
                // Throw error if not enough data is provided
                throw new InvalidOperationException("No hay suficientes datos para modificar la entidad");
            }
            if (UserExists(new User(userData)))
            {
                throw new InvalidOperationException("Usuario con el mismo correo ya existe");
            }

            // Create new user from the provided data
            var user = new User(userData);

            // Updated related data of the creation
            user.CreatedBy = "Admin";
            user.CreatedDate = DateTime.Now;

            // Add the user to the database and save the changes
            // Generated user data is updated with genereated id
            user.Id = _context.Users.Add(user).Entity.Id;
            await _context.SaveChangesAsync();

            // Return created user data
            return user;
        }

        public bool UserExists(UserAuthorization userToCheck)
        {
            // Check base expectations
            CheckBaseExpectations();

            // Return if any user has the email in the data provided
            return _context.Users!.Any(user => user.Email.ToUpper() == userToCheck.Email.ToUpper());
        }
        public bool UserExists(User userToCheck)
        {
            // Check base expectations
            CheckBaseExpectations();

            // Return if any user has the email in the data provided
            return _context.Users!.Any(user => user.Email.ToUpper() == userToCheck.Email.ToUpper());
        }
        public async Task<User> CreateUser(UserEditable userData)
        {
            // Check base expectations
            var userClaims = CheckBaseExpectations();

            if (userData.Name == null ||
                userData.Email == null ||
                userData.Password == null ||
                userData.Rol == null)
             {
                // Throw error if not enough data is provided
                throw new InvalidOperationException("No hay suficientes datos para modificar la entidad");
            }
            if(UserExists(new User(userData)))
            {
                throw new InvalidOperationException("Usuario con el mismo correo ya existe");
            }

            // Create new user from the provided data
            var user = new User(userData);

            // Updated related data of the creation
            user.CreatedBy = userClaims.Identity.Name;
            user.CreatedDate = DateTime.Now;

            // Add the user to the database and save the changes
            // Generated user data is updated with genereated id
            user.Id = _context.Users.Add(user).Entity.Id;
            await _context.SaveChangesAsync();

            // Return created user data
            return user;
        }

        public async Task<User> EditUser(UserEditable userData, int userId)
        {
            // Check base expectations
            var userClaims = CheckBaseExpectations();

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

            // Updated data of the user related to updation
            user.UpdatedBy = userClaims.Identity?.Name;
            user.UpdatedDate = DateTime.Now;
            user.IsDeleted = false;

            // User is updated and changes are saved
            _context.Users.Update(user);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Return updated user data
            return user;
        }
        public async Task<User> DeleteUser(int userId)
        {
            // Check base expectations
            var userClaims = CheckBaseExpectations();

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

        ClaimsPrincipal CheckBaseExpectations()
        {
            if (_context.Users == null)
            {
                // Throw error if reference to database is null
                throw new NullReferenceException("Referencia a base de datos en nula");
            }
            return GetUserClaims();
        }

        ClaimsPrincipal GetUserClaims()
        {
            var userClaims = _httpContextAccessor?.HttpContext?.User;
            if (userClaims == null) { throw new BadHttpRequestException("Datos de usuario que realiza la solicitud no encontrados"); }

            return userClaims;
        }
    }
}
