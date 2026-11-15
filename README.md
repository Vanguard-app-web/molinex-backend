# Molinex Backend

Backend de Molinex para el curso de Aplicaciones Web. El proyecto usa C# y ASP.NET Core con el SDK de .NET 10.

Abre `Molinex.Platform.csproj` en Rider o ejecuta este comando desde la raíz del repositorio.

```powershell
dotnet run --urls http://localhost:5080
```

Consulta `GET http://localhost:5080/health` para comprobar que la aplicación está activa. La respuesta esperada es `{"status":"ok"}`.
