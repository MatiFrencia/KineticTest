using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure.Data;

public class ServiceHelper
{
    public static IConfiguration? Configuration { get; set; }

    public static void SetConfig(IServiceProvider serviceProvider)
    {
        Configuration = serviceProvider.GetRequiredService<IConfiguration>();
    }
}