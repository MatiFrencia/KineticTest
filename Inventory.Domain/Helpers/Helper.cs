namespace Inventory.Domain.Helpers;

public static class Helper
{
    public static DateTime Now => DateTime.UtcNow.AddHours(-3);
}