# GestionFacturas

Proyecto de gestión de facturas que utiliza el framework de desarrollo de Angular con backend .NET.

### Consideraciones de uso

- **Configurar archivo "appsettings.json":** Dentro de la carpeta del Backend remplazar *"DbGestionFacturas"*, usar la conexión que se vaya a utilizar para la base de datos.
- **Uso de proxy para pruebas:** La aplicación de angular solo se ha provado en el desarrollo con la configuración del proxy dentro de *"proxy.conf.json"* (con herramientas de angular).

---

- **Testing de Frontend:** Para el Frontend se han utilizado la herramienta de Jasmin para la ejecucción de pruebas y Istanbul para verificar la cobertura del código.

- **Testing de Backend:** Para el Backend se han realizado las pruebas con XUnit, Moq y herramientas de Microsoft.
