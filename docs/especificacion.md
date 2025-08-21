
# Simulación Bancaria Multi‑Sucursal

Este sección describe el diseño de concurrencia en el proyecto **Simulación Bancaria Multi‑Sucursal**, con énfasis en la gestión de hilos, sincronización y detención ordenada. Las clases principales son `Sucursal.cs`, `Simulador.cs`, `Ventanilla.cs` y `Cajero.cs`.

---

## 1. Exclusión mutua con `lock`

En entidades como **Cuenta** y **Transaccion**, utilizamos `lock` para proteger operaciones críticas que modifican el estado compartido (ejemplo: saldo de una cuenta). Esto garantiza exclusión mutua y evita condiciones de carrera.

```csharp
public void Debitar(decimal monto)
{
    lock (_lockSaldo)
    {
        if (_saldo >= monto)
            _saldo -= monto;
    }
}
```

🔹 **Beneficio:** ningún hilo puede debitar y acreditar al mismo tiempo de la misma cuenta, evitando inconsistencias.

---

## 2. Operaciones atómicas con `Interlocked`

En **Sucursal.cs**, se emplea `Interlocked` para actualizar métricas (procesados, éxitos, fallos, tiempos acumulados). Ejemplo:

```csharp
Interlocked.Increment(ref _procesados);
Interlocked.Add(ref _tiempoServicioAcumMs, servicioMs);
```

🔹 **Beneficio:** garantiza que múltiples hilos (ventanillas y cajeros) actualicen contadores sin necesidad de `lock`, logrando eficiencia y seguridad.

---

## 3. Comunicación productor–consumidor con `BlockingCollection`

Las sucursales gestionan **colas de clientes** con `BlockingCollection`, tanto para ventanillas como para cajeros:

```csharp
private readonly BlockingCollection<Cliente> _colaVentanilla;
private readonly BlockingCollection<Cliente> _colaCajero;
```

* **Productores:** generador de clientes en `Simulador` llama a `EncolarCliente`.
* **Consumidores:** hilos de `Ventanilla` y `Cajero` toman clientes con `Take(ct)`.

🔹 **Beneficio:** `BlockingCollection` bloquea de forma natural cuando la cola está vacía, evitando *busy waiting* y simplificando el modelo de concurrencia.

---

## 4. Cancelación cooperativa con `CancellationTokenSource`

El **Simulador** crea un `CancellationTokenSource` y pasa su `Token` a ventanillas y cajeros. Esto permite detener la simulación de forma ordenada:

```csharp
while (!_ct.IsCancellationRequested)
{
    Cliente? cliente = _sucursal.TomarClienteVentanilla(_estrategia, _ct);
    if (cliente == null) continue;
    // ... procesar transacción ...
}
```

Cuando expira la duración configurada, el `Simulador` invoca:

```csharp
_cts.Cancel();
```

Los hilos detectan la cancelación y finalizan limpiamente.

🔹 **Beneficio:** evita hilos colgados y garantiza cierre controlado de la simulación.

---

## 5. Ejecución concurrente de puntos de atención

* **`Ventanilla.cs`**: hilos más lentos (50–250 ms) que simulan atención presencial.
* **`Cajero.cs`**: hilos más rápidos (40–180 ms) que simulan autoservicio.
* Ambos ejecutan la transacción del cliente y registran métricas en `Sucursal` de manera atómica.

Ejemplo de registro concurrente:

```csharp
_sucursal.RegistrarResultado(cliente, exito, PuntoAtencion.Ventanilla, servicioMs);
```

---

## 6. Prevención de condiciones de carrera

* **Saldo de cuentas**: protegido con `lock` → evita inconsistencias financieras.
* **Métricas de sucursal**: protegidas con `Interlocked` → garantiza exactitud de estadísticas.
* **Colas de clientes**: gestionadas con `BlockingCollection` → evita accesos simultáneos peligrosos.
* **Finalización de hilos**: controlada con `CancellationTokenSource` → evita hilos zombis.

---

## 7. Paralelismo vs Ejecución Secuencial

El uso de múltiples hilos en ventanillas y cajeros permite que **decenas de clientes sean atendidos en paralelo**, en lugar de procesarse uno a uno de forma secuencial.

* **Secuencial:** un cliente termina su transacción antes de que el siguiente pueda comenzar, lo que genera largas colas.
* **Paralelo con hilos:** varios clientes son atendidos simultáneamente, reduciendo tiempos de espera promedio y aumentando el número de operaciones procesadas por unidad de tiempo.

🔹 **Ejemplo práctico:** con 4 cajeros y 2 ventanillas, se pueden atender hasta 6 clientes al mismo tiempo. En un modelo secuencial, solo se atendería **1 cliente a la vez**.

**Beneficio directo:** mejor rendimiento, mayor escalabilidad y simulaciones más realistas.

---

## 8. Resumen

La combinación de técnicas garantiza:

* **Seguridad de concurrencia**: datos consistentes aún con múltiples hilos.
* **Escalabilidad**: ventanillas y cajeros operan en paralelo sin bloqueos innecesarios.
* **Robustez**: la simulación puede detenerse de manera predecible.
* **Rendimiento mejorado**: paralelismo permite simular mayor carga de clientes en menor tiempo.

En conclusión, el diseño implementa un patrón productor–consumidor concurrente, con métricas atómicas y sincronización en recursos críticos, logrando un simulador bancario confiable, extensible y eficiente.

