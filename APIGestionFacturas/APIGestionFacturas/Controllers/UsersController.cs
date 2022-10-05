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
        public async Task<ActionResult<User>> ChangeRol(int id, UserRol rol)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Rol = rol;                                     //Se actualiza el rol
                user.UpdatedBy = HttpContext.User.Identity.Name;    //Se indica el usuario que actualiza
                user.UpdatedDate = DateTime.Now;                    //Se actualiza tiempo de último cambio
                _context.Users.Update(user);                        //Se actualiza la información del usuario
                _context.Entry(user).State = EntityState.Modified;
                //Actualización asíncrona de la base de datos
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUser", new { id = user.Id }, user);
            }

            return BadRequest("Usuario no encontrado");

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

            User? user = await _context.Users.FindAsync(id);

            if(user == null ||
                (userData.Name == null &&
                userData.Password == null &&
                userData.Email == null &&
                userData.Rol == null))
            {
                return BadRequest();
            }

            try
            {
                //Se actualiza cada propiedad del usuario si se ha incluido en los datos recibidos
                if (userData.Name != null) { user.Name = userData.Name; }
                if (userData.Password != null) { user.Password = userData.Password; }
                if (userData.Email != null) { user.Email= userData.Email; }
                if (userData.Rol != null) { user.Rol = (UserRol)userData.Rol; }

                //Se actualizan la base de datos con los cambios
                _context.Users.Update(user);

                _context.Entry(user).State = EntityState.Modified;

                await _context.SaveChangesAsync();  //Se guardan los cambios de manera asíncrona
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_userService.userExists(_context.Users, user))
                {
                    return NotFound();
                }
                else
                {
                    //_logger.LogWarning($"{nameof(UsersController)} - {nameof(PutUser)}:: UNEXPECTED BEHAVIOUR IN FUNCTION CALL");

                    throw ex;
                }
            }

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<User>> PostUser(UserEditable userData)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(PostUser)}:: RUNNING FUNCTION CALL");

            if(userData.Name == null ||
                userData.Email == null ||
                userData.Password == null ||
                userData.Rol == null) 
            { return BadRequest("Faltan datos para generar la entidad"); }

            var user = new User(userData);
            user.CreatedBy = HttpContext.User.Identity.Name;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(DeleteUser)}:: RUNNING FUNCTION CALL");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.DeletedBy = HttpContext.User.Identity.Name;
            user.DeletedDate = DateTime.Now;
            user.IsDeleted = true;


            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
