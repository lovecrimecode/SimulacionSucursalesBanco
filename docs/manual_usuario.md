# Manual de Usuario

## Descripción

Este sistema simula la atención de clientes en sucursales bancarias para comparar estrategias de atención y medir el rendimiento bajo diferentes configuraciones.

## Ejecución del programa

1. **Abrir la aplicación de consola.**
2. **Ingresar los parámetros solicitados:**
   - Cantidad de sucursales (ejemplo: 1, 3, 5)
   - Ventanillas por sucursal (ejemplo: 2, 4)
   - Cajeros por sucursal (ejemplo: 1, 2)
   - Clientes simulados (ejemplo: 20, 50, 100)
   - Duración de la simulación en segundos (ejemplo: 10, 20, 60)
   - Estrategia de atención:
     - 1 = FIFO (orden de llegada)
     - 2 = Prioridad (preferenciales primero)
     - 3 = Mixta (combinación de ambas)

3. **Visualizar los resultados en consola:**
   - Métricas por sucursal y globales, incluyendo:
     - Tiempo promedio de espera
     - Tiempo promedio de servicio
     - Transacciones procesadas
     - Éxitos y fallos
     - Fondos finales
     - Clientes restantes en cola

## Parámetros base

- Sucursales: 1, 3, 5

## Variables de medición
- Tiempo promedio de espera
- Transacciones procesadas por minuto
- Uso de CPU (en pruebas controladas)
