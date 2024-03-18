# retotecniconet
Reto tecnico .Net



ESPAÑOL: 


Para ejecutar y probar el proyecto de Azure Functions con autenticación, incluyendo la integración con Google OAuth, sigue esta guía detallada:

Cómo Ejecutar el Proyecto

1. Dependencias: Asegúrate de tener instalado .NET 6 SDK y Azure Functions Core Tools. El proyecto ya incluye las dependencias necesarias en el archivo FunctionApp1.csproj, como Entity Framework Core, Azure Functions SDK y paquetes relacionados con la autenticación.

2. Ejecución Local:
Abre una terminal o CMD y navega al directorio del proyecto.
Ejecuta func start para iniciar el proyecto localmente.
Asegúrate de tener configuradas las variables de entorno necesarias, como la cadena de conexión de la base de datos (DbConnectionString) y las claves para JWT (JWT_Secret, JWT_Issuer, JWT_Audience) en tu archivo local.settings.json.

Rutas de las APIs

Login: POST /api/Login

Registro: POST /api/Register

Login con Google: GET /api/google-login

Callback de Google: GET /api/google-auth-callback

JSON para Probar en Postman
Login:
  {
    "Email": "test@example.com",
    "HashContraseña": "hashedPassword"
  }


  Registro:

    {
    "Nombre": "Test User",
    "Email": "test@example.com",
    "HashContraseña": "hashedPassword"
  }

validacion coneccino a la base de datos :

GET : http://localhost:7071/api/TestDatabaseConnection

------------------------------------------------------------
credenciales de la base de datos : azure sql

host : serverretonet.database.windows.net
puerto : 1433
database : dbretonet
user : adminretonet
clave : Cirilo159.

------------------------------------------------
script tabla usada :

-- dbretonet.dbo.Usuarios definition

-- Drop table

-- DROP TABLE dbretonet.dbo.Usuarios;

CREATE TABLE dbretonet.dbo.Usuarios (
	Id int NOT NULL,
	Nombre nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Email nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	HashContraseña nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Sal nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK__Usuarios__3214EC0790A0B14B PRIMARY KEY (Id),
	CONSTRAINT UQ__Usuarios__531402F30A1542F4 UNIQUE (Email)
);

-------------------------------------------------------

  Configuración de la Autenticación con Google

1. Google Cloud Platform:

Crea un proyecto en Google Cloud Platform.
Ve a "Credenciales" y crea un ID de cliente OAuth 2.0.
Configura las URI de redireccionamiento autorizadas para incluir tu endpoint de callback (por ejemplo, http://localhost:7071/api/google-auth-callback para desarrollo local).

2. Variables de Entorno:
Añade GoogleClientId y GoogleClientSecret a tu local.settings.json para desarrollo local y configúralas en la plataforma de Azure para producción.

Pruebas Unitarias
El archivo AuthFunctionsTests.cs incluye pruebas unitarias para las funcionalidades de login y registro. Para ejecutar estas pruebas:

1. Asegúrate de tener instalado xUnit y el SDK de .NET.
2. Navega al directorio del proyecto de pruebas (FunctionApp1.Tests) en una terminal.
3. Ejecuta dotnet test para correr las pruebas unitarias.

Estas pruebas cubren la creación de usuarios y el proceso de login, validando que los endpoints funcionen como se espera.

Notas Adicionales
Para la autenticación con Google, el flujo inicia con el usuario haciendo clic en un enlace que lo redirige a Google para el login. Después del login, Google redirige al usuario de vuelta a tu aplicación usando el endpoint de callback, donde puedes manejar la respuesta y crear un token JWT para el usuario si es necesario.

------------------------------------------------------------------------------------------------------------


INGLES :


To run and test the Azure Functions project with authentication, including integration with Google OAuth, follow this detailed guide:

**How to Run the Project**

1. Dependencies: Make sure you have .NET 6 SDK and Azure Functions Core Tools installed. The project already includes the necessary dependencies in the FunctionApp1.csproj file, such as Entity Framework Core, Azure Functions SDK, and packages related to authentication.

2. Local Execution:
   Open a terminal or CMD and navigate to the project directory.
   Execute `func start` to start the project locally.
   Ensure you have the necessary environment variables configured, such as the database connection string (DbConnectionString) and the keys for JWT (JWT_Secret, JWT_Issuer, JWT_Audience) in your local.settings.json file.
   
**API Paths**
- Login: POST /api/Login
- Registration: POST /api/Register
- Login with Google: GET /api/google-login
- Google Callback: GET /api/google-auth-callback

**JSON for Testing in Postman**
- Login:
  ```json
  {
    "Email": "test@example.com",
    "HashPassword": "hashedPassword"
  }
  ```
- Registration:
  ```json
  {
    "Name": "Test User",
    "Email": "test@example.com",
    "HashPassword": "hashedPassword"
  }
  ```

**Database Connection Validation:**

GET: http://localhost:7071/api/TestDatabaseConnection

------------------------------------------------------------
**Database Credentials: Azure SQL**

- Host: serverretonet.database.windows.net
- Port: 1433
- Database: dbretonet
- User: adminretonet
- Password: Cirilo159.

------------------------------------------------
**Script for Table Used:**

```sql
-- dbretonet.dbo.Users definition

-- Drop table
-- DROP TABLE dbretonet.dbo.Usuarios;

CREATE TABLE dbretonet.dbo.Usuarios (
	Id int NOT NULL,
	Nombre nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Email nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	HashContraseña nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Sal nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK__Usuarios__3214EC0790A0B14B PRIMARY KEY (Id),
	CONSTRAINT UQ__Usuarios__531402F30A1542F4 UNIQUE (Email)
);
```

-------------------------------------------------------

**Configuring Authentication with Google**

1. Google Cloud Platform:

- Create a project in Google Cloud Platform.
- Go to "Credentials" and create an OAuth 2.0 Client ID.
- Configure the authorized redirect URIs to include your callback endpoint (e.g., http://localhost:7071/api/google-auth-callback for local development).

2. Environment Variables:
   Add GoogleClientId and GoogleClientSecret to your local.settings.json for local development and configure them on the Azure platform for production.

**Unit Tests**
The AuthFunctionsTests.cs file includes unit tests for login and registration functionalities. To run these tests:

1. Make sure you have xUnit and the .NET SDK installed.
2. Navigate to the project directory of the tests (FunctionApp1.Tests) in a terminal.
3. Execute `dotnet test` to run the unit tests.

These tests cover user creation and the login process, validating that the endpoints work as expected.

**Additional Notes**
For authentication with Google, the flow starts with the user clicking a link that redirects them to Google for login. After logging in, Google redirects the user back to your application using the callback endpoint, where you can handle the response and create a JWT token for the user if necessary.

