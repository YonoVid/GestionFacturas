using APIGestionFacturas.DataAccess;
using APIGestionFacturas.Services;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit.Abstractions;

namespace APIGestionFacturas.Tests.Services
{
    public class UserServiceTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly UserService _service;

        private List<User> users;
        private UserEditable editable;
        private UserAuthorization authorization;
        private string httpUser;

        private Mock<GestionFacturasContext> _context;
        private Mock<IHttpContextAccessor> _mockHttp;

        public UserServiceTest(ITestOutputHelper testOutputHelper) : base()
        {
            users = MockDbHelper.GetUserList();

            editable = new UserEditable
            {
                Name = users[0].Name,
                Email = users[0].Email,
                Password = users[0].Password,
                Rol = users[0].Rol
            };

            authorization = new UserAuthorization
            {
                Name = users[0].Name,
                Email = users[0].Email,
                Password = users[0].Password
            };

            httpUser = "Test identity";

            _context = new Mock<GestionFacturasContext>();

            var dbMock = MockDbContext.GetQueryableMockDbSet<User>(users);

            _context.SetupGet(p => p.Users).Returns(dbMock);
            _context.Setup(p => p.SetModified(It.IsAny<User>()));
            _context.Setup(p => p.SaveChanges()).Returns(1);
            _context.Setup(p => p.SaveChangesAsync(default)).Returns(Task.FromResult(1));


            _mockHttp = new();
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(null, "Basic"));
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Name, httpUser));
            httpContext.User.AddIdentity(identity);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            _service = new UserService(_context.Object, _mockHttp.Object);

            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async void GetUsersSuccessful()
        {
            _testOutputHelper.WriteLine(_context.Object.Users!.Count().ToString());

            var response = await _service.GetUsers();

            Assert.NotNull(response);

            Assert.Equal(users, response);
        }

        // GET USER 
        [Fact]
        public async void GetUserSuccessful()
        {
            _testOutputHelper.WriteLine(_context.Object.Users!.Count().ToString());

            _testOutputHelper.WriteLine((await _context.Object.Users!.FindAsync(users[0].Id))!.Id.ToString());

            var response = await _service.GetUser(users[0].Id);

            Assert.NotNull(response);

            Assert.Equal(users[0], response);
        }

        [Fact]
        public async void GetUserNotFound()
        {
            _testOutputHelper.WriteLine(_context.Object.Users!.Count().ToString());

            _testOutputHelper.WriteLine((await _context.Object.Users!.FindAsync(users[0].Id))!.Id.ToString());

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async ()=> await _service.GetUser(-1));

            Assert.Equal("Usuario no encontrado", response.Message);
        }

        // USER LOGIN
        [Fact]
        public async void GetUserLoginSuccessful()
        {
            _testOutputHelper.WriteLine(_context.Object.Users!.Count().ToString());

            _testOutputHelper.WriteLine((await _context.Object.Users!.FindAsync(users[0].Id))!.Id.ToString());

            var response = _service.GetUserLogin(authorization);

            Assert.Equal(users[0], response);
        }
        [Fact]
        public async void GetUserLoginNotFound()
        {
            _testOutputHelper.WriteLine(_context.Object.Users!.Count().ToString());

            _testOutputHelper.WriteLine((await _context.Object.Users.FindAsync(users[0].Id)).Id.ToString());

            var loginData = new UserAuthorization { Name = "", Email = "", Password = "" };

            var response = _service.GetUserLogin(loginData);

            Assert.Equal(null, response);
        }

        // USER REGISTER
        [Fact]
        public async void RegisterUserSuccessful()
        {
            var loginData = new UserAuthorization { Name = "New", Email = "new@mail.com", Password = "new" };

            var response = await _service.RegisterUser(loginData);

            Assert.Equal(loginData.Name, response.Name);
            Assert.Equal(loginData.Email, response.Email);
            Assert.Equal(loginData.Password, response.Password);
        }
        [Fact]
        public async void RegisterUserNotEnoughData()
        {
            var loginData = new UserAuthorization { Name = null, Email = String.Empty, Password = String.Empty };

            var response = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.RegisterUser(loginData));

            Assert.Equal("No hay suficientes datos para crear la entidad", response.Message);
        }
        [Fact]
        public async void RegisterUserAlreadyExists()
        {
            var response = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.RegisterUser(authorization));

            Assert.Equal("Usuario con el mismo correo ya existe", response.Message);
        }

        // CREATE USER
        [Fact]
        public async void CreateUserSuccessful()
        {
            var newUser = new UserEditable { Name = "New", Email = "new@mail.com", Password = "new", Rol = UserRol.USER };

            var response = await _service.CreateUser(newUser);

            Assert.Equal(newUser.Name, response.Name);
            Assert.Equal(newUser.Email, response.Email);
            Assert.Equal(newUser.Password, response.Password);
            Assert.Equal(newUser.Rol, response.Rol);
        }
        [Fact]
        public async void CreateUserNotEnoughData()
        {
            var newUser = new UserEditable { Name = null, Email = null, Password = null, Rol = null };

            var response = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateUser(newUser));

            Assert.Equal("No hay suficientes datos para crear la entidad", response.Message);
        }
        [Fact]
        public async void CreateUserAlreadyExists()
        {
            var newUser = new UserEditable { Name = editable.Name, Email = editable.Email, Password = editable.Password, Rol = editable.Rol };

            var response = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateUser(newUser));

            Assert.Equal("Usuario con el mismo correo ya existe", response.Message);
        }

        // UPDATE USER
        [Fact]
        public async void UpdateUserSuccessful()
        {
            var editData = new UserEditable { Name = "New", Email = "new@mail.com", Password = "new", Rol = UserRol.USER };

            var response = await _service.EditUser(editData, users[0].Id);

            Assert.Equal(editData.Name, response.Name);
            Assert.Equal(editData.Email, response.Email);
            Assert.Equal(editData.Password, response.Password);
            Assert.Equal(editData.Rol, response.Rol);
        }
        [Fact]
        public async void UpdateUserNotEnoughData()
        {
            var editData = new UserEditable { Name = null, Email = null, Password = null, Rol = null };

            var response = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.EditUser(editData, users[0].Id));

            Assert.Equal("No hay suficientes datos para modificar la entidad", response.Message);
        }
        [Fact]
        public async void UpdateUserNotFounded()
        {
            var editData = new UserEditable { Name = "New", Email = "new@mail.com", Password = "new", Rol = UserRol.USER };

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.EditUser(editData, -1));

            Assert.Equal("Usuario no encontrado", response.Message);
        }

        // DELETE USER
        [Fact]
        public async void DeleteUserSuccessful()
        {
            var response = await _service.DeleteUser(users[0].Id);

            Assert.Equal(users[0].Name, response.Name);
            Assert.Equal(users[0].Email, response.Email);
            Assert.Equal(users[0].Password, response.Password);
            Assert.Equal(users[0].Rol, response.Rol);

            Assert.Equal(true, response.IsDeleted);
        }
        [Fact]
        public async void DeleteUserNotFounded()
        {
            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.DeleteUser(-1));

            Assert.Equal("Usuario no encontrado", response.Message);
        }

        // BASE CHECKS
        [Fact]
        public void CheckBaseDatabaseNull()
        {
            _context.SetupGet(_ => _.Users).Returns((DbSet<User>)null);

            Assert.Null(_context.Object.Users);

            var response = Assert.Throws<NullReferenceException>(() => _service.CheckBaseExpectations());

            Assert.Equal("Referencia a base de datos es nula", response.Message);
        }

        [Fact]
        public void CheckBaseClaimsNull()
        {
            _mockHttp.SetupGet(_ => _.HttpContext.User).Returns((ClaimsPrincipal)null);

            Assert.Null(_mockHttp.Object.HttpContext.User);

            var response = Assert.Throws<BadHttpRequestException>(() => _service.CheckBaseExpectations());

            Assert.Equal("Datos de usuario que realiza la solicitud no encontrados", response.Message);
        }
    }
}
