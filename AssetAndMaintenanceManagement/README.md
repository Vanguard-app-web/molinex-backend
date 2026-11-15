# Asset and Maintenance Management — Antony

Responsable: `saavedraantony-max` (`saavedraantony2006@gmail.com`).
Este contexto incorpora `AssetAndMaintenanceManagement/` del ZIP: maquinaria, estado actual e historial de intervenciones. Los códigos se normalizan a mayúsculas y deben ser únicos. Los mantenimientos conservan su máquina de origen. El historial se ordena por fecha de intervención descendente y, en empates, por ID descendente.

## Capas

- `Domain/`: agregados `Machine` y `MaintenanceRecord`, comandos, consultas, eventos, errores, value objects y contratos de repositorios.
- `Application/`: registro de máquinas y mantenimientos, consultas, verificación de existencia y manejo de eventos.
- `Infrastructure/`: repositorios EF Core, mapeo relacional y registro de dependencias.
- `Interfaces/Rest/`: controladores protegidos, recursos y ensambladores.
- `Verification/`: prueba HTTP reproducible de TS-03.

`PREVENTIVE` no admite detalles de falla. `CORRECTIVE` exige falla, causa, acción y minutos de parada no negativos. Los estados válidos son `OPERATIONAL`, `REQUIRES_ATTENTION`, `UNDER_MAINTENANCE` y `OUT_OF_SERVICE`; la fecha del estado es obligatoria. Registrar un mantenimiento no cambia automáticamente el estado de la máquina.

## Integración con Fabricio

El `develop` recibido todavía no tiene `Shared/`, `Resources/`, `GlobalUsings.cs`, paquetes, autenticación ni el host completo. Sus módulos de Producción y Calidad tampoco compilan sin esa base. Esta entrega no reemplaza `Program.cs`, el `.csproj`, migraciones ni la configuración de otros responsables.

Cuando la base compartida esté integrada:

1. Conservar los paquetes del `.csproj` del ZIP y sus global usings. Mantenimiento usa EF Core y Cortex.Mediator, además de los contratos de `Shared`.
2. Registrar el contexto con estas líneas en el host, sin duplicar los cuatro registros individuales del ZIP:

   ```csharp
   using Molinex.Platform.AssetAndMaintenanceManagement.Infrastructure.Configuration;
   builder.Services.AddAssetAndMaintenanceManagement();
   ```

3. En `AppDbContext.OnModelCreating`, incluir `builder.ApplyAssetAndMaintenanceConfiguration()` con el namespace de `Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions` de este módulo. Mantener la configuración compartida y la convención snake_case del ZIP.
4. Conservar `AddControllers`, la convención kebab-case, `MapControllers`, `ProblemDetailsFactory`, localización, autenticación IAM y `UseRequestAuthorization` del host del ZIP. Los atributos de autorización de este módulo dependen de IAM (Brayan).
5. Registrar Cortex.Mediator con escaneo del ensamblado y el pipeline compartido `TransactionCommandBehavior<,>`: el handler agrega al repositorio y el pipeline confirma `IUnitOfWork`. No confirmar nuevamente dentro de los handlers.
6. Fabricio valida la migración inicial del modelo completo en MySQL. Las tablas son `machines` y `maintenance_records`, con código único y clave foránea restrictiva a la máquina. Las credenciales se proporcionan mediante el entorno o secretos locales.

## API y TS-03

Todas las rutas requieren `Authorization: Bearer <token>` emitido por IAM.

| Método | Ruta | Resultado |
| --- | --- | --- |
| POST | `/api/v1/machines` | Registra máquina: 201; código duplicado: 409 |
| GET | `/api/v1/machines` | Lista máquinas |
| GET | `/api/v1/machines/{id}` | Consulta máquina; inexistente: 404 |
| GET | `/api/v1/machines/{id}/status` | Consulta estado actual; inexistente: 404 |
| POST | `/api/v1/maintenance-records` | Registra intervención: 201; máquina inexistente: 404 |
| GET | `/api/v1/maintenance-records?machineId={id}` | Lista registros; filtro opcional |
| GET | `/api/v1/maintenance-records/{id}` | Consulta intervención; inexistente: 404 |
| GET | `/api/v1/maintenance-records/by-machine/{id}` | Historial; máquina inexistente: 404; máquina sin intervenciones: 200 con `[]` |

Ejemplo de máquina:

```json
{"code":"MOL-001","name":"Rice mill","model":"MX-100","status":"OPERATIONAL","statusChangedAt":"2026-01-01T08:00:00Z"}
```

Ejemplo de mantenimiento preventivo (reemplazar `machineId` por el ID recibido):

```json
{"machineId":"1","type":"PREVENTIVE","performedAt":"2026-01-02T10:00:00Z","description":"Lubricate bearings","technicianName":"Antony","technicianId":null,"anomalyId":null,"failure":null,"cause":null,"actionTaken":null,"downtimeMinutes":null}
```

TS-03: crear máquina → registrar mantenimiento → consultar historial y estado → consultar/registrar contra una máquina inexistente y comprobar 404. Los IDs se devuelven como strings para conservar precisión en JavaScript; el POST de mantenimiento también acepta un ID numérico.

Con el backend integrado en ejecución, usar PowerShell 7 desde la raíz del repositorio. Guardar en `$token` el token obtenido con `POST /api/v1/tokens` y ejecutar:

```powershell
./AssetAndMaintenanceManagement/Verification/Test-TS03.ps1 -BaseUri 'http://localhost:5080' -Token $token -ReportPath './ts03-report.json'
```

La prueba crea dos máquinas con códigos únicos y tres intervenciones; puede repetirse sin colisiones. Verifica autorización, códigos normalizados/duplicados, estado, historial vacío, orden por fecha distinto al orden de inserción, aislamiento por máquina, mantenimiento correctivo, validaciones y casos inexistentes. Produce un informe opcional sin tokens. Ejecutarla en una base de desarrollo: los datos de prueba permanecen para inspección.

La validación aislada usa las fuentes de este módulo y de Producción/Calidad del checkout, con infraestructura e IAM del ZIP. En esa copia temporal se cambia el proveedor del host a SQLite y `Migrate` a `EnsureCreated`, y se registra una collation compatible para el mapeo MySQL de Producción; eso prueba API, dominio, servicios y persistencia relacional, pero no certifica MySQL, la migración inicial, Docker ni Azure.

Resultado de la validación aislada: compilación con 0 errores y 0 advertencias, 64 comprobaciones HTTP aprobadas y persistencia de la máquina y sus tres intervenciones tras reiniciar la API. La compilación del `develop` recibido queda pendiente de la infraestructura compartida indicada arriba.

## Convenciones de control de versiones

Por instrucción expresa de Antony, esta entrega se registra directamente en `develop`, con cinco commits en inglés y sin crear ramas. Es una excepción al GitFlow solicitado por el manual: no debe presentarse como evidencia de haber usado una feature branch.

El flujo exigido por el manual contempla `main` para releases, `develop` para integración y una rama por feature, por ejemplo `feature/asset-maintenance-management`, creada desde `develop` e integrada allí tras revisión. Las convenciones para el equipo son `feature/<description-in-kebab-case>`, `release/<major>.<minor>.<patch>` desde `develop` y `hotfix/<major>.<minor>.<patch>` desde `main`; releases y hotfixes se integran en `main` y `develop`, con tag `v<major>.<minor>.<patch>`. Esta entrega no crea releases, hotfixes ni tags.

Semantic Versioning: incrementar MAJOR ante cambios incompatibles, MINOR ante funcionalidad compatible y PATCH ante correcciones compatibles. Ejemplos: `release/1.1.0`, `hotfix/1.1.1`, tag `v1.1.1`; son convenciones, no releases publicadas.

Conventional Commits: `<type>(<scope>): <description in English>`. Usar `feat` para funcionalidad, `fix` para correcciones, `test` para pruebas y `docs` para documentación; indicar incompatibilidad mediante `!` o un footer `BREAKING CHANGE:`. Los cinco commits de esta entrega separan dominio, servicios, persistencia, API y verificación.

Referencias: [Conventional Commits](https://www.conventionalcommits.org/), [GitLab: What is GitFlow?](https://about.gitlab.com/blog/what-is-gitflow/), [Atlassian: Gitflow workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow), [git-flow cheatsheet](https://danielkummer.github.io/git-flow-cheatsheet/index.html), [A successful Git branching model](https://nvie.com/posts/a-successful-git-branching-model/) y [Semantic Versioning 2.0.0](https://semver.org/).
