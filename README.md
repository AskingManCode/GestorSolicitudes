# Gestor de Solicitudes - Pública S.A.

Sistema web para la gestión centralizada de solicitudes internas de la empresa ficticia **Pública S.A.**

## Descripción general

El proyecto reemplaza el manejo de información en archivos Excel por una aplicación web que permite registrar, controlar y auditar solicitudes de forma segura y ordenada.

Este sistema ayuda a reducir errores operativos, mejorar la trazabilidad, automatizar el control de vencimientos y centralizar la información de clientes, representantes, tareas y reportes en una sola plataforma.

## Objetivos del sistema

- Registrar solicitudes con información completa y estructurada.
- Administrar representantes y usuarios del sistema.
- Controlar estados de cada solicitud y sus vencimientos.
- Registrar tareas asociadas por solicitud.
- Generar reportes de montos, horas y seguimiento.
- Enviar notificaciones por correo al momento de los cambios de estado.
- Mantener una bitácora de auditoría para cada acción relevante.

## Módulos principales

| Módulo | Historias de usuario | Funcionalidad principal |
|---|---|---|
| Usuarios | USR1 – USR4 | Login, bienvenida, panel general y administración de usuarios |
| Solicitudes | SOL1 – SOL7 | Registro, desglose mensual, representantes, estados, vencimientos y reportes |
| Tareas | TAR1 – TAR3 | Gestión de tareas por solicitud y reportes de horas |

## Stack tecnológico

- **Lenguaje:** C#
- **Framework web:** ASP.NET Core / Razor Pages
- **Base de datos:** MySQL
- **Acceso a datos:** Dapper
- **UI:** Bootstrap
- **Arquitectura:** Monolito en capas
- **Control de versiones:** Git

## Estructura del repositorio

```text
GestorSolicitudes/
├── Codigo/
│   └── PublicaSA/
│       ├── PublicaSA.WebPresentation/
│       ├── PublicaSA.BusinessLogic/
│       ├── PublicaSA.DataAccess/
│       ├── PublicaSA.Entities/
│       └── PublicaSA.slnx
├── Documentacion/
├── Pruebas Tecnicas/
├── Scripts SQL/
│   ├── 01. GestorSolicitudesDB.sql
│   └── 02. DatosIniciales.sql
├── .gitignore
├── README.md
└── .git/
```

## Arquitectura

El sistema está implementado como un monolito en capas:

| Capa | Proyecto | Responsabilidad |
|---|---|---|
| Entidades | `PublicaSA.Entities` | Modelos de dominio y entidades de negocio |
| Acceso a datos | `PublicaSA.DataAccess` | Repositorios y acceso a MySQL con Dapper |
| Lógica de negocio | `PublicaSA.BusinessLogic` | Validaciones, servicios, cifrado, correos y tareas automáticas |
| Presentación | `PublicaSA.WebPresentation` | Interfaz web con Razor Pages y vistas |

Flujo de ejecución: `WebPresentation -> BusinessLogic -> DataAccess -> MySQL`.

## Requisitos previos

- .NET SDK 10
- MySQL o MariaDB
- Git
- Visual Studio 2022 o posterior (recomendado)

## Instalación y ejecución local

1. Clonar el repositorio:

   ```bash
   git clone https://github.com/AskingManCode/GestorSolicitudes.git
   cd GestorSolicitudes
   ```

2. Abrir la solución en Visual Studio o desde la línea de comandos:

   ```bash
   cd Codigo/PublicaSA
   ```

3. Crear la base de datos ejecutando los scripts de `Scripts SQL`:

   ```sql
   -- Ejecutar primero
   01. GestorSolicitudesDB.sql

   -- Opcional, según corresponda para datos iniciales
   02. DatosIniciales.sql
   ```

4. Configurar la conexión y variables de entorno en el archivo de configuración del proyecto web:

   ```text
   Codigo/PublicaSA/PublicaSA.WebPresentation/appsettings.json
   ```

   o en `appsettings.Development.json` para entornos locales.

5. Ejecutar la aplicación:

   ```bash
   dotnet restore
   dotnet run --project Codigo/PublicaSA/PublicaSA.WebPresentation/PublicaSA.WebPresentation.csproj
   ```

6. Abrir la URL indicada por la consola en el navegador.

## Configuración importante

Los valores sensibles no deben subirse al repositorio. Se recomienda configurarlos localmente mediante secretos del usuario o un archivo de desarrollo no versionado.

| Valor | Uso |
|---|---|
| Cadena de conexión a MySQL | Conexión a la base de datos |
| Clave AES-GCM 256 | Cifrado de contraseñas |
| Configuración SMTP | Envío de correos por cambios de estado |
| Frecuencia de vencimientos | Ejecución del proceso automático |

## Reglas del negocio

- El usuario se bloquea tras 3 intentos fallidos de login.
- Las contraseñas se almacenan cifradas con AES-GCM 256.
- Las solicitudes se crean en estado "Nueva".
- Una solicitud vence a los 5 días hábiles sin respuesta y pasa a "Vencida".
- El color del listado representa el estado: rojo = vencida, verde = nueva, amarillo = vence hoy.
- El IVA se calcula como 13% del monto y el total incluye el IVA.
- No se permite eliminar registros con información relacionada.
- Cada cambio de estado genera una notificación por correo al representante relacionado.

## Bitácora y auditoría

Toda operación de creación, modificación, eliminación y consulta queda registrada junto con errores técnicos relevantes.

| Acción | Registro guardado |
|---|---|
| Crear | Acción + JSON del nuevo registro |
| Modificar | Acción + JSON del valor anterior y actual |
| Eliminar | Acción + JSON del registro eliminado |
| Consultar | Mensaje del tipo "El usuario consulta `<elemento>`" |

Cada evento incluye fecha, usuario y, cuando aplica, la acción ejecutada por un proceso automático registrada como "Sistema".

## Flujo de trabajo con Git

```text
main        <- código estable, solo vía Pull Request desde develop
└── develop    <- integración del equipo
    └── feature/<id-hu>-<nombre-corto>   <- rama por historia o tarea
```

1. Crear la rama desde `develop`.
2. Hacer commits pequeños y descriptivos.
3. Abrir un Pull Request hacia `develop`.
4. Solicitar revisión a otro integrante.
5. No hacer push directo a `main` ni `develop`.
6. Al cierre del alcance, integrar `develop` en `main` mediante Pull Request.

## Pruebas técnicas

Cada historia de usuario debe validarse al finalizar su desarrollo. Se recomienda documentar al menos un caso de prueba por criterio de aceptación, con evidencia visual o de ejecución.

| # HU | Criterio de aceptación | Evidencia |
|---|---|---|
| ID de la HU | Descripción del criterio probado | Captura o evidencia de ejecución exitosa |

Las pruebas consolidadas se alojan en la carpeta `Pruebas Tecnicas/` según su estructura interna.

## Despliegue

Para desplegar la aplicación en Ples:

1. Crear la base de datos MySQL y ejecutar los scripts SQL del proyecto.
2. Publicar la solución en modo Release:

   ```bash
   dotnet publish Codigo/PublicaSA/PublicaSA.WebPresentation/PublicaSA.WebPresentation.csproj -c Release -o <carpeta-de-publicacion>
   ```

3. Crear el sitio en Plesk apuntando a la carpeta publicada.
4. Configurar la cadena de conexión, la clave de cifrado y la configuración SMTP del servidor.
5. Verificar login, reportes, notificaciones y proceso de vencimiento.

**URL del sistema desplegado:** pendiente por definir.

## Equipo

- Sebastián Jiménez Arrieta
- Aldo Burgos Argüello
- Dorian Blanco Muñoz
- Cristian Monge Raabe
- Juan Hidalgo Muñoz
