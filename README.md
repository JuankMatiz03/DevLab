## Monorepo DevLab

## backend api
http://localhost:5088

## frontend angular
http://localhost:4200

## ruta base del api consumida por el front
/api

## Requisitos

windows 10 u 11

dotnet sdk 8

sql server 2022 developer

sql server management studio ssms

git

node js lts 18 o 20

angular cli global

instalar angular cli

npm i -g @angular/cli

## Paso 1 base de datos

abrir ssms y ejecutar el archivo ubicado en la carpte db del monorepo

## Paso 2 configurar backend

## archivo
server/DevLab.Api/appsettings.json

{
  "ConnectionStrings": {
    "LabDev": "Server=TU_HOSTNAME;Database=LabDev;User Id=developer;Password=abc123ABC;Encrypt=True;TrustServerCertificate=True;"
  },
  "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } },
  "AllowedHosts": "*"
}


reemplaza TU_HOSTNAME por el nombre del equipo o localhost

## Paso 3 ejecutar backend
cd server/DevLab.Api
dotnet restore
dotnet run


## swagger en
http://localhost:5088/swagger

## Paso 4 ejecutar frontend
cd web/devlab-web
npm i
npm start


## abrir
http://localhost:4200

## Endpoints principales

GET clientes
GET /api/customers

GET productos
GET /api/products

crear factura
POST /api/invoices

{
  "number": 2001,
  "customerId": 1,
  "items": [
    { "productId": 1, "quantity": 2, "unitPrice": 85000 },
    { "productId": 2, "quantity": 1, "unitPrice": 45000 }
  ]
}


## buscar por cliente
GET /api/invoices/search?customerId=1

## buscar por numero
GET /api/invoices/search?number=2001

## Flujo rapido de prueba

abrir la pantalla create invoice en el front
cargar numero de factura y cliente
agregar productos y cantidades
guardar y confirmar toast
buscar por numero para ver encabezado y detalle

## Tips de conexion sql

si ssms muestra error de cifrado marca trust server certificate en la ventana de conexion
en la app usa
Encrypt=True;TrustServerCertificate=True
o
Encrypt=False

si no deja borrar la base usa en ssms

USE master;
ALTER DATABASE [LabDev] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE [LabDev];

## Resumen

backend listo en puerto 5088
frontend listo en puerto 4200 con proxy a api
base de datos LabDev con procedimientos almacenados y tvp
todo corriendo en menos de diez minutos