﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using APIGestionFacturas.Services;
using APIGestionFacturas.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace APIGestionFacturas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly GestionFacturasContext _context;
        private readonly IUserService _userService;
        private readonly JwtSettings _jwtSettings;

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
                    else return BadRequest("Wrong password");
                }
                else return BadRequest("User not found");

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
        public async Task<IActionResult> PutUser(int id, UserEditableTemplate userData)
        {
           // _logger.LogInformation($"{nameof(UsersController)} - {nameof(PutUser)}:: RUNNING FUNCTION CALL");

            User? user = await _context.Users.FindAsync(id);

            if(user == null)
            {
                return BadRequest();
            }

            try
            {
                if (userData.Name != null) { user.Name = userData.Name; }
                if (userData.Password != null) { user.Password = userData.Password; }
                if (userData.Email != null) { user.Email= userData.Email; }
                if (userData.Rol != null) { user.Rol = (UserRol)userData.Rol; }

                _context.Users.Update(user);

                _context.Entry(user).State = EntityState.Modified;

                await _context.SaveChangesAsync();
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
        public async Task<ActionResult<User>> PostUser(User user)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(PostUser)}:: RUNNING FUNCTION CALL");

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

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
