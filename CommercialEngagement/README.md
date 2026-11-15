# Commercial Engagement

Responsable: Brayan (`brayanjk22`, `u20241e550@upc.edu.pe`). Rama: `feature/commercial-engagement`.

Este módulo permite registrar solicitudes comerciales desde la landing page y consultarlas mediante autenticación. Sus 18 archivos de dominio, aplicación, persistencia y API conservan el contenido de `molinex-backend (1).zip`. Se añaden el registro de dependencias, esta guía y una comprobación HTTP reproducible.

## Reglas y API

- Nombre, empresa y mensaje: obligatorios, sin espacios externos, máximo 2048 caracteres.
- Correo: dirección válida, máximo 254 caracteres.
- Identificador generado por MySQL y serializado como texto; fecha de envío UTC asignada por el servidor.
- `POST /api/v1/commercial-inquiries`: público; devuelve `201`, recurso y cabecera `Location`; entrada inválida devuelve `400`.
- `GET /api/v1/commercial-inquiries/{id}`: requiere token IAM; devuelve `200`, `401` sin autenticación o `404` si no existe.
- Códigos estables: `Commercial.InquiryNotFound`, `Commercial.InvalidFullName`, `Commercial.InvalidCompanyName`, `Commercial.InvalidEmail` y `Commercial.InvalidMessage`.

El agregado emite `CommercialInquirySubmittedEvent`. El servicio de comando añade la entidad al repositorio; el pipeline transaccional compartido realiza el commit y despacha los eventos. El servicio no llama a `SaveChanges` por separado. La consulta de persistencia utiliza `AsNoTracking`.

## Integración a cargo del responsable de infraestructura

La base de esta feature es `develop` en `fdd52f9`. Ese estado ya carecía de `Shared`, recursos, imports globales, paquetes y configuración completa del host; por ello su build fallaba antes de añadir Commercial Engagement. Esta entrega no reemplaza archivos de otros integrantes.

Para integrar con la infraestructura común del ZIP:

1. Incorporar `Shared`, `GlobalUsings.cs`, dependencias de Cortex Mediator y EF Core/MySQL, recursos y configuración común del host.
2. En `Program.cs`, importar `Molinex.Platform.CommercialEngagement.Infrastructure.Configuration` y registrar `builder.Services.AddCommercialEngagement();`. Este método sustituye los tres registros individuales del repositorio y servicios comerciales.
3. En el `AppDbContext` compartido, importar la extensión de `CommercialEngagement.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions` y aplicar `modelBuilder.ApplyCommercialEngagementConfiguration();`. Mantener las convenciones comunes de nombres, auditoría y conversión de IDs.
4. Mantener `TimeProvider.System`, el mediador con escaneo de este ensamblado, los comportamientos de comandos, unidad de trabajo y despacho de eventos después de persistir. Registrar controladores, autenticación IAM y la fábrica común de Problem Details.
5. Incorporar en los recursos compartidos las cinco claves comerciales indicadas arriba, con textos en inglés y español, y activar la localización mediante `Accept-Language`.
6. Aplicar las migraciones de infraestructura compartida en una base de desarrollo. La tabla comercial se llama `commercial_inquiries`. No duplicar la creación de tablas ni alterar manualmente el historial de EF.

La feature no incluye migraciones, credenciales ni cambios de IAM, `Program.cs`, proyecto o infraestructura compartida: su integración corresponde a Fabricio. No contiene merge a `develop`.

## Verificación

Se validó en una carpeta temporal usando el host e infraestructura del ZIP y superponiendo los cinco módulos presentes en Git, incluido este módulo y su nuevo registro de dependencias:

- Build Release: 0 errores y 0 advertencias.
- 9 pruebas unitarias: validación, límites, evento y transformación de recursos.
- 17 comprobaciones HTTP contra una instancia MySQL aislada: creación anónima, persistencia, autenticación, recurso ausente, límites y errores localizados en inglés/español.

Estas comprobaciones validan el módulo con infraestructura integrada; no certifican que la base incompleta de `develop` compile por sí sola. Las pruebas unitarias de revisión se ejecutaron fuera del repositorio. El script HTTP queda versionado para repetir las comprobaciones tras la integración.

Con PowerShell 7, API integrada en ejecución y un token IAM válido en `$token`:

```powershell
.\CommercialEngagement\Verification\Test-CommercialEngagement.ps1 -BaseUrl 'http://localhost:5081' -Token $token
```

El script crea solicitudes sintéticas en la base configurada por la API; usar una base de pruebas. No muestra el token. Si alguna comprobación falla, finaliza con error.
