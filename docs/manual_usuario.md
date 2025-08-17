# Manual de Usuario

## Descripci�n

Este sistema simula la atenci�n de clientes en sucursales bancarias para comparar estrategias de atenci�n y medir el rendimiento bajo diferentes configuraciones.

## Ejecuci�n del programa

1. **Abrir la aplicaci�n de consola.**
2. **Ingresar los par�metros solicitados:**
   - Cantidad de sucursales (ejemplo: 1, 3, 5)
   - Ventanillas por sucursal (ejemplo: 2, 4)
   - Cajeros por sucursal (ejemplo: 1, 2)
   - Clientes simulados (ejemplo: 20, 50, 100)
   - Duraci�n de la simulaci�n en segundos (ejemplo: 10, 20, 60)
   - Estrategia de atenci�n:
     - 1 = FIFO (orden de llegada)
     - 2 = Prioridad (preferenciales primero)
     - 3 = Mixta (combinaci�n de ambas)

3. **Visualizar los resultados en consola:**
   - M�tricas por sucursal y globales, incluyendo:
     - Tiempo promedio de espera
     - Tiempo promedio de servicio
     - Transacciones procesadas
     - �xitos y fallos
     - Fondos finales
     - Clientes restantes en cola

## Par�metros base

- Sucursales: 1, 3, 5

## Variables de medici�n
- Tiempo promedio de espera
- Transacciones procesadas por minuto
- Uso de CPU (en pruebas controladas)
