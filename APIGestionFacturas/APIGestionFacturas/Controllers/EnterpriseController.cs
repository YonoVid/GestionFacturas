using APIGestionFacturas.DataAccess;
using APIGestionFacturas.Services;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIGestionFacturas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnterpriseController : ControllerBase
    {
        private readonly GestionFacturasContext _context;           //Contexto con las referencias a tablas de la base de datos
        private readonly IEnterpriseService _enterpriseService;     //Servicio relacionado a las empresas
        private readonly JwtSettings _jwtSettings;                  //Configuración de JWT

        public EnterpriseController(GestionFacturasContext context,
                            IEnterpriseService enterpriseService,
                            JwtSettings jwtSettings)
        {
            _context = context;
            _enterpriseService = enterpriseService;
            _jwtSettings = jwtSettings;
        }

        // **** CRUD de la tabla ****

        // GET: api/Enterprise
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<Enterprise>>> GetEnterprises()
        {
            var enterprises = await _enterpriseService.getAvailableEnterprises(_context.Enterprises, HttpContext.User).ToListAsync();
            if (enterprises != null)
            {
                return enterprises;
            }
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(GetUsers)}:: RUNNING FUNCTION CALL");
            return new List<Enterprise>();
        }

        // GET: api/Enterprise/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<Enterprise>> GetEnterprise(int id)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(GetUsers)}:: RUNNING FUNCTION CALL");

            var enterprise = await _enterpriseService.getAvailableEnterprise(_context.Enterprises, HttpContext.User, id);

            if (enterprise == null)
            {
                return NotFound();
            }

            return enterprise;
        }

        // PUT: api/Enterprise/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> PutEnterprise(int id, EnterpriseEditable enterpriseData)
        {
            // _logger.LogInformation($"{nameof(UsersController)} - {nameof(PutUser)}:: RUNNING FUNCTION CALL");

            Enterprise? enterprise= await _context.Enterprises.FindAsync(id);

            if (enterprise == null)
            {
                return NotFound();
            }
            else if (enterpriseData.Name == null &&
                    enterpriseData.UserId == null)
            {
                return BadRequest();
            }

            try
            {
                if (enterpriseData.Name != null) { enterprise.Name = enterpriseData.Name; }
                if (enterpriseData.UserId != null) { enterprise.UserId = enterpriseData.UserId; }


                _context.Enterprises.Update(enterprise);

                _context.Entry(enterprise).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //_logger.LogWarning($"{nameof(UsersController)} - {nameof(PutUser)}:: UNEXPECTED BEHAVIOUR IN FUNCTION CALL");
                throw ex;
            }

            return CreatedAtAction("PutEnterprise", new { id = enterprise.Id }, enterprise);
        }

        // POST: api/Enterprise
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<Enterprise>> PostEnterprise(EnterpriseEditable enterpriseData)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(PostUser)}:: RUNNING FUNCTION CALL");

            if(enterpriseData.Name == null) 
            { return BadRequest("Faltan datos para generar la entidad"); }

            var enterprise = new Enterprise(enterpriseData);

            if (enterpriseData.UserId != null)
            {
                enterprise.User = await _context.Users.FindAsync(enterpriseData.UserId);
                if (enterprise.User == null) { return BadRequest("Id de usuario no encontrado"); }
            }

            enterprise.CreatedBy = HttpContext.User.Identity.Name;
            _context.Enterprises.Add(enterprise);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostEnterprise", new { id = enterprise.Id }, enterprise);
        }

        // DELETE: api/Enterprise/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> DeleteEnterprise(int id)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(DeleteUser)}:: RUNNING FUNCTION CALL");

            var enterprise = await _context.Enterprises.FindAsync(id);
            if (enterprise == null)
            {
                return NotFound();
            }

            enterprise.DeletedBy = HttpContext.User.Identity.Name;
            enterprise.DeletedDate = DateTime.Now;
            enterprise.IsDeleted = true;


            _context.Enterprises.Update(enterprise);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
