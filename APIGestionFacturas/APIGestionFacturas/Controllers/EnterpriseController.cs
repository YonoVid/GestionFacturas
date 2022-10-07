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


            Enterprise editedEnterprise;

            try
            {
                editedEnterprise = await _enterpriseService.editEnterprise(_context, HttpContext.User, enterpriseData, id);
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
                if (_context.Invoices.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    //_logger.LogWarning($"{nameof(UsersController)} - {nameof(PutUser)}:: UNEXPECTED BEHAVIOUR IN FUNCTION CALL");

                    throw ex;
                }
            }

            return CreatedAtAction("PutEnterprise", new { id = editedEnterprise.Id }, editedEnterprise);
        }

        // POST: api/Enterprise
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<Enterprise>> PostEnterprise(EnterpriseEditable enterpriseData)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(PostUser)}:: RUNNING FUNCTION CALL");

            Enterprise createdEnterprise;

            if(enterpriseData.Name == null) 
            { return BadRequest("Faltan datos para generar la entidad"); }

            try
            {
                createdEnterprise = await _enterpriseService.createEnterprise(_context, HttpContext.User, enterpriseData);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }

            return CreatedAtAction("PostEnterprise", new { id = createdEnterprise.Id }, createdEnterprise);
        }

        // DELETE: api/Enterprise/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> DeleteEnterprise(int id)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(DeleteUser)}:: RUNNING FUNCTION CALL");

            Enterprise deletedEnterprise;

            try
            {
                deletedEnterprise = await _enterpriseService.deleteEnterprise(_context, HttpContext.User, id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
