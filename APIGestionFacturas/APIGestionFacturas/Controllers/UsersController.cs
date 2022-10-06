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
        private readonly GestionFacturasContext _context;       //Contexto con las referencias a tablas de la base de datos
        private readonly IUserService _userService;             //Servicio relacionado a los usuarios
        private readonly JwtSettings _jwtSettings;              //Configuración de JWT

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
                var Token = new UserToken();
                var Valid = _userService.userExists(_context.Users, userLogin);

                if (Valid)
                {
                    var user = _userService.getUserLogin(_context.Users, userLogin);
                    if (user != null)
                    {
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
        public async Task<ActionResult<User>> Register(User user)
        {
            if(!_userService.userExists(_context.Users, user))
            {
                user.CreatedBy = "Admin";

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                user.Password = "*********";
                return CreatedAtAction("GetUser", new { id = user.Id }, user);
            }

            return BadRequest("El usuario ya existe");
            
        }

        [HttpPost]
        [Route("[action]/{id}&{rol}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> ChangeRol(int id, UserRol rol)
        {
            UserEditable userData = new UserEditable();
            userData.Rol = rol;

            return await PutUser(id, userData);

        }


        // **** CRUD de la tabla ****

        // GET: api/Users
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(GetUsers)}:: RUNNING FUNCTION CALL");
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(GetUsers)}:: RUNNING FUNCTION CALL");
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> PutUser(int id, UserEditable userData)
        {
            // _logger.LogInformation($"{nameof(UsersController)} - {nameof(PutUser)}:: RUNNING FUNCTION CALL");

            User userEdited;
            try
            {
                userEdited = await _userService.editUser(_context, HttpContext.User, userData, id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_userService.userExists(_context.Users, new User(userData)))
                {
                    return NotFound();
                }
                else
                {
                    //_logger.LogWarning($"{nameof(UsersController)} - {nameof(PutUser)}:: UNEXPECTED BEHAVIOUR IN FUNCTION CALL");

                    throw ex;
                }
            }

            return CreatedAtAction("PutUser", new { id = userEdited.Id }, userEdited);
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<User>> PostUser(UserEditable userData)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(PostUser)}:: RUNNING FUNCTION CALL");
            User userCreated;
            if(userData.Name == null ||
                userData.Email == null ||
                userData.Password == null ||
                userData.Rol == null) 
            { return BadRequest("Faltan datos para generar la entidad"); }

            try
            {
                userCreated = await _userService.createUser(_context, HttpContext.User, userData);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }

            return CreatedAtAction("PostUser", new { id = userCreated.Id }, userCreated);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(DeleteUser)}:: RUNNING FUNCTION CALL");

            User deletedUser;
            try
            {
                deletedUser= await _userService.deleteUser(_context, HttpContext.User, id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
