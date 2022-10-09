using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using APIGestionFacturas.Services;
using APIGestionFacturas.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using GestionFacturasModelo.Model.Templates;

namespace APIGestionFacturas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly GestionFacturasContext _context;
        private readonly IUserService _userService;
        private readonly JwtSettings _jwtSettings;

        //Initialize services
        public UsersController(GestionFacturasContext context,
                                IUserService userService,
                                JwtSettings jwtSettings)
        {
            _context = context;
            _userService = userService;
            _jwtSettings = jwtSettings;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Login(UserAuthorization userLogin)
        {
            try
            {
                // Generate new UserToken class
                var Token = new UserToken();
                // Check if user exists
                var Valid = _userService.UserExists(_context.Users, userLogin);

                if (Valid)
                {
                    // Check if user login data is correct
                    var user = _userService.GetUserLogin(_context.Users, userLogin);
                    if (user != null)
                    {
                        // Create new Token
                        Token = JwtHelpers.GenTokenKey(new UserToken()
                        {
                            UserName = user.Name,
                            EmailId = user.Email,
                            Id = user.Id,
                            UserRol= user.Rol,
                            GuidId = Guid.NewGuid()
                        }, _jwtSettings);
                    }
                    else return BadRequest("Contraseña incorrecta");
                }
                else return BadRequest("Usuario no encontrado");

                // Return generated token
                return Ok(new
                {
                    Token,
                    Token.UserName
                });
            }
            catch (Exception ex)
            {

                throw new Exception("GetToken error", ex);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<User>> Register(UserAuthorization userData)
        {
            // Generate new User class from the data
            User user = new User(userData);

            // Check if user with same unique data already exists
            if(!_userService.UserExists(_context.Users, user))
            {
                // Create and save user
                user.CreatedBy = "Admin";
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Hide password
                user.Password = "*********";

                // Return created user
                return CreatedAtAction("GetUser", new { id = user.Id }, user);
            }

            // If the user exists return BadRequest
            return BadRequest("El usuario ya existe");
        }

        [HttpPost]
        [Route("[action]/{id}&{rol}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> ChangeRol(int id, UserRol rol)
        {
            // Create user editable with the new rol
            UserEditable userData = new UserEditable();
            userData.Rol = rol;

            // Call controller function to modify user data
            return await PutUser(id, userData);

        }


        // **** CRUD de la tabla ****

        // GET: api/Users
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            // Return all users from the database
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            // Search selected user with the id in the database
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                // If user isn't found send NotFound result
                return NotFound();
            }
            // Return founded user
            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> PutUser(int id, UserEditable userData)
        {
            // Define variable to store edited user
            User userEdited;
            try
            {
                // Use service to obtain edited user and store it
                userEdited = await _userService.EditUser(_context, HttpContext.User, userData, id);
            }
            catch (KeyNotFoundException ex)
            {
                // If key is not found return NotFound result
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // If not enough data is provided return BadRequest result
                return BadRequest(ex.Message);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // For any other error from the database throw an exception
                throw ex;
            }

            // Return data from the edited user
            return CreatedAtAction("PutUser", new { id = userEdited.Id }, userEdited);
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<User>> PostUser(UserEditable userData)
        {
            // Create variable to store created user
            User userCreated;
            try
            {
                // Use service to create and store user
                userCreated = await _userService.CreateUser(_context, HttpContext.User, userData);
            }
            catch(InvalidOperationException ex)
            {
                // If not enough data is provided return BadRequest result
                return BadRequest(ex.Message);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // For any other error from the database throw an exception
                throw ex;
            }
            // Return data from the created user
            return CreatedAtAction("PostUser", new { id = userCreated.Id }, userCreated);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Create variable to store deleted user
            User deletedUser;
            try
            {
                // Use service to obtain deleted user and store it
                deletedUser = await _userService.DeleteUser(_context, HttpContext.User, id);
            }
            catch (KeyNotFoundException ex)
            {
                // If the user is not founded
                return NotFound();
            }

            // Return message to indicate action was successful
            return Ok();
        }
    }
}
