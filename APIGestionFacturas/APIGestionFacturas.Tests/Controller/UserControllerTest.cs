using APIGestionFacturas.Controllers;
using APIGestionFacturas.Services;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit.Abstractions;

namespace APIGestionFacturas.Tests.Controller
{
    public class UserControllerTest : BaseControllerTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly UsersController _controller;

        private readonly Mock<IUserService> _userServiceMock;

        private int id = 0;
        private User user;
        private UserEditable editable;
        private UserAuthorization authorization;

        private JwtSettings _jwtSettings;

        public UserControllerTest(ITestOutputHelper testOutputHelper) : base()
        {
            id = 0;
            user = new User
            {
                Id = id,
                Name = "Example user",
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };

            _jwtSettings = new JwtSettings
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = "00000000-0000-0000-0000-000000000000",
                ValidateIssuer = true,
                ValidIssuer = "https://localhost:7255",
                ValidateAudience = true,
                ValidAudience = "https://localhost:7255",
                RequireExpirationTime = true,
                ValidateLifetime = true
            };

            editable = new UserEditable();

            authorization = new UserAuthorization();

            _userServiceMock = new();

            _controller = new UsersController(_userServiceMock.Object, _jwtSettings);

            _testOutputHelper = testOutputHelper;
        }

        //LOGIN USER
        [Fact]
        public async void LoginUserSuccessful()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.UserExists(authorization))
                .Returns(true);
            _userServiceMock.Setup(userService => userService.GetUserLogin(authorization))
                .Returns(user);

            var response = _controller.Login(authorization);

            Assert.NotNull(response);

            var userToken = (ResponseToken)((response as ObjectResult)?.Value);

            Assert.Equal(user.Name, userToken.Token.UserName);
            Assert.Equal(user.Email, userToken.Token.EmailId);
            Assert.Equal(user.Rol, userToken.Token.UserRol);
        }
        [Fact]
        public void LoginUserNotFound()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.UserExists(authorization))
                .Returns(true);
            _userServiceMock.Setup(userService => userService.GetUserLogin(authorization))
                .Returns((User?)null);

            var response = _controller.Login(authorization);

            Assert.NotNull(response);

            Assert.IsType<BadRequestObjectResult>(response);
        }


        [Fact]
        public async void LoginUserWrongPassword()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.UserExists(authorization))
                .Returns(false);

            var response = _controller.Login(authorization);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async void LoginUserInvalidError()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.UserExists(authorization))
                .Throws(new InvalidOperationException());

            var response = _controller.Login(authorization);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async void LoginUserDatabaseError()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.UserExists(authorization))
                .Throws(new NullReferenceException());

            var response = _controller.Login(authorization);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response);
            Assert.Equal(500, (response as ObjectResult)?.StatusCode);
        }

        //REGISTER USER
        [Fact]
        public async void RegisterUserSuccessful()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.RegisterUser(authorization))
                .ReturnsAsync(user);

            var response = await _controller.Register(authorization);

            Assert.NotNull(response);

            Assert.Equal(user, (response.Result as ObjectResult)?.Value);
        }

        [Fact]
        public async void RegisterUserInvalidError()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.RegisterUser(authorization))
                .Throws(new InvalidOperationException());

            var response = await _controller.Register(authorization);

            Assert.NotNull(response);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async void RegisterUserDatabaseError()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.RegisterUser(authorization))
                .Throws(new NullReferenceException());

            var response = await _controller.Register(authorization);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response.Result);
            Assert.Equal(500, (response.Result as ObjectResult)?.StatusCode);
        }

        /*
         * -----------------------
         *  CRUD OF THE ENTITIES
         * -----------------------
         */

        // GET USER
        [Fact]
        public async void GetUserSuccessful()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.GetUser(id))
                .ReturnsAsync(user);

            var response = await _controller.GetUser(id);

            Assert.NotNull(response);

            Assert.Equal(user, response.Value);
        }

        [Fact]
        public async void GetUserNotFounded()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.GetUser(id))
                .Throws(new KeyNotFoundException());

            var response = await _controller.GetUser(id);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async void GetUserDatabaseError()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.GetUser(id))
                .Throws(new NullReferenceException());

            var response = await _controller.GetUser(id);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response.Result);
            Assert.Equal(500, (response.Result as ObjectResult).StatusCode);
        }
        /// GET USERS
        [Fact]
        public async void GetUsersSuccessful()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.GetUsers())
                .ReturnsAsync(new List<User>());

            var response = await _controller.GetUsers();

            _testOutputHelper.WriteLine(response.Value.ToString());

            Assert.NotNull(response);

            Assert.Equal(new List<User>(), response.Value);
        }

        [Fact]
        public async void GetUsersDatabaseError()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.GetUsers())
                .Throws(new NullReferenceException());

            var response = await _controller.GetUsers();

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response.Result);
            Assert.Equal(500, (response.Result as ObjectResult).StatusCode);
        }
        //POST USER
        [Fact]
        public async void PostUserSuccessful()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.CreateUser(editable))
                .ReturnsAsync(user);

            var response = await _controller.PostUser(editable);

            Assert.NotNull(response);

            Assert.Equal(user, (response.Result as ObjectResult)?.Value);
        }

        [Fact]
        public async void PostUserInvalidError()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.CreateUser(editable))
                .Throws(new InvalidOperationException());

            var response = await _controller.PostUser(editable);

            Assert.NotNull(response);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async void PostUserDatabaseError()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.CreateUser(editable))
                .Throws(new NullReferenceException());

            var response = await _controller.PostUser(editable);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response.Result);
            Assert.Equal(500, (response.Result as ObjectResult)?.StatusCode);
        }

        //PUT USER
        [Fact]
        public async void PutUserSuccessful()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.EditUser(editable, id))
                .ReturnsAsync(user);

            var response = await _controller.PutUser(id, editable);

            Assert.NotNull(response);

            Assert.Equal(user, (response as ObjectResult)?.Value);
        }

        [Fact]
        public async void PutUserNotFound()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.EditUser(editable, id))
                .Throws(new KeyNotFoundException());

            var response = await _controller.PutUser(id, editable);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async void PutUserInvalidError()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.EditUser(editable, id))
                .Throws(new InvalidOperationException());

            var response = await _controller.PutUser(id, editable);

            Assert.NotNull(response);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async void PutUserDatabaseError()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.EditUser(editable, id))
                .Throws(new NullReferenceException());

            var response = await _controller.PutUser(id, editable);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response);
            Assert.Equal(500, (response as ObjectResult)?.StatusCode);
        }

        // DELETE USER
        [Fact]
        public async void DeleteUserSuccessful()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.DeleteUser(id))
                .ReturnsAsync(user);

            var response = await _controller.DeleteUser(id);

            Assert.NotNull(response);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async void DeleteUserNotFounded()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.DeleteUser(id))
                .Throws(new KeyNotFoundException());

            var response = await _controller.DeleteUser(id);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async void DeleteUserDatabaseError()
        {
            AddAdminToken();

            _userServiceMock.Setup(userService => userService.DeleteUser(id))
                .Throws(new NullReferenceException());

            var response = await _controller.DeleteUser(id);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response);
            Assert.Equal(500, (response as ObjectResult)?.StatusCode);
        }

    }
}