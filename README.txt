Buenas!!

Instrucciones para ejecutar la app:
HAY 2 OPCIONES:
Ejecutar directamente con visual studio:
1. Hacer un git clone: git clone https://github.com/MatiFrencia/KineticTest.git
2. Abrir el archivo "KineticTest.sln"
3. Seleccionar como Proyecto de Inicio el docker-compose
4. Darle Play y esperar 1 minuto.
5. Para ver el swagger: poner en el navegador "https://localhost:5001/swagger/index.html" 
6. Listo!


Ejecutar directamente con Docker-Compose:
1. Hacer un git clone: git clone https://github.com/MatiFrencia/KineticTest.git
2. Abrir powershell o cmd en el directorio donde se encuentra la sln
3. Ejecutar  docker-compose up --build
4. Esperar 1 minuto
5. Para ver el swagger: poner en el navegador "https://localhost:5001/swagger/index.html" 
6. Listo!



PD: Deje el handler de CreateProductEvent de Notification.Service con un throw Exception, para que rompa y puedan testear lo del CircuitBreaker y el manejo de resiliencia,
los eventos que dan error se guardan en una tabla y los que funcionaron correctamente en otra. Si envían muchas peticiones de CreateProductEvent, van a poder visualizar
el comportamiento haciendo un select de esas dos tablas:

SELECT * FROM [NotificationDB].[dbo].[FailedEventLogs]
SELECT * FROM [NotificationDB].[dbo].[EventLogs]

Van a poder visualizar como deja de logear los FailedEvent ya que el circuito se abre y apenas vuelve a cerrarse los sigue logeando.


Espero sea lo que esperaban!!
Saludos!!