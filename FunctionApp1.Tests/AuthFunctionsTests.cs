using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using FunctionApp1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using FunctionApp1.Models;
using Microsoft.EntityFrameworkCore;

namespace FunctionApp1.Tests
{
    public class AuthFunctionsTests
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public AuthFunctionsTests()
        {
            _loggerMock = new Mock<ILogger>();
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDbForTesting")
                .EnableSensitiveDataLogging() // Habilita el registro de datos sensibles
                .Options;
        }

        [Fact]
        public async Task Login_ReturnsOkObjectResult_WithValidCredentials()
        {
            
            var context = new ApplicationDbContext(_dbContextOptions);
            context.Usuarios.Add(new Usuario
            {
                Email = "test@example.com",
                HashContraseña = "hashedPassword",
                Sal = "salt",
                Nombre = "Test User" 
            });
            context.SaveChanges();

            var authFunctions = new AuthFunctions(context);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{\"Email\": \"test@example.com\", \"HashContraseña\": \"hashedPassword\"}"));
            httpContext.Request.ContentType = "application/json";

           
            var result = await authFunctions.Login(httpContext.Request, _loggerMock.Object);

           
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsOkObjectResult_WithValidData()
        {
            
            var context = new ApplicationDbContext(_dbContextOptions);
            var authFunctions = new AuthFunctions(context);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{\"Nombre\": \"Test User\", \"Email\": \"test@example.com\", \"HashContraseña\": \"hashedPassword\"}"));
            httpContext.Request.ContentType = "application/json";

            
            var result = await authFunctions.Register(httpContext.Request, _loggerMock.Object);

          
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}