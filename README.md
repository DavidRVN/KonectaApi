
# Prueba Teórico/Práctica - Konecta

Este repositorio contiene el desarrollo de la prueba técnica para el proceso de selección de Desarrollador en Konecta.

## 1. Requisitos previos
- Visual Studio 2022 Community
  - ASP.NET y desarrollo web
  - Desarrollo de escritorio con .NET Framework
  - Herramientas de datos y SQL Server
  - .NET Framework 4.7 Targeting Pack
- SQL Server 2019 o superior
- SQL Server Management Studio (SSMS)
- Postman (opcional para pruebas de API)

## 2. Instalación
1. Clonar o descargar este repositorio.
2. Abrir la solución `KonectaAPI.sln` en Visual Studio.
3. Configurar la cadena de conexión en `Web.config`:

```xml
<connectionStrings>
  <add name="KonectaDB"
       connectionString="Data Source=localhost;Initial Catalog=KonectaDB;Integrated Security=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

> Ajustar `Data Source` según la instancia de SQL Server.

## 3. Script SQL
Ejecutar en SSMS:

```sql
CREATE DATABASE KonectaDB;
GO

USE KonectaDB;
GO

CREATE TABLE Areas (
    IdArea INT IDENTITY(1,1) PRIMARY KEY,
    NombreArea NVARCHAR(100) NOT NULL
);

CREATE TABLE Colaboradores (
    NumeroIdentificacion NVARCHAR(20) PRIMARY KEY,
    Nombres NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Direccion NVARCHAR(200) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    Telefono NVARCHAR(20) NOT NULL,
    Salario DECIMAL(18,2) NOT NULL,
    IdArea INT NOT NULL,
    FechaIngreso DATE NOT NULL,
    Sexo CHAR(1) CHECK (Sexo IN ('M','F')),
    CONSTRAINT FK_Colaboradores_Areas FOREIGN KEY (IdArea) REFERENCES Areas(IdArea)
);

INSERT INTO Areas (NombreArea) VALUES
('Recursos Humanos'),
('Tecnología'),
('Contabilidad');
```

## 4. Ejecución del proyecto
1. Seleccionar el proyecto `KonectaAPI` como proyecto de inicio en Visual Studio.
2. Ejecutar con **F5**.
3. Abrir el frontend en:  
   `https://localhost:44371/FrontEnd/index.html`

## 5. Endpoints
- **POST** `/api/RegistrarColaborador`
- **GET** `/api/ConsultarColaboradorPorIdentificacion/{id}`
- **PUT** `/api/ActualizarColaborador/{id}`
- **DELETE** `/api/EliminarColaborador/{id}`

## 6. Frontend
- HTML + Bootstrap + JavaScript puro
- Funcionalidades:
  - Registrar colaborador
  - Consultar colaborador
  - Editar colaborador
  - Eliminar colaborador

## 7. Autor
- **Nombre:** David Rios
- **Correo:** riozv95@gmail.com
