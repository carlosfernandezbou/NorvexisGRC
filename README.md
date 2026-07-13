# NorvexisGRC

## Introducción

NorvexisGRC es una plataforma de **Governance, Risk & Compliance (GRC)** diseñada para gestionar controles de seguridad, riesgos y cumplimiento normativo dentro de una organización.

La aplicación se estructura en tres módulos principales:

- **ISMS (Information Security Management System)**
- **SOA (Statement of Applicability)**
- **Risk Management**

La solución sigue una arquitectura **full-stack**, utilizando Angular en frontend y ASP.NET Core en backend, con Azure CosmosDB como base de datos y despliegue preparado para Microsoft Azure.

---

## Funcionalidades

- Gestión de controles ISMS
- Seguimiento de SOA
- Gestión de riesgos:
  - Identificación
  - Evaluación
  - Tratamiento
- Gestión dinámica de KPIs
- Generación automática de KPIs mediante jobs programados
- CRUD completo para entidades principales
- Control de acceso basado en roles
- Autenticación JWT
- Rate limiting y protección básica contra brute force
- Audit Trail
- Métricas y monitorización
- Documentación API mediante Swagger

---

## Arquitectura

| Capa | Tecnología |
|---|---|
| Frontend | Angular |
| Backend | ASP.NET Core (.NET) |
| Base de datos | Azure CosmosDB |
| Jobs | Quartz.NET |
| Autenticación | JWT + ASP.NET Identity |
| ORM | Entity Framework Core |
| Observabilidad | OpenTelemetry |
| Métricas | Prometheus |
| Dashboards | Grafana |
| Documentación API | Swagger / OpenAPI |
| Testing | xUnit + Moq + FluentAssertions |

---

## Estructura del Proyecto

```txt
/frontend        → Aplicación Angular
/backend         → API ASP.NET Core
/tests           → Tests unitarios e integración
```

---

## Tecnologías utilizadas

- Angular
- ASP.NET Core
- Entity Framework Core
- Azure CosmosDB
- ASP.NET Identity
- JWT Authentication
- Quartz.NET
- OpenTelemetry
- Prometheus
- Grafana
- Swagger / OpenAPI
- xUnit
- Moq
- FluentAssertions

---

## Puesta en Marcha

### Prerrequisitos

- Node.js
- Angular CLI
- .NET SDK
- Cosmos DB Emulator

---

### Instalación

```bash
git clone <repository-url>
cd <project-folder>
```

---

## Cómo ejecutar localmente

### Backend

```bash
cd NorvexisGRCBack/GRC
dotnet run
```

### Frontend

```bash
cd NorvexisGRCFront
ng serve
```

---

## Ejecución con Docker (stack completo)

El proyecto incluye un `docker-compose.yml` que levanta **todo el stack** en contenedores:
frontend (Angular + nginx), backend (ASP.NET Core), base de datos (emulador de Azure Cosmos DB),
Prometheus y Grafana.

### Prerrequisitos

- Docker Desktop (con contenedores Linux)

Se usa el emulador **vNext** de Cosmos, ligero y de arranque rápido (segundos).

### Arranque

```bash
docker compose up --build
```

El backend **espera automáticamente** al health probe del emulador (`/ready`) antes de arrancar y
sembrar la base de datos, y reintenta el seeding hasta que Cosmos acepta operaciones.

### Servicios y URLs

| Servicio | URL | Notas |
|---|---|---|
| Frontend | http://localhost:4200 | Angular servido por nginx |
| API / Swagger | http://localhost:5000/swagger | |
| Health config | http://localhost:5000/health/config | debe mostrar `isUsingDockerComposeEnvironment: true` |
| Cosmos Data Explorer | https://localhost:1234/ | emulador vNext; certificado autofirmado |
| Prometheus | http://localhost:9090 | Status → Targets → `grc-api` en **UP** |
| Grafana | http://localhost:3000 | admin / admin — datasource Prometheus ya provisionado |

### Detalles de configuración

- La conexión a Cosmos se inyecta por variables de entorno (`CosmosDb__*`), que sobrescriben
  `appsettings.json`.
- `CosmosDb__DisableTlsValidation=true` hace que el backend acepte el certificado autofirmado del
  emulador (modo Gateway). **Este flag es solo para desarrollo/Docker; nunca usarlo contra un Cosmos
  real de Azure.**
- Los datos de Cosmos y los dashboards de Grafana persisten en volúmenes nombrados
  (`cosmos-data`, `prometheus-data`, `grafana-data`), por lo que sobreviven a `docker compose down`.
  Para empezar de cero: `docker compose down -v`.
- El frontend llama a la API en `http://localhost:5000/api` (puerto publicado en el host), por lo que
  no requiere cambios para el uso local.

### Parar el stack

```bash
docker compose down        # conserva los datos
docker compose down -v     # elimina también los volúmenes
```

---

## URLs Locales

| Servicio | URL |
|---|---|
| Frontend | http://localhost:4200 |
| Swagger | http://localhost:<puerto>/swagger |
| Prometheus | http://localhost:9090 |
| Grafana | http://localhost:3000 |

---

## Documentación API

La documentación de endpoints está disponible mediante Swagger:

```txt
http://localhost:<puerto>/swagger
```

Swagger permite:

- Probar endpoints
- Autenticarse mediante JWT
- Inspeccionar requests/responses
- Validar contratos API

---

## Autenticación y Seguridad

### Roles

| Rol | Permisos |
|---|---|
| Admin | Acceso completo |
| NormalUser | Acceso limitado |

---

### JWT Authentication

La aplicación utiliza autenticación basada en **JWT (JSON Web Tokens)**.

### Flujo de autenticación

1. El usuario inicia sesión
2. El backend valida credenciales
3. Se genera un token JWT
4. El frontend almacena el token
5. El token se envía en cada request:

```http
Authorization: Bearer <token>
```

---

### Seguridad implementada

La aplicación incluye varias medidas de protección:

- JWT Authentication
- Control de acceso basado en roles
- Rate limiting
- Lockout temporal tras intentos fallidos
- Protección básica contra brute force
- Auditoría de autenticación
- Delay controlado para evitar timing attacks
- Respuestas genéricas para evitar enumeración de usuarios

---

### Bloqueo de cuenta

Tras múltiples intentos fallidos de autenticación, la cuenta queda temporalmente bloqueada.

El backend devuelve:

```http
423 Locked
```

---

### Rate limiting

La API limita el número de peticiones por IP.

Cuando se supera el límite:

```http
429 Too Many Requests
```

---

## Módulos

### ISMS

Gestión de controles y métricas relacionadas con seguridad de la información.

### SOA

Gestión del Statement of Applicability y seguimiento de controles aplicados.

### Risk Management

Gestión completa del ciclo de riesgos:

- Identificación
- Evaluación
- Tratamiento

---

## KPI Templates

La aplicación permite crear plantillas dinámicas de KPIs.

### Crear un KPI Template

1. Abrir Swagger
2. Crear un `kpi_conf`
3. Seleccionar el tipo de periodicidad
4. Crear un `kpiField_conf`
5. Asociarlo al `kpi_conf`
6. Definir:
   - `fieldName`
   - `defaultValue`

### Ejemplos

| fieldName | defaultValue |
|---|---|
| KPICategoryId | category-id |
| KPI_Name | Number of security incidents |
| TargetValue | 100 |

---

## Jobs y tareas programadas

La aplicación utiliza **Quartz.NET** para ejecutar tareas programadas en segundo plano.

### MonthlyKpiJob

El job `MonthlyKpiJob` genera automáticamente KPIs periódicos utilizando las plantillas configuradas.

### Funcionalidades

- Generación automática de KPIs
- Ejecución programada mediante cron
- Logging de errores
- Métricas Prometheus
- Ejecución desacoplada del frontend

### Flujo de ejecución

1. Quartz ejecuta el job
2. Se llama a `GenerateKpisAsync()`
3. Se generan KPIs automáticamente
4. Se registran métricas de éxito/fallo

### Métricas disponibles

| Métrica | Descripción |
|---|---|
| `kpi_jobs_executed_total` | Jobs ejecutados correctamente |
| `kpi_jobs_failed_total` | Jobs fallidos |

### Expresión Cron

Ejemplo:

```csharp
.WithCronSchedule("0 5 0 * * ?")
```

Ejecuta el job diariamente a las 00:05 UTC.

---

## Audit Trail

La aplicación registra eventos relevantes:

- Login
- Login fallido
- Logout
- Cuenta bloqueada
- Acciones sobre entidades API

Los logs antiguos se eliminan automáticamente tras un periodo definido.

---

## Base de Datos

La aplicación utiliza **Azure CosmosDB** para almacenar:

- Riesgos
- Evaluaciones
- Tratamientos
- KPIs
- Datos ISMS
- Datos SOA
- Auditoría

---

## Monitorización

La aplicación incluye monitorización mediante:

- Prometheus
- Grafana
- OpenTelemetry

---

## Configuración Prometheus

1. Descargar Prometheus
2. Descomprimir
3. Ejecutar:

```bash
.\prometheus.exe --config.file="C:\Local\NorvexisGRC\NorvexisGRCBack\GRC\prometheus.yml"
```

4. Abrir:

```txt
http://localhost:9090
```

---

## Configuración Grafana

1. Descargar Grafana
2. Abrir:

```txt
http://localhost:3000
```

3. Añadir datasource Prometheus:

```txt
Connections → Data sources → Prometheus
```

URL:

```txt
http://localhost:9090
```

---

## Queries útiles

### Requests por segundo

```promql
rate(http_server_request_duration_seconds_count[1m])
```

### Estado API

```promql
up{job="grc-api"}
```

### KPI Jobs ejecutados

```promql
kpi_jobs_executed_total
```

### Endpoints lentos

```promql
histogram_quantile(
  0.95,
  sum by (le, http_route) (
    rate(http_server_request_duration_seconds_bucket[5m])
  )
)
```

### Errores HTTP

```promql
sum by (http_response_status_code) (
  rate(http_server_request_duration_seconds_count[5m])
)
```

### Memoria .NET

```promql
dotnet_gc_heap_total_allocated_bytes_total
```

### CPU

```promql
rate(dotnet_process_cpu_time_seconds_total[1m])
```

---

## Tests

El proyecto incluye:

- Tests unitarios
- Tests de integración
- Mocking de servicios
- WebApplicationFactory
- TestAuthHandler para autenticación simulada

---

## Ejecutar tests

```bash
cd NorvexisGRCBack/GRC.Tests
dotnet test
```

> Antes de ejecutar los tests, asegúrate de que la API no esté ejecutándose para evitar bloqueos del binario `GRC.exe`.

---