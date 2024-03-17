using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using FunctionApp1.Data;

namespace FunctionApp1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("TestDatabaseConnection")]
        public static async Task<IActionResult> TestDatabaseConnection(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("DbConnectionString", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(connectionString))
            {
                log.LogError("Database connection string is not set.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                using (var context = new ApplicationDbContext(optionsBuilder.Options))
                {
                    // Cambio para usar una operación asincrónica para verificar la conexión
                    var canConnect = await context.Database.CanConnectAsync();
                    if (!canConnect)
                    {
                        throw new Exception("Unable to connect to the database.");
                    }
                }

                return new OkObjectResult("Successfully connected to the database.");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to connect to the database.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}