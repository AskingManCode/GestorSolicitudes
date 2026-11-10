# Pantallas EXT4 y EXT5 · Registrar propuesta

Este plugin agrega la pantalla de registro sin depender de que EXT3 esté terminada. Reutiliza la operación SOAP `RegistrarPropuesta` del servicio WCF ya construido. No consulta directamente la base de datos.

## Ubicación y función de los archivos

Guardá esta carpeta completa en `Codigo/PublicaSA_Wordpress/publicasa-ext4` de la rama `feat/ext4-ext5-propuestas-excel`.

| Archivo | Función |
| --- | --- |
| `publicasa-ext4.php` | Registra el plugin, los ajustes, el shortcode y el envío al WCF. Valida nuevamente los campos recibidos. |
| `templates/formulario.php` | Presenta datos del proveedor, mensajes, acciones y filas mensuales. |
| `templates/fila.php` | Define los ocho datos de cada desglose. |
| `assets/formulario.js` | Agrega y quita meses, calcula IVA y total, y detecta períodos repetidos antes de enviar. |
| `assets/formulario.css` | Presentación adaptable a computadora y celular, limitada a la pantalla EXT4. |

El plugin vive en WordPress porque allí está el frontend externo del proyecto. La validación de disponibilidad, el alta del proveedor, el control de duplicados, la transacción y la bitácora siguen a cargo del servicio de negocio existente.

## Actualizar a la versión 0.2.0

En WordPress, usá **Plugins → Añadir plugin → Subir plugin**, seleccioná el ZIP actualizado y elegí **Reemplazar el actual con el subido**. La carpeta sigue siendo `publicasa-ext4`; se conservan los ajustes y el shortcode de la página. No hace falta borrar el plugin.

Los resultados y las validaciones del formulario se muestran en una ventana centrada con fondo oscurecido. **Éxito** usa encabezado verde; **Aviso**, amarillo; **Error**, rojo. Se cierra con Aceptar, la X o Escape. Los mensajes conservan sus tildes y no se interpretan como HTML.

## Instalar para probar

1. En WordPress, entrá en **Plugins → Añadir nuevo → Subir plugin**, seleccioná el ZIP del plugin, instalalo y activalo. Otra opción es copiar esta carpeta en `wp-content/plugins/publicasa-ext4` y activarlo desde Plugins.
2. Creá una página con título **Registrar propuesta** y slug **registrar-propuesta**. Agregá un bloque Shortcode con `[publicasa_registrar_propuesta]` y publicá la página.
3. En **Ajustes → Propuestas EXT4**, configurá la dirección WSDL. Para el equipo donde funciona Visual Studio: `http://localhost:53139/Service1.svc?wsdl`.
4. PHP necesita la extensión **SOAP**. Si usás XAMPP, habilitá `extension=soap` en el `php.ini` que utiliza Apache y reiniciá Apache.
5. Iniciá el WCF en Visual Studio y dejalo ejecutándose.
6. Abrí la página de WordPress añadiendo `?idSolicitud=27` a su dirección. Si WordPress está en un subdirectorio, conservá ese prefijo. Elegí una solicitud vigente de tu base de datos.

`localhost` se interpreta desde el servidor de WordPress. Funciona si PHP y WCF corren en la misma computadora. Si WordPress está en un contenedor o en otro servidor, configurá una dirección del WCF accesible desde ese entorno. La dirección de IIS Express de desarrollo no es una dirección de despliegue.

No agregues las credenciales de la base de datos al plugin. El WCF continúa usando `Conexiones.local.config`.

La página debe excluirse de cualquier caché de página completa, para que los mensajes y nonces no se compartan entre visitantes. Una solicitud enviada debe procesarse una sola vez; el servicio conserva su control de propuesta duplicada.

## Contrato con EXT3: botón Participar

La entrada acordada es una petición GET a la página de EXT4:

```text
/registrar-propuesta/?idSolicitud=27
```

`idSolicitud` es un entero positivo, correspondiente a `IdSolicitud` de la solicitud que se está consultando. No se pasan datos del proveedor, contraseñas ni desgloses en la dirección.

En la plantilla PHP donde tu compañero dibuje el botón **Participar**, puede usar:

```php
<?php
// $idSolicitud corresponde a la solicitud mostrada por EXT3.
// El helper conserva la dirección base configurada en WordPress.
$url_participar = publicasa_ext4_url($idSolicitud);
?>
<a href="<?php echo esc_url($url_participar); ?>">Participar</a>
```

Esta llamada va dentro de la plantilla de detalle de EXT3, porque desde esa pantalla se inicia la participación. El plugin debe estar activo para que exista `publicasa_ext4_url()`.

Si EXT3 construye su propio enlace, basta con respetar la ruta y el parámetro anteriores. No necesita llamar a RegistrarPropuesta: esa operación se llama al confirmar EXT4.

## Regreso al detalle EXT3

En **Ajustes → Propuestas EXT4**, el campo **Ruta del detalle EXT3** acepta una ruta de este WordPress, por ejemplo:

```text
/detalle-solicitud/?idSolicitud={id}
```

El plugin sustituye `{id}` por el identificador recibido. Tu compañero debe confirmar el slug y el nombre del parámetro de EXT3; entonces se actualiza este ajuste. **Cancelar** y **Volver a la solicitud** usan esa ruta. Hasta que EXT3 exista, el regreso puede mostrar una página no encontrada.

No se usa la cabecera Referer como destino de regreso.

## Comportamiento del formulario

- Sin identificador, o con un identificador inválido, se indica que debe ingresarse desde Participar.
- Se pide cédula jurídica y nombre del proveedor. El formato permitido es de 10 dígitos o `3-101-999999`.
- Los tres datos de contacto están disponibles para el proveedor nuevo. El servicio determina si la cédula existe y exige representante, teléfono y correo únicamente para un alta. Esta versión no consulta ni autocompleta el nombre al salir de la cédula: el WCF actual solo expone el registro.
- Cada desglose pide mes, año, horas, monto, porcentaje y observaciones. IVA y total se calculan automáticamente y se envían al servicio para su validación.
- No se permite repetir mes y año dentro de la misma propuesta. No se impone que la suma de porcentajes sea 100, porque esa regla no está en los requisitos usados.
- Al confirmar, PHP llama a `RegistrarPropuesta` con `registro.IdSolicitud`, `registro.Proveedor` y `registro.Desgloses.DesglosePropuesta`.
- El formulario no envía identificadores internos del proveedor/desglose, estados ni fechas. El servicio asigna esos datos.
- Si el servicio rechaza el registro, se muestra su mensaje y se conservan los campos. Si tiene éxito, se muestra el identificador devuelto y se retira el formulario.
- Si la comunicación falla después del envío, se informa que hay que verificar el registro antes de reenviar: la operación podría haberse guardado.
- **Cargar Excel** abre una vista dentro de la pantalla. Se conserva el proveedor y se puede cancelar la carga sin modificar los desgloses. La carga exitosa reemplaza los desgloses, vuelve al formulario y muestra un aviso. El registro ocurre únicamente al confirmar la propuesta.

## EXT5: formato y carga de Excel

1. En EXT4, pulsá **Cargar Excel** y descargá la plantilla incluida.
2. La primera fila contiene los ocho encabezados: **Mes, Año, Horas, Monto, IVA, Total, Observaciones, Porcentaje de cobro**. Se permite otro orden, pero no columnas repetidas ni datos en columnas sin encabezado. También se acepta `Anio` y `PorcentajeCobro`.
3. Cada fila siguiente corresponde a un desglose. Los dos ejemplos se reemplazan por los datos de la propuesta. Se ignoran las filas completamente vacías. No agregues filas de totales.
4. Mes debe ser un número del 1 al 12. Año: de 2000 a 2150. Importes, horas y porcentaje: números positivos o cero, con hasta dos decimales. El porcentaje usa 50 para 50%; también se reconoce una celda de Excel con formato porcentual. Los montos numéricos pueden tener formato visual de moneda. Si escribís números como texto, no uses separadores de miles.
5. IVA y Total son requeridos y se verifican antes de importar. La plantilla incluye fórmulas `ROUND(Monto*13%,2)` y `Monto+IVA`. Guardá el Excel después de recalcularlo. Si una fórmula no tiene resultado guardado, se pide recalcular y volver a guardar. Las fórmulas no se ejecutan en el navegador.
6. Seleccioná un archivo `.xlsx` o `.xls` de hasta **5 MB**, elegí la hoja visible con el desglose y pulsá **Cargar desgloses**. Se permiten hasta **100 desgloses** por carga y una hoja de hasta 1001 filas físicas, contando el encabezado. Estas cotas mantienen la importación dentro del límite habitual de variables POST de PHP. No se admiten archivos con contraseña.
7. Si hay un error, se informa la fila real del Excel y no se importa ninguna fila. El formulario conserva sus desgloses anteriores. Corregí el Excel, guardalo y volvé a seleccionarlo.
8. Si todo es válido, los desgloses reemplazan los actuales y se vuelve a EXT4 para revisar o editar. La importación no llama al WCF ni guarda una propuesta. El botón **Confirmar propuesta** realiza el envío normal, con las mismas validaciones de PHP y del servicio.

El archivo se procesa localmente en un Web Worker con un límite de 15 segundos por operación. SheetJS 0.20.3 se incluye en `assets/vendor`, junto con su licencia Apache 2.0. El plugin funciona sin descargar librerías desde sitios externos. Los datos importados se asignan a valores de campos, sin interpretar su contenido como HTML.

### Archivos de EXT5

- `assets/excel-worker.js`: lectura del libro y selección de hoja sin bloquear la interfaz.
- `assets/excel-validacion.js`: encabezados, campos requeridos, rangos, períodos repetidos e IVA/total.
- `assets/plantilla-propuesta.xlsx`: ejemplo editable con dos meses.
- `assets/vendor/xlsx.full.min.js`: lector de Excel de terceros.

## Prueba en el ambiente real

1. Abrir la página sin `idSolicitud`: debe mostrar el aviso de entrada desde Participar.
2. Abrirla con un identificador válido y agregar dos meses: los valores de ambas filas deben permanecer independientes.
3. Escribir monto `100000`: IVA `13000.00`, total `113000.00`. Con monto `0.50`: IVA `0.07`, total `0.57`.
4. Repetir mes y año en dos filas: no debe enviarse la propuesta.
5. Usar la cédula de prueba `3101999999` y la solicitud `27`, si conservan el registro de la prueba anterior: el WCF debe devolver que el proveedor ya tiene una propuesta para esa solicitud.
6. Para probar un alta exitosa, usar datos de prueba autorizados y una combinación de proveedor/solicitud que aún no tenga propuesta. Comprobar el resultado, las filas guardadas y la bitácora en MySQL.
7. Desconectar el servicio: debe mostrarse un mensaje general conservando el formulario.
8. Ajustar la ruta de EXT3 cuando esté lista y verificar Cancelar desde la solicitud seleccionada.

La prueba completa contra WordPress, el WSDL real y MySQL debe ejecutarse en el equipo del proyecto. Una validación local de sintaxis o de la pantalla no reemplaza esa prueba.

### Pruebas adicionales de EXT5

1. Completar proveedor y una fila manual. Entrar en Cargar Excel y cancelar: todo debe conservarse.
2. Cargar la plantilla: deben aparecer noviembre y diciembre de 2026, con IVA 13000/6500 y totales 113000/56500. El proveedor debe conservarse.
3. Cambiar IVA, total o vaciar un dato en el Excel: el aviso debe indicar la fila, sin cambiar el formulario.
4. Repetir un mes/año y verificar el rechazo.
5. Probar un libro con varias hojas y seleccionar la correcta.
6. Modificar los desgloses importados en EXT4 y confirmar con datos de prueba autorizados. Verificar ambas filas y la bitácora en MySQL.

La prueba de lectura y validación local cubre archivos XLSX y XLS, fórmulas con resultados guardados, porcentajes, campos vacíos, rangos, duplicados y el límite de filas. La integración completa con WordPress y el WCF se verifica en el equipo del proyecto.


## Diseño institucional — versión 0.3.0

EXT4 y EXT5 utilizan una plantilla propia con la paleta de EXT2/EXT3:
azul #4e73df, verde #1cc88a, gris de fondo #f8f9fc y tarjetas blancas.
La cabecera, el logo institucional y el pie siguen el módulo externo.
Los avisos mantienen sus estados verde, amarillo y rojo.
La hoja de estilos del plugin no requiere Bootstrap ni FontAwesome para funcionar.
La página que contiene `[publicasa_registrar_propuesta]` selecciona la plantilla
`templates/pagina.php` automáticamente y conserva la barra de WordPress.
El listado se obtiene de una página publicada con `[publicasa_solicitudes]`;
se da preferencia al `page_id` de la ruta EXT3 configurada. Si no existe,
el encabezado dirige a la portada del sitio.

Los ajustes WSDL y ruta EXT3 se conservan al actualizar. No se modifican las
validaciones, los cálculos, el contrato SOAP ni la importación de Excel.
El logo se incluye desde el módulo externo del mismo repositorio.
