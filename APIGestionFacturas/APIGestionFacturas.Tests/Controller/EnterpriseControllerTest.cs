using APIGestionFacturas.Controllers;
using APIGestionFacturas.Services;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit.Abstractions;

namespace APIGestionFacturas.Tests.Controller
{
    public class EnterpriseControllerTest : BaseControllerTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly EnterpriseController _controller;

        private readonly Mock<IEnterpriseService> _enterpriseServiceMock;

        private int id = 0;
        private Enterprise enterprise;
        private EnterpriseEditable editable;

        public EnterpriseControllerTest(ITestOutputHelper testOutputHelper) : base()
        {
            id = 0;
            enterprise = new Enterprise
            {
                Id = id,
                Name = "Example enterprise",
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };

            editable = new EnterpriseEditable();

            _enterpriseServiceMock = new();

            _controller = new EnterpriseController(_enterpriseServiceMock.Object, new JwtSettings());

            _testOutputHelper = testOutputHelper;
        }

        /*
         * -----------------------
         *  CRUD OF THE ENTITIES
         * -----------------------
         */

        // GET ENTERPRISE
        [Fact]
        public async void GetEnterpriseSuccessful()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.GetAvailableEnterprise(id))
                .ReturnsAsync(enterprise);

            var response = await _controller.GetEnterprise(id);

            Assert.NotNull(response);

            Assert.Equal(enterprise, response.Value);
        }

        [Fact]
        public async void GetEnterpriseNotFounded()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.GetAvailableEnterprise(id))
                .Throws(new KeyNotFoundException());

            var response = await _controller.GetEnterprise(id);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async void GetEnterpriseDatabaseError()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.GetAvailableEnterprise(id))
                .Throws(new NullReferenceException());

            var response = await _controller.GetEnterprise(id);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response.Result);
            Assert.Equal(500, (response.Result as ObjectResult).StatusCode);
        }
        /// GET ENTERPRISES
        [Fact]
        public async void GetEnterprisesSuccessful()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.GetAvailableEnterprises())
                .Returns((new[] { enterprise }).AsQueryable());

            var response = await _controller.GetEnterprises();

            _testOutputHelper.WriteLine(response.Value.ToString());

            Assert.NotNull(response);

            Assert.Equal(new[] { enterprise }, response.Value);
        }

        [Fact]
        public async void GetEnterprisesDatabaseError()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.GetAvailableEnterprises())
                .Throws(new NullReferenceException());

            var response = await _controller.GetEnterprises();

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response.Result);
            Assert.Equal(500, (response.Result as ObjectResult).StatusCode);
        }
        //POST ENTERPRISE
        [Fact]
        public async void PostEnterpriseSuccessful()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.CreateEnterprise(editable))
                .ReturnsAsync(enterprise);

            var response = await _controller.PostEnterprise(editable);

            Assert.NotNull(response);

            Assert.Equal(enterprise, (response.Result as ObjectResult)?.Value);
        }

        [Fact]
        public async void PostEnterpriseNotFound()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.CreateEnterprise(editable))
                .Throws(new KeyNotFoundException());

            var response = await _controller.PostEnterprise(editable);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async void PostEnterpriseInvalidError()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.CreateEnterprise(editable))
                .Throws(new InvalidOperationException());

            var response = await _controller.PostEnterprise(editable);

            Assert.NotNull(response);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async void PostEnterpriseDatabaseError()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.CreateEnterprise(editable))
                .Throws(new NullReferenceException());

            var response = await _controller.PostEnterprise(editable);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response.Result);
            Assert.Equal(500, (response.Result as ObjectResult)?.StatusCode);
        }

        //PUT ENTERPRISE
        [Fact]
        public async void PutEnterpriseSuccessful()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.EditEnterprise(editable, id))
                .ReturnsAsync(enterprise);

            var response = await _controller.PutEnterprise(id, editable);

            Assert.NotNull(response);

            Assert.Equal(enterprise, (response as ObjectResult)?.Value);
        }

        [Fact]
        public async void PutEnterpriseNotFound()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.EditEnterprise(editable, id))
                .Throws(new KeyNotFoundException());

            var response = await _controller.PutEnterprise(id, editable);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async void PutEnterpriseInvalidError()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.EditEnterprise(editable, id))
                .Throws(new InvalidOperationException());

            var response = await _controller.PutEnterprise(id, editable);

            Assert.NotNull(response);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async void PutEnterpriseDatabaseError()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.EditEnterprise(editable, id))
                .Throws(new NullReferenceException());

            var response = await _controller.PutEnterprise(id, editable);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response);
            Assert.Equal(500, (response as ObjectResult)?.StatusCode);
        }

        // DELETE ENTERPRISE
        [Fact]
        public async void DeleteEnterpriseSuccessful()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.DeleteEnterprise(id))
                .ReturnsAsync(enterprise);

            var response = await _controller.DeleteEnterprise(id);

            Assert.NotNull(response);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async void DeleteEnterpriseNotFounded()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.DeleteEnterprise(id))
                .Throws(new KeyNotFoundException());

            var response = await _controller.DeleteEnterprise(id);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async void DeleteEnterpriseDatabaseError()
        {
            AddAdminToken();

            _enterpriseServiceMock.Setup(enterpriseService => enterpriseService.DeleteEnterprise(id))
                .Throws(new NullReferenceException());

            var response = await _controller.DeleteEnterprise(id);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response);
            Assert.Equal(500, (response as ObjectResult)?.StatusCode);
        }

    }
}