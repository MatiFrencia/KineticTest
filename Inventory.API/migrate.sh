#!/bin/bash

# Espera a que RabbitMQ esté disponible
while ! nc -z rabbitmq 5672; do
  echo "Esperando RabbitMQ..."
  sleep 1
done

# Ejecuta la migración para la base de datos SQLite de Inventory
echo "Ejecutando migraciones de Inventory API..."
dotnet ef database update --project Inventory.Infrastructure

# Ejecuta la migración para la base de datos SQLite de Notification Service (si lo tienes configurado)
echo "Ejecutando migraciones de Notification Service..."
dotnet ef database update --project Notification.Infrastructure

# Finalmente, arranca la aplicación
echo "Arrancando la aplicación..."
exec dotnet Inventory.API.dll
