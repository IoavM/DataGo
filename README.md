# 🏢 DataGo Residencial — CRUD de Clientes

Solución Full Stack para la gestión de clientes residenciales desarrollada en **.NET 8** (Backend) y **Angular 19** (Frontend), con persistencia en **PostgreSQL** mediante **Entity Framework Core**.

---

## 🏛️ Arquitectura del Sistema

La solución implementa una separación de responsabilidades limpia y modular en 3 capas lógicas:

```text
DataGo/
├── backend/                  # ASP.NET Core Web API (.NET 8)
│   ├── Controllers/          # Controladores REST delgados (ClientesController, BarriosController)
│   ├── Services/             # Lógica y validación de reglas de negocio (ClienteService)
│   ├── Data/                 # AppDbContext y configuración de Seeds
│   ├── Models/               # Entidades de dominio (ClienteResidencial, ClienteCanalModerno, BarrioCatalogo)
│   ├── DTOs/                 # Objetos de transferencia tipados (Create, Update, Response, List)
│   ├── Migrations/           # Migraciones reales generadas con EF Core
│   └── migracion_datago.sql  # Script SQL idempotente para despliegue
│
├── backend.Tests/            # Suite de pruebas unitarias xUnit (9 pruebas de reglas críticas)
│   └── ReglasNegocioTests.cs
│
└── frontend/                 # Single Page Application (Angular 19 Standalone)
    ├── src/app/pages/
    │   ├── clientes-list/    # Listado, filtros reactivos, badges y baja lógica
    │   └── cliente-nuevo/    # Flujo guiado de 6 pasos en página continua desplazable
    ├── src/app/components/
    │   └── sidebar/          # Navegación fija al 100% de la altura
    └── src/app/services/     # ClienteService con HttpClient y Signals
```

---

## 📋 Reglas de Negocio Implementadas

1. **Tratamiento y Nombres:**
   * `Sr/Sra`: Razón social extendida legal editable; `NombreCompleto`, `Nombres` y `Apellidos` calculados automáticamente mediante partición de palabras. `ClaseImpuesto` = Persona Natural.
   * `Empresa`: `NombreCompleto`, `Nombres` y `Apellidos` toman directamente la denominación de la empresa. `ClaseImpuesto` = Persona Jurídica.
2. **Identificación Fiscal:**
   * Para Empresas el tipo de documento es obligatoriamente `NIT`.
   * Validación del **Dígito de Verificación (DV)** según la fórmula oficial de la **DIAN (Módulo 11 con pesos primos)**.
   * Validación contra tabla auxiliar `ClientesCanalModerno`: Si el documento ya existe en dicho canal, la API rechaza con error indicando que debe crearse mediante el flujo especializado.
   * Prevención de documentos duplicados entre clientes activos.
3. **Comunicación:**
   * Obligatoriedad de al menos un teléfono (fijo o celular).
   * Obligatoriedad estricta de correo electrónico si el documento es NIT.
   * Filtros anti-dummy: rechazo de teléfonos (`0000000`, `1111111`, `1234567`) y correos ficticios (`test@test.com`, `asd@asd.com`).
4. **Dirección y Autocompletado:**
   * Si es rural: Habilita campo de texto libre obligatorio.
   * Si es urbana: Valida componentes de vía principal y alimentadora y genera la dirección estandarizada.
   * Catálogo de barrios que autocompleta municipio, departamento, país y zona de transporte.
5. **Condición de Pago:** Asignación fija de `0010 Contado`.
6. **Estrato:** Validación de rango entre 1 y 6.
7. **Consulta:** Si se consulta un ID inexistente, responde HTTP 404 con `"El cliente a consultar no existe"`. Si el cliente está bloqueado, incluye advertencia `"Cliente bloqueado"`.
8. **Modificación (PUT):** Un cliente bloqueado no admite modificaciones (responde HTTP 400). Solo se permite actualizar campos permitidos por el enunciado.
9. **Retiro (DELETE):** **Baja lógica obligatoria** (`Bloqueado = true`, `FechaRetiro = DateTime.UtcNow`). NUNCA se elimina físicamente el registro. Se rechaza el retiro si ya está bloqueado.

---

## 🚀 Instrucciones de Ejecución

### 1. Requisitos Previos
* .NET 8 SDK instalado.
* Node.js v18+ y npm instalados.
* PostgreSQL corriendo en `localhost:5432` con usuario `postgres` y contraseña `postgres` (configurable en `backend/appsettings.json`).

### 2. Base de Datos y Migraciones

La base de datos `datago_db` se inicializa y actualiza automáticamente al arrancar el backend. También puedes aplicar las migraciones manualmente:

```bash
cd backend
dotnet ef database update
```

*Si prefieres ejecutar el script SQL directamente en pgAdmin:*
Utiliza el archivo generado: `backend/migracion_datago.sql`.

### 3. Ejecutar el Backend (.NET 8)

```bash
cd backend
dotnet run --launch-profile http
```
* **API escuchando en:** `http://localhost:5181`
* **Swagger UI:** `http://localhost:5181/swagger`

### 4. Ejecutar las Pruebas Unitarias

Desde la raíz de la solución:
```bash
dotnet test
```
*Ejecuta 9 pruebas unitarias verificando cada una de las reglas críticas de negocio (100% de éxito).*

### 5. Ejecutar el Frontend (Angular)

```bash
cd frontend
npm start -- --port 4250
```
* **Aplicación disponible en:** `http://localhost:4250`

---

## 📝 Supuestos Asumidos (Decisiones Documentadas)

1. **Separación de Nombres y Apellidos en `Sr/Sra`:**  
   Al recibir la razón social extendida, las dos últimas palabras se asumen como apellidos y las anteriores como nombres de pila. Si solo tiene dos palabras, la primera es nombre y la segunda apellido.
2. **Visibilidad de Clientes Retirados en el Listado:**  
   Los clientes en baja lógica permanecen visibles en la tabla general para efectos de trazabilidad y auditoría histórica, pero se destacan con un badge de *"Bloqueado"* y se deshabilitan las acciones de modificación.
3. **Expresiones Dummy:**  
   Se definió una lista negra para prevenir valores placeholder en teléfonos repetitivos y correos de prueba.
