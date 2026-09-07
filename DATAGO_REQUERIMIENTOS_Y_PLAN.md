# 📋 PROYECTO DATAGO: REQUERIMIENTOS CLAVE Y ESPECIFICACIONES

**Rol evaluado:** Desarrollador Full Stack Junior (.NET + Angular)  
**Duración oficial:** 3 horas continuas  
**Entregable:** Repositorio Git con Backend (.NET), Frontend (Angular), Migraciones reales de Base de Datos y README.

---

## 🎯 1. OBJETIVO DEL RETO
Construir el **CRUD completo de Cliente Residencial** (Crear, Consultar, Modificar y Retirar/Baja Lógica) aplicando estrictas reglas de negocio en el **Backend**.

> ⚠️ **REGLA CRÍTICA:** No se requiere autenticación ni roles (asumir un único usuario). Las validaciones deben ejecutarse y responder en el **Backend** con códigos HTTP apropiados.

---

## 📦 2. MODELO DE DATOS Y CAMPOS

### A. Datos Generales
* **Tratamiento (Obligatorio):** `Sr/Sra` o `Empresa`.
* **Nombre de Negocio (Obligatorio, Editable):** Nombre comercial con el que se conoce al cliente.
* **Razón Social Extendida (Obligatorio):** Denominación legal completa.
* **Campos Calculados (Solo lectura en pantalla):**
  * `Nombre Completo`, `Nombres`, `Apellidos`.
  * **Si Tratamiento es `Sr/Sra`:**
    * *Nombre Completo:* `Nombres` y `Apellidos`.
    * *Nombres:* Nombres separados por coma.
    * *Apellidos:* Apellidos separados por coma.
    * *(Supuesto a documentar: algoritmo de partición por palabras, ej: primeras palabras como nombres, últimas dos como apellidos).*
  * **Si Tratamiento es `Empresa`:**
    * `Nombre Completo`, `Nombres` y `Apellidos` contienen todos el nombre de la empresa.

### B. Identificación Fiscal
* **Tipo de Documento propuesto:**
  * Si es `Sr/Sra`: Propone `Cédula de Ciudadanía` (permite cambiar a `Cédula de Extranjería`).
  * Si es `Empresa`: Propone `NIT` (bloqueado, no se puede modificar).
* **Validación de Estructura:**
  * Validar número según reglas colombianas.
  * Para **NIT:** Validar y calcular el **Dígito de Verificación (DV)** según la fórmula oficial DIAN (módulo 11). Error específico si el NIT o DV es incorrecto.
* **Validación contra "Clientes Canal Moderno":**
  * Debe existir una tabla auxiliar (`ClientesCanalModerno`) con tipo y número de documento pre-cargados de ejemplo.
  * Si el documento ya existe en esa tabla -> Rechazar con error: *"El cliente debe crearse mediante el flujo 'Creación clientes canal moderno'"*.

### C. Datos de Comunicación
* **Campos:** Teléfono fijo (con extensión), Celular, Correo electrónico.
* **Regla 1:** Debe existir al menos un teléfono (fijo o celular).
* **Regla 2:** El correo electrónico es **obligatorio únicamente si el tipo de documento es NIT**.
* **Regla 3 (Anti-Dummy):** Validar que no se ingresen datos basura (ej: teléfonos "1111111", "0000000", "1234567" o correos como "test@test.com", "a@a.com").

### D. Dirección
* **Autocompletado por Barrio:** Catálogo de barrios que autocompleta en solo lectura: `Municipio`, `Departamento`, `País` y `Zona de transporte`.
* **Checkbox "Dirección rural":**
  * Si se marca: Se habilita un único campo de texto libre.
  * Si no es rural (urbana): Se habilitan y validan:
    * *Vía principal:* Tipo/Nombre, Letra, Número, Cardinalidad.
    * *Vía alimentadora:* Número, Letra, Cardinalidad, Número (placa), Cardinalidad.

### E. Otros Campos
* **Centro (Obligatorio):** Texto o catálogo simple.
* **Estrato (Obligatorio):** Lista desplegable del 1 al 6.
* **Clase de Impuesto (Solo lectura, fijado automáticamente):**
  * `Sr/Sra` ➔ `Persona Natural`.
  * `Empresa` ➔ `Persona Jurídica`.
* **Condición de Pago (Solo lectura, fijado automáticamente):**
  * Por defecto: `0010 Contado`.

---

## 🔄 3. OPERACIONES DEL CRUD Y REGLAS

### 1. Creación (POST `/api/clientes`)
* Valida todas las reglas de negocio anteriores.
* Si el documento existe en Canal Moderno ➔ HTTP 400.
* Si falta teléfono o datos requeridos ➔ HTTP 400.
* Éxito ➔ HTTP 201 Created con el cliente guardado.

### 2. Consulta (GET `/api/clientes` y GET `/api/clientes/{id}`)
* Listado general y detalle por ID.
* Filtros de búsqueda: por **documento de identificación**, **nombre** o **código/ID**.
* Si se consulta un ID puntual que no existe ➔ HTTP 404 con mensaje explícito:  
  `"El cliente a consultar no existe"`.
* Si el cliente está **bloqueado/retirado** ➔ Mostrar los datos con una advertencia visible: `"Cliente bloqueado"`.

### 3. Modificación (PUT `/api/clientes/{id}`)
* **Regla de oro:** Si el cliente está marcado como **bloqueado/retirado**, **NO PUEDE MODIFICARSE**. Responder HTTP 400 con error claro: *"No se puede modificar un cliente bloqueado"*.
* **Campos editables:** Nombre negocio, Nombres / Apellidos (recalculan razón social extendida), Teléfono / Celular / Correo, Barrio / Dirección, Estrato, Centro.
* **Campos bloqueados (no editables):** Tratamiento, tipo y número de documento, clase de impuesto, condición de pago.

### 4. Retiro (DELETE / PATCH `/api/clientes/{id}/retirar`)
* **BAJA LÓGICA OBLIGATORIA:** Nunca eliminar físicamente de la base de datos.
* Se marca con `Bloqueado = true`, `Activo = false` y `FechaRetiro = DateTime.UtcNow`.
* Validar que el cliente exista.
* Validar que no esté ya retirado previamente (si ya está retirado, responder error).

---

## 🏛️ 4. ESTRUCTURA INICIAL DEL PROYECTO CREADO

El proyecto ya quedó inicializado en la carpeta:  
`c:\Users\ioavm\OneDrive\Escritorio\IOAV\DataGo`

```text
DataGo/
├── backend/                       # ASP.NET Core Web API (.NET 8)
│   ├── Controllers/               # Controladores de la API (con Swagger)
│   ├── Program.cs                 # Configuración de servicios y CORS
│   └── backend.csproj             # Proyecto C#
│
├── frontend/                      # Angular 19+ (Standalone)
│   ├── src/app/                   # Componentes, vistas y servicios
│   └── package.json               # Dependencias ya instaladas
│
├── prueba_tecnica_junior_datago_3.pdf  # Enunciado original de la prueba
└── DATAGO_REQUERIMIENTOS_Y_PLAN.md    # Este documento maestro
```

---

## 📝 5. SUPUESTOS DOCUMENTADOS PARA EL README

1. **Separación de Nombres y Apellidos en `Sr/Sra`:**  
   Al recibir la razón social extendida (ej: *"Juan Carlos Pérez Gómez"*), si contiene más de dos palabras, las dos últimas se asumen como apellidos (`Pérez, Gómez`) y las anteriores como nombres (`Juan, Carlos`). Si tiene dos palabras, la primera es nombre y la segunda apellido.
2. **Listado de Clientes Retirados:**  
   Los clientes retirados/bloqueados sí se muestran en la tabla general pero visualmente identificados con un badge rojo de `"BLOQUEADO"`, y con los botones de edición deshabilitados.
3. **Expresiones Dummy Prohibidas:**  
   Teléfonos con dígitos idénticos consecutivos (`1111111`, `0000000`) o secuencias obvias (`1234567`), y correos que contengan dominios ficticios como `@test.com`, `@ejemplo.com`, `@fake.com` o nombres de usuario como `asd`, `admin`, `dummy`.
