using Microsoft.Extensions.DependencyInjection;

namespace Notification.Service
{
    public static class ServiceProviderHelper
    {
        // Esta propiedad estática se debe establecer en la clase Startup o Program cuando el contenedor se construye
        public static IServiceProvider ServiceProvider { get; set; }

        // Método para obtener el servicio desde el contenedor
        public static T GetService<T>() where T : class
        {
            return ServiceProvider.GetService<T>();
        }
    }
}
