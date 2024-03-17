using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

public static class GoogleAuthFunctions
{
    [FunctionName("GoogleLogin")]
    public static IActionResult GoogleLogin(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "google-login")] HttpRequest req,
        ILogger log)
    {
        string redirectUrl = $"{req.Scheme}://{req.Host}/api/google-auth-callback";
        string clientId = Environment.GetEnvironmentVariable("GoogleClientId");
        string responseType = "code";
        string scope = "email";
        string authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&response_type={responseType}&scope={scope}&redirect_uri={redirectUrl}";

        log.LogInformation("Redirigiendo al login de Google.");
        return new RedirectResult(authUrl);
    }

    [FunctionName("GoogleAuthCallback")]
    public static async Task<IActionResult> GoogleAuthCallback(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "google-auth-callback")] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Manejando el callback de Google.");

        string code = req.Query["code"];
        if (string.IsNullOrEmpty(code))
        {
            return new BadRequestObjectResult("Código no proporcionado.");
        }

        
        string userId = "SimulatedGoogleUserId";

        log.LogInformation($"Proceso de autenticación de Google completado para el usuario: {userId}.");
        return new OkObjectResult($"Usuario autenticado con Google: {userId}");
    }
}