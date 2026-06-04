# 🏗️ Firmeza – Módulo Administrativo Base

Sistema de administración web para un negocio de venta y distribución de materiales de construcción, desarrollado con ASP.NET Core 8 Razor Pages, PostgreSQL y Entity Framework Core.

---

## 📋 Descripción

Firmeza es un panel administrativo que permite gestionar productos, clientes y ventas del negocio. Cuenta con autenticación basada en roles, donde los administradores tienen acceso completo al panel y los clientes están restringidos.

---

## 🛠️ Tecnologías utilizadas

- **ASP.NET Core 8** – Framework web (Razor Pages)
- **Entity Framework Core 8** – ORM para acceso a datos
- **PostgreSQL** – Base de datos relacional
- **Npgsql** – Proveedor de PostgreSQL para EF Core
- **ASP.NET Core Identity** – Autenticación y autorización
- **Bootstrap 5** – Diseño y estilos
- **Bootstrap Icons** – Iconografía
- **EPPlus** – Exportación a Excel (instalado, uso futuro)
- **QuestPDF** – Generación de PDFs (instalado, uso futuro)

---

## 📁 Estructura del proyecto

```
Firmeza/
├── Firmeza.sln
└── Firmeza.Web/
    ├── Data/
    │   └── ApplicationDbContext.cs
    ├── Models/
    │   ├── ApplicationUser.cs
    │   ├── Producto.cs
    │   ├── Cliente.cs
    │   ├── Venta.cs
    │   └── DetalleVenta.cs
    ├── Pages/
    │   ├── Login.cshtml
    │   ├── Logout.cshtml
    │   ├── AccesoDenegado.cshtml
    │   ├── Admin/
    │   │   ├── Dashboard.cshtml
    │   │   ├── Productos.cshtml
    │   │   ├── CrearProducto.cshtml
    │   │   ├── EditarProducto.cshtml
    │   │   ├── Clientes.cshtml
    │   │   ├── CrearCliente.cshtml
    │   │   ├── EditarCliente.cshtml
    │   │   ├── Ventas.cshtml
    │   │   ├── CrearVenta.cshtml
    │   │   └── DetalleVenta.cshtml
    │   └── Shared/
    │       └── _AdminLayout.cshtml
    ├── Migrations/
    ├── wwwroot/
    ├── appsettings.json
    └── Program.cs
```

---

## 🗄️ Diagrama Entidad-Relación

```
┌─────────────────┐       ┌─────────────────┐
│   AspNetUsers   │       │    Productos     │
│─────────────────│       │─────────────────│
│ Id (PK)         │       │ Id (PK)          │
│ UserName        │       │ Nombre           │
│ Email           │       │ Descripcion      │
│ NombreCompleto  │       │ Precio           │
│ PasswordHash    │       │ Stock            │
└─────────────────┘       └────────┬────────┘
                                   │ 1
┌─────────────────┐                │
│    Clientes     │       ┌────────▼────────┐
│─────────────────│       │  DetallesVenta  │
│ Id (PK)         │       │─────────────────│
│ Nombre          │       │ Id (PK)          │
│ Documento       │       │ VentaId (FK)     │
│ Correo          │       │ ProductoId (FK)  │
│ Telefono        │       │ Cantidad         │
└────────┬────────┘       │ PrecioUnitario   │
         │ 1              └────────▲────────┘
         │                        │ N
┌────────▼────────┐      ┌────────┴────────┐
│     Ventas      │──────│                 │
│─────────────────│  1   │                 │
│ Id (PK)         │      └─────────────────┘
│ Fecha           │
│ ClienteId (FK)  │
└─────────────────┘
```

---

## 🏛️ Diagrama de Clases

```
ApplicationUser (IdentityUser)
├── + NombreCompleto: string

Producto
├── + Id: int
├── + Nombre: string
├── + Descripcion: string
├── + Precio: decimal
└── + Stock: int

Cliente
├── + Id: int
├── + Nombre: string
├── + Documento: string
├── + Correo: string
└── + Telefono: string

Venta
├── + Id: int
├── + Fecha: DateTime
├── + ClienteId: int
├── + Cliente: Cliente
└── + Detalles: List<DetalleVenta>

DetalleVenta
├── + Id: int
├── + VentaId: int
├── + Venta: Venta
├── + ProductoId: int
├── + Producto: Producto
├── + Cantidad: int
└── + PrecioUnitario: decimal

ApplicationDbContext (IdentityDbContext<ApplicationUser>)
├── + Productos: DbSet<Producto>
├── + Clientes: DbSet<Cliente>
├── + Ventas: DbSet<Venta>
└── + DetallesVenta: DbSet<DetalleVenta>
```

---

## 🔐 Roles del sistema

| Rol | Acceso |
|-----|--------|
| **Admin** | Panel completo (Dashboard, Productos, Clientes, Ventas) |
| **Cliente** | Bloqueado en Razor – redirigido a página de Acceso Denegado |

**Credenciales por defecto del administrador:**
- Email: `admin@firmeza.com`
- Contraseña: `Admin123!`

---

## ⚙️ Instalación local

### Requisitos previos
- .NET SDK 8.0+
- PostgreSQL 16+
- dotnet-ef tool

### Pasos

**1. Clonar el repositorio:**
```bash
git clone https://github.com/tzerk-last/Firmeza-M-dulo-Administrativo-Base.git
cd Firmeza
```

**2. Configurar la base de datos en `appsettings.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=FirmezaDB;Username=tu_usuario;Password=tu_contraseña"
  }
}
```

**3. Crear la base de datos y aplicar migraciones:**
```bash
cd Firmeza.Web
dotnet ef database update
```

**4. Ejecutar el proyecto:**
```bash
dotnet run
```

**5. Abrir en el navegador:**
```
http://localhost:5062/Login
```

---

## 🐳 Docker (borrador)

### Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Firmeza.Web/Firmeza.Web.csproj", "Firmeza.Web/"]
RUN dotnet restore "Firmeza.Web/Firmeza.Web.csproj"
COPY . .
WORKDIR "/src/Firmeza.Web"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Firmeza.Web.dll"]
```

### docker-compose.yml
```yaml
version: '3.8'
services:
  web:
    build: .
    ports:
      - "8080:80"
    environment:
      - ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=FirmezaDB;Username=postgres;Password=postgres
    depends_on:
      - db

  db:
    image: postgres:16
    environment:
      POSTGRES_DB: FirmezaDB
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data

volumes:
  pgdata:
```

---

## ✅ Funcionalidades implementadas

- [x] Autenticación con roles (Admin / Cliente)
- [x] Migraciones EF Core (sin SQL manual)
- [x] Dashboard con métricas (productos, clientes, ventas)
- [x] CRUD completo de Productos
- [x] CRUD completo de Clientes
- [x] CRUD completo de Ventas con detalles
- [x] Búsqueda y filtrado de productos y clientes
- [x] Manejo de errores con try-catch
- [x] Página de Acceso Denegado
- [x] Diseño responsive con Bootstrap 5
- [x] Barra lateral de navegación

---

## 👨‍💻 Autor

Proyecto desarrollado como parte del módulo M6.3S1 – Firmeza Administrativo Base.
