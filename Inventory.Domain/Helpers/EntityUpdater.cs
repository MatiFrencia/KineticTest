using System.Reflection;

namespace Inventory.Domain.Helpers
{
	public static class EntityUpdater
	{
		public static (T UpdatedEntity, bool HasChanges) UpdateChangedFields<T>(T actualEntity, T newEntity)
		{
			if (actualEntity == null || newEntity == null)
				throw new ArgumentNullException("Los objetos no pueden ser nulos.");

			bool hasChanges = false;

			// Obtén las propiedades como un Span<T> para evitar asignaciones adicionales.
			var properties = new Span<PropertyInfo>(typeof(T).GetProperties());

			foreach (var property in properties)
			{
				// Solo considera las propiedades que se pueden leer y escribir.
				if (property.CanRead && property.CanWrite)
				{
					var actualValue = property.GetValue(actualEntity);
					var newValue = property.GetValue(newEntity);

					// Compara valores y actualiza si son diferentes y el valor nuevo no es el valor por defecto.
					if (!Equals(actualValue, newValue) && !IsDefaultValue(newValue))
					{
						property.SetValue(actualEntity, newValue);
						hasChanges = true;
					}
				}
			}

			// Devuelve la entidad actualizada y el estado de cambios como un ValueTuple.
			return (actualEntity, hasChanges);
		}

		// Método para verificar si un valor es el valor por defecto de su tipo.
		private static bool IsDefaultValue(object value)
		{
			return value == null || EqualityComparer<object>.Default.Equals(value, GetDefaultValue(value.GetType()));
		}

		// Método que obtiene el valor por defecto para un tipo dado.
		private static object GetDefaultValue(Type type)
		{
			return type.IsValueType ? Activator.CreateInstance(type) : null;
		}
	}
}
