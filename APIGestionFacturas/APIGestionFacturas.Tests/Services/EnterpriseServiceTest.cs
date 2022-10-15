using APIGestionFacturas.DataAccess;
using APIGestionFacturas.Services;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace APIGestionFacturas.Tests.Services
{
    public class EnterpriseServiceTest: IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly EnterpriseService _service;

        private List<Enterprise> enterprises;
        private List<Invoice> invoices;
        private List<InvoiceLine> invoiceLines;
        private List<User> users;
        private EnterpriseEditable editable;
        private string httpUser;

        private Mock<GestionFacturasContext> _context;
        private Mock<IHttpContextAccessor> _mockHttp;

        public EnterpriseServiceTest(ITestOutputHelper testOutputHelper) : base()
        {
            enterprises = MockDbHelper.GetEnterpriseList();
            invoices = MockDbHelper.GetInvoiceList();
            invoiceLines = MockDbHelper.GetInvoiceLineList();
            users = MockDbHelper.GetUserList();


            editable = new EnterpriseEditable
            {
                Name = enterprises[0].Name,
                UserId = enterprises[0].UserId
            };

            httpUser = "Test identity";

            _context = new Mock<GestionFacturasContext>();

            _context.SetupGet(p => p.Enterprises).Returns(MockDbContext.GetQueryableMockDbSet<Enterprise>(enterprises));
            _context.SetupGet(p => p.Invoices).Returns(MockDbContext.GetQueryableMockDbSet<Invoice>(invoices));
            _context.SetupGet(p => p.InvoiceLines).Returns(MockDbContext.GetQueryableMockDbSet<InvoiceLine>(invoiceLines));
            _context.SetupGet(p => p.Users).Returns(MockDbContext.GetQueryableMockDbSet<User>(users));

            _context.Setup(p => p.SetModified(It.IsAny<Enterprise>()));
            _context.Setup(p => p.SaveChanges()).Returns(1);
            _context.Setup(p => p.SaveChangesAsync(default)).Returns(Task.FromResult(1));


            _mockHttp = new();
            var httpContext = new DefaultHttpContext();
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Name, httpUser));
            identity.AddClaim(new Claim("Id", users[0].Id.ToString()));

            httpContext.User = new ClaimsPrincipal(identity);
            
            httpContext.User.AddIdentity(identity);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            _service = new EnterpriseService(_context.Object, _mockHttp.Object);

            _testOutputHelper = testOutputHelper;
        }

        public void Dispose()
        {
            enterprises = MockDbHelper.GetEnterpriseList();
            invoices = MockDbHelper.GetInvoiceList();
            invoiceLines = MockDbHelper.GetInvoiceLineList();
            users = MockDbHelper.GetUserList();
        }

        [Fact]
        public async void GetEnterprisesSuccessful()
        {
            var stored = _context.Object.Enterprises.FirstOrDefault();
            _testOutputHelper.WriteLine(stored.IsDeleted.ToString());
            _testOutputHelper.WriteLine(stored.Id.ToString());

            var claims = _mockHttp.Object.HttpContext.User.Identity as ClaimsIdentity;
            Assert.Equal(users[0].Id.ToString(), claims.FindFirst("Id").Value);

            var response = _service.GetAvailableEnterprises();


            Assert.NotNull(response);

            _testOutputHelper.WriteLine(response.ToString());
            _testOutputHelper.WriteLine(response.ToList().ToString());
            _testOutputHelper.WriteLine(response.ToList()[0].ToString());

            Assert.Equal(enterprises, response.ToList());
        }
        [Fact]
        public async void GetEnterprisesNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(null, "Basic"));

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = _service.GetAvailableEnterprises();

            _testOutputHelper.WriteLine(response.Expression.ToString());
            _testOutputHelper.WriteLine(response.Count().ToString());
            Assert.Empty(response.ToList());
        }
        [Fact]
        public async void GetEnterprisesAdministatorNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
            httpContext.User = new ClaimsPrincipal(claims);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = _service.GetAvailableEnterprises();

            Assert.NotNull(response);
            Assert.Equal(enterprises, response.ToList());
        }
        [Fact]
        public async void GetEnterprisesNullEnterpriseDatabase()
        {
            _context.Setup(c => c.Enterprises).Returns<DbSet>(null);

            var response = Assert.Throws<NullReferenceException>(() => _service.GetAvailableEnterprises());

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }

        // GET USER 
        [Fact]
        public async void GetEnterpriseSuccessful()
        {
            var claims = _mockHttp.Object.HttpContext.User.Identity as ClaimsIdentity;

            Assert.Equal(users[0].Id.ToString(), claims.FindFirst("Id").Value);

            var response = await _service.GetAvailableEnterprise(enterprises[0].Id);

            Assert.NotNull(response);

            Assert.Equal(enterprises[0], response);
        }
        [Fact]
        public async void GetEnterpriseNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(null, "Basic"));

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetAvailableEnterprise(enterprises[0].Id));

            Assert.Equal("Empresa no encontrada", response.Message);
        }
        [Fact]
        public async void GetEnterpriseAdministatorNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
            httpContext.User = new ClaimsPrincipal(claims);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = await _service.GetAvailableEnterprise(enterprises[0].Id);

            Assert.NotNull(response);

            Assert.Equal(enterprises[0], response);
        }
        [Fact]
        public async void GetEnterpriseNotFound()
        {
            _testOutputHelper.WriteLine(_context.Object.Enterprises!.Count().ToString());

            _testOutputHelper.WriteLine((await _context.Object.Enterprises!.FindAsync(enterprises[0].Id))!.Id.ToString());

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetAvailableEnterprise(-1));

            Assert.Equal("Empresa no encontrada", response.Message);
        }
        [Fact]
        public async void GetEnterpriseNullEnterpriseDatabase()
        {
            _context.Setup(c => c.Enterprises).Returns<DbSet>(null);

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.GetAvailableEnterprise(0));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }

        // CREATE ENTERPRISE
        [Fact]
        public async void CreateEnterpriseSuccessful()
        {
            var newEnterprise = new EnterpriseEditable { Name = "New", UserId = 0 };

            var response = await _service.CreateEnterprise(newEnterprise);

            Assert.Equal(newEnterprise.Name, response.Name);
            Assert.Equal(newEnterprise.UserId, response.UserId);
        }
        [Fact]
        public async void CreateEnterpriseNullEnterpriseDatabase()
        {
            _context.Setup(c => c.Enterprises).Returns<DbSet>(null);
            var editData = new EnterpriseEditable { Name = "New", UserId = 0 };

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.CreateEnterprise(editData));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }
        [Fact]
        public async void CreateEnterpriseNullUserDatabase()
        {
            _context.Setup(c => c.Users).Returns<DbSet>(null);
            var editData = new EnterpriseEditable { Name = "New", UserId = 0 };

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.CreateEnterprise(editData));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }
        [Fact]
        public async void CreateEnterpriseNotEnoughData()
        {
            var newEnterprise = new EnterpriseEditable { Name = null,UserId = null };

            var response = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateEnterprise(newEnterprise));

            Assert.Equal("Faltan datos para generar la entidad", response.Message);
        }
        [Fact]
        public async void CreateEnterpriseNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(null, "Basic"));

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var newEnterprise = new EnterpriseEditable { Name = "New", UserId = -1 };

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.CreateEnterprise(newEnterprise));

            Assert.Equal("Id de usuario no encontrado", response.Message);
        }
        [Fact]
        public async void CreateEnterpriseAdministatorNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
            httpContext.User = new ClaimsPrincipal(claims);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var newEnterprise = new EnterpriseEditable { Name = "New", UserId = 0 };

            var response = await _service.CreateEnterprise(newEnterprise);

            Assert.Equal(newEnterprise.Name, response.Name);
            Assert.Equal(newEnterprise.UserId, response.UserId);
        }
        [Fact]
        public async void CreateEnterpriseUserNotFound()
        {
            var newEnterprise = new EnterpriseEditable { Name = "New", UserId = -1 };

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.CreateEnterprise(newEnterprise));

            Assert.Equal("Id de usuario no encontrado", response.Message);
        }

        // UPDATE ENTERPRISE
        [Fact]
        public async void UpdateEnterpriseSuccessful()
        {
            var editData = new EnterpriseEditable { Name = "New", UserId = 0 };

            var response = await _service.EditEnterprise(editData, enterprises[0].Id);

            Assert.Equal(editData.Name, response.Name);
            Assert.Equal(editData.UserId, response.UserId);
        }
        [Fact]
        public async void UpdateEnterpriseNullUserDatabase()
        {
            _context.Setup(c => c.Users).Returns<DbSet>(null);
            var editData = new EnterpriseEditable { Name = "New", UserId = 1 };

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.EditEnterprise(editData, -1));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }

        [Fact]
        public async void UpdateEnterpriseNotEnoughData()
        {
            var editData = new EnterpriseEditable { Name = null, UserId = null};

            var response = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.EditEnterprise(editData, enterprises[0].Id));

            Assert.Equal("No hay suficientes datos para modificar la entidad", response.Message);
        }
        [Fact]
        public async void UpdateEnterpriseUserNotFound()
        {
            var editData = new EnterpriseEditable { Name = "New", UserId = 1 };

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.EditEnterprise(editData, enterprises[0].Id));

            Assert.Equal("Usuario no encontrado", response.Message);
        }

        [Fact]
        public async void UpdateEnterpriseNotFound()
        {
            var editData = new EnterpriseEditable { Name = "New", UserId = 1 };

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.EditEnterprise(editData, -1));

            Assert.Equal("Empresa no encontrada", response.Message);
        }

        // DELETE ENTERPRISE
        [Fact]
        public async void DeleteEnterpriseSuccessful()
        {
            var response = await _service.DeleteEnterprise(enterprises[0].Id);

            Assert.Equal(enterprises[0].Name, response.Name);
            Assert.Equal(enterprises[0].UserId, response.UserId);

            Assert.Equal(httpUser, response.DeletedBy);
            Assert.Equal(httpUser, invoices[0].DeletedBy);
            Assert.True(response.IsDeleted);
            Assert.True(invoices[0].IsDeleted);
        }
        [Fact]
        public async void DeleteEnterpriseNullInvoiceDatabase()
        {
            _context.Setup(c => c.Invoices).Returns<DbSet>(null);

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.DeleteEnterprise(0));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }
        [Fact]
        public async void DeleteEnterpriseNullInvoiceLineDatabase()
        {
            _context.Setup(c => c.InvoiceLines).Returns<DbSet>(null);
            var editData = new EnterpriseEditable { Name = "New", UserId = 1 };

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.DeleteEnterprise(0));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }
        [Fact]
        public async void DeleteEnterpriseNotFounded()
        {
            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.DeleteEnterprise(-1));

            Assert.Equal("Empresa no encontrada", response.Message);
        }

        // BASE CHECKS
        [Fact]
        public void GetClaimsNull()
        {
            _mockHttp.SetupGet(_ => _.HttpContext.User).Returns((ClaimsPrincipal)null);

            Assert.Null(_mockHttp.Object.HttpContext.User);

            var response = Assert.Throws<BadHttpRequestException>(() => _service.getUserClaims());

            Assert.Equal("Datos de usuario que realiza la solicitud no encontrados", response.Message);
        }
    }
}
