using FunctionApp1.Data;
using FunctionApp1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Linq;
using System.Security.Cryptography;

public class AuthFunctions
{
    private readonly ApplicationDbContext _context;

    public AuthFunctions(ApplicationDbContext context)
    {
        _context = context;
    }

    [FunctionName("Login")]
    public async Task<IActionResult> Login(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Proceso de login iniciado.");
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        Usuario loginRequest = JsonConvert.DeserializeObject<Usuario>(requestBody);
        if (loginRequest == null)
        {
            log.LogWarning("El cuerpo de la solicitud no pudo ser deserializado a un objeto Usuario.");
            return new BadRequestObjectResult(new { message = "El formato de la solicitud es incorrecto." });
        }

        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == loginRequest.Email);
        if (usuario == null)
        {
            log.LogWarning("Login fallido: Usuario no encontrado con el email {Email}.", loginRequest.Email);
            return new BadRequestObjectResult(new { message = "Usuario no encontrado." });
        }

        var passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: loginRequest.HashContraseña,
            salt: Convert.FromBase64String(usuario.Sal),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        log.LogInformation("Verificación de contraseña iniciada para el usuario {Email}.", loginRequest.Email);
        log.LogInformation("Salt utilizado: {Salt}.", usuario.Sal);

        if (usuario.HashContraseña != passwordHash)

        {
            log.LogWarning("Login fallido: Contraseña incorrecta para el usuario {Email}.", loginRequest.Email);
            log.LogInformation("Hash de contraseña esperado: {ExpectedHash}", usuario.HashContraseña);
            log.LogInformation("Hash de contraseña proporcionado: {ProvidedHash}", passwordHash);
            log.LogWarning("Login fallido: Contraseña incorrecta para el usuario {Email}.", loginRequest.Email);
            return new BadRequestObjectResult(new { message = "Contraseña incorrecta." });
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_Secret"));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = Environment.GetEnvironmentVariable("JWT_Issuer"),
            Audience = Environment.GetEnvironmentVariable("JWT_Audience")
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        log.LogInformation("Login exitoso para el usuario {Email}.", loginRequest.Email);
        return new OkObjectResult(new { Token = tokenString });
    }

    [FunctionName("Register")]
    public async Task<IActionResult> Register(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Proceso de registro iniciado.");
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        Usuario registerRequest = JsonConvert.DeserializeObject<Usuario>(requestBody);
        if (registerRequest == null)
        {
            log.LogWarning("El cuerpo de la solicitud no pudo ser deserializado a un objeto Usuario.");
            return new BadRequestObjectResult(new { message = "El formato de la solicitud es incorrecto." });
        }

        var existingUser = _context.Usuarios.Any(u => u.Email == registerRequest.Email);
        if (existingUser)
        {
            log.LogWarning("Registro fallido: El usuario con el email {Email} ya existe.", registerRequest.Email);
            return new BadRequestObjectResult(new { message = "El usuario ya existe." });
        }

        string salt = GenerateSalt();
        string hashedPassword = HashPassword(registerRequest.HashContraseña, salt);

        var usuario = new Usuario
        {
            Nombre = registerRequest.Nombre,
            Email = registerRequest.Email,
            HashContraseña = hashedPassword,
            Sal = salt
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        log.LogInformation("Usuario registrado con éxito con el email {Email}.", registerRequest.Email);
        return new OkObjectResult(new { message = "Usuario registrado con éxito." });
    }

    private static string GenerateSalt()
    {
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return Convert.ToBase64String(salt);
    }

    private static string HashPassword(string password, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
        return hashed;
    }
}

