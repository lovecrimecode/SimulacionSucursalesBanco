# **Simulación de Sucursales de banco**

## Materia: 
Programación Paralela
 
## Integrantes del equipo: Grupo 1D

```
| Zelidee Güémez Henríquez      | 2023-1706 |
| Jesus Elias Diaz Feliz        | 2023-1683 |
| Jayfry Manuel	Guerrero Váldez | 2023-1107 |
| Gianrobert Ramirez Dominguez  | 2023-1743 |
```

### Nombre del líder:
Zelidee Güémez

### Fecha de entrega: 20-ago.-2025

---

## Índice

1. [Introducción](#introducción)
2. [Descripción del Problema](#descripción-del-problema)
3. [Cumplimiento de los Requisitos del Proyecto](#cumplimiento-de-los-requisitos-del-proyecto)
4. [Diseño de la Solución](#diseño-de-la-solución)
5. [Implementación Técnica](#implementación-técnica)
6. [Evaluación de Desempeño](#evaluación-de-desempeño)
7. [Trabajo en Equipo](#trabajo-en-equipo)
8. [Conclusiones](#conclusiones)
9. [Referencias](#referencias)
10. [Anexos](#anexos)

	---
	
# **1. Introducción**

## Presentación general del proyecto

El proyecto consiste en el desarrollo de una **simulación computacional de un sistema bancario multi-sucursal**, diseñado para modelar el funcionamiento paralelo de **ventanillas físicas** y **cajeros automáticos** en la atención de clientes. Cada cliente posee una cuenta de ahorro (personal) o corriente (empresarial), y puede realizar operaciones como depósitos, retiros o consultas de saldo.

La aplicación ha sido implementada como un **sistema de consola en C#**, empleando **programación concurrente** mediante hilos (*threads*) para emular la atención simultánea en distintos puntos de servicio. El sistema gestiona **recursos compartidos** (como colas de clientes y fondos de cada sucursal) y permite **comparar distintas estrategias de paralelización**, evaluar **escalabilidad** y medir **rendimiento**, con el objetivo de optimizar tiempos de atención y uso de recursos.

## **Justificación del tema elegido**

La atención en entidades financieras es un problema recurrente en la vida real, donde **la gestión eficiente de filas y recursos** impacta directamente en la satisfacción del cliente y en la productividad de la institución. Modelar este comportamiento mediante programación paralela permite **probar estrategias de distribución de carga** sin necesidad de implementarlas en un entorno real, reduciendo costos y riesgos. Además, este tipo de simulaciones tiene aplicaciones directas en otros sectores como supermercados, oficinas gubernamentales o centros de servicio al cliente.

## **Objetivos**

### **Objetivo general:**

Desarrollar una simulación bancaria multi-sucursal con capacidad de atención simultánea, administración de recursos compartidos y análisis de rendimiento, utilizando técnicas de programación paralela en C#.

### **Objetivos específicos:**

1. Implementar un modelo concurrente que represente ventanillas y cajeros como hilos independientes.
2. Diseñar mecanismos seguros para el manejo de datos compartidos (fondos y colas de clientes).
3. Analizar y comparar el rendimiento de diferentes estrategias de paralelización (FIFO, prioridad, ventanillas especializadas o mixtas).
4. Evaluar la escalabilidad del sistema aumentando recursos y volumen de clientes.
5. Obtener métricas cuantitativas de desempeño.

---

## **2. Descripción del Problema**

En un banco real, múltiples clientes esperan en filas para ser atendidos en ventanillas o cajeros automáticos. Cada punto de atención funciona de forma simultánea, pero comparte recursos como el fondo de efectivo de la sucursal o la cola de espera. Una mala distribución de estos recursos puede provocar **tiempos de espera elevados, congestión en ciertos puntos y baja eficiencia global**.

### Contexto del problema seleccionado

Se recrea el flujo de atención en sucursales bancarias, con múltiples ventanillas y cajeros, procesando transacciones en paralelo.

### Aplicación del problema en un escenario real

El modelo puede ser utilizado por bancos u otras instituciones para **probar estrategias de distribución de clientes** y detectar configuraciones más eficientes antes de aplicarlas en entornos físicos.

### Importancia del paralelismo en la solución

El uso de hilos permite simular operaciones concurrentes y medir su impacto en métricas como tiempo promedio de espera y número de transacciones procesadas.

---

## **3. Cumplimiento de los Requisitos del Proyecto**

### **Ejecución simultánea de múltiples tareas:**

- Cada ventanilla y cajero automático es implementado como un hilo independiente.
- Esto permite que múltiples clientes sean atendidos de forma paralela, emulando el funcionamiento real de un banco donde varios puntos de servicio operan al mismo tiempo.

### **Necesidad de compartir datos entre tareas:**

- Los hilos comparten estructuras como la **cola de clientes** y el **fondo total de la sucursal**.
- Se utilizan **mecanismos de sincronización para evitar condiciones de carrera** al modificar el saldo o gestionar el turno de cada cliente.

### **Exploración de diferentes estrategias de paralelización:**

Se comparan enfoques:
- **FIFO**: Atiende clientes en orden de llegada.
- **Prioridad**: Da preferencia a clientes preferenciales.
- **Mixta**: Combina FIFO y prioridad, dando un 75% de prioridad a clientes preferenciales. Esto permite evaluar cómo varía el rendimiento según la forma de asignar clientes a recursos.

Esto permite evaluar cómo varía el rendimiento según la forma de asignar clientes a recursos.

### **Escalabilidad con más recursos:**

- El sistema permite aumentar la cantidad de sucursales, ventanillas y cajeros, así como el volumen de clientes.
- Esto facilita pruebas de carga para medir si el rendimiento se mantiene o mejora al incrementar los hilos disponibles.

### **Métricas de evaluación del rendimiento:**

- Se registran indicadores como el tiempo promedio de espera por cliente, tiempo promedio de servicio, speedup, eficiencia, transacciones por hora, tasa de éxito y uso promedio de cada punto de atención.
- Estos datos permiten comparaciones objetivas entre estrategias y escenarios.

### **Aplicación a un problema del mundo real:**

- El modelo simula con precisión un entorno bancario, replicando dinámicas de atención, colas y gestión de recursos críticos.
- Es adaptable a otros entornos con problemas similares de concurrencia y distribución de carga.

---

## **4. Diseño de la Solución**

### Arquitectura General del Sistema

La solución se basa en un modelo orientado a objetos donde cada entidad del sistema se representa mediante clases:

- **Cliente**: Contiene datos personales, tipo de cuenta, saldo y transacción solicitada.
- **Cuenta**: Maneja operaciones básicas (depósito, retiro, consulta).
- **Sucursal**: Administra su fondo total y las colas de clientes (normales y preferenciales).
- **Ventanilla y Cajero**: Implementan la lógica de atención concurrente.
- **Simulador**: Orquesta la ejecución, creación de hilos y recopilación de métricas.
- **CalculadoraMetricas**: Calcula métricas como tiempo de espera, speedup, eficiencia y transacciones por hora.
- **Estrategias**: Definen cómo se gestionan las colas (FIFO, Prioridad, Mixta).


### Diagrama de Componentes y Tareas Paralelas
En carpeta: `docs/`

### Estrategia de Paralelización utilizada

- Hilos para cada ventanilla y cajero, gestionados por `Thread` y `ThreadPool`.
- Sincronización con `lock`, `Interlocked`, `BlockingCollection` y `CancellationTokenSource` para coordinar detención.
- Estrategias configurables: FIFO, Prioridad, Mixta.

### Herramientas y Tecnologías empleadas

- **Lenguaje:** C# 
- **Concurrencia:** `Threads`, `System.Threading`, `System.Collections.Concurrent`.
- **Control de versiones:** GitHub
- **Pruebas**: xUnit
  
---

## **5. Implementación Técnica**

### Descripción de la estructura del proyecto

Organizada en carpetas `/src`, `/docs`, `/tests` y `/metrics` para separación de código, documentación, pruebas y resultados.

```
SimulacionSucursalesBanco/
│
├── /src
│   ├── clases/
│   │    ├── Ventanilla.cs
│   │    ├── Cajero.cs
│   │    ├── Sucursal.cs
│   │    ├── Cuenta.cs
│   │    ├── Cliente.cs
│   │    ├── Simulador.cs
│   │    └── Transaccion.cs    
│   ├── main.cs                  
│   ├── CalculadoraMetricas.cs 
│   ├── EstrategiaFIFO.cs
│   ├── EstrategiaMixta.cs
│   ├── EstrategiaPrioridad.cs
│   ├── IEstrategiaAtencion.cs
│
├── /tests
│   ├── TestCola.cs
│   ├── TestTransacciones.cs
│   ├── TestConcurrencia.cs
│   ├── TestUtils.cs
│   ├── PruebasUnitarias.cs
│
├── /metrics
│
├── /docs
│   ├── especificacion.md
│   ├── manual_usuario.md
│   ├── escenarios_simulacion.md
│   └── diagrama_UML.png
│
├── Program.cs    
├── SimulacionSucursalesBanco.csproj
├── README.md
└── .gitignore
```

### Explicación del código clave
- **main.cs**: Interfaz de consola para configurar y ejecutar la simulación.
- **Simulador.cs**: Gestiona hilos, genera clientes con retraso fijo de 10ms y recopila métricas.
- **CalculadoraMetricas.cs**: Calcula métricas avanzadas (speedup, eficiencia, transacciones/hora).
- **Sucursal.cs**: Valida fondos y gestiona colas con `BlockingCollection`.
- *Estrategias*: Implementan `IEstrategiaAtencion` para FIFO, Prioridad y Mixta (75% prioridad).

### Uso de mecanismos de sincronización
- `lock` para proteger `_fondos` en `Sucursal.cs`.
- `Interlocked` para contadores de métricas.
- `BlockingCollection` para colas thread-safe.
- `CancellationTokenSource` para coordinar detención de hilos.

### Justificación técnica de las decisiones tomadas

- Se eligió C# por su manejo robusto de la concurrencia y la facilidad para implementar estructuras seguras.
- Se usó consola para poder centrarse en el modelo lógico y no en la interfaz gráfica, priorizando rendimiento y simplicidad de pruebas.

---

## **6. Evaluación de Desempeño**
###	Comparativa entre ejecución secuencial y paralela
###	Métricas: tiempo de ejecución, eficiencia, escalabilidad
###	Gráficas o tablas con resultados
###	Análisis de cuellos de botella o limitaciones

---

## **7. Trabajo en Equipo**

### Descripción del reparto de tareas

  #### Dev 1: Gianrobert – Entidades y operaciones bancarias

**Tareas principales:**
- Diseñar y programar las clases `Cliente`, `Cajero`, `Simulador`, `Sucursal` y `Ventanilla`.
- Implementar la lógica de depósitos, retiros y consultas de saldo.
- Garantizar la correcta sincronización del saldo de la sucursal.

**Documentos trabajados:**
- `/src/Cliente.cs`
- `/src/Cajero.cs`
- `/src/Simulador.cs`
- `/src/Sucursal.cs`
- `/src/Ventanilla.cs`


 #### Dev 2: Jesus Elias – Concurrencia y gestión de hilos

**Tareas principales:**
- Diseñar y programar las clases `Cuenta` y `Transaccion`.
- Implementar hilos para ventanillas y cajeros.
- Crear y manejar colas de clientes compartidas.
- Controlar la concurrencia en la atención.

**Documentos trabajados:**
- `/src/Cuenta.cs`
- `/src/Transaccion.cs`

*Co-Authored-by:*
- `/src/Cliente.cs`
- `/src/Cajero.cs`
- `/src/Simulador.cs`
- `/src/Sucursal.cs`
- `/src/Ventanilla.cs`


  #### Dev 3: Jayfry – Estrategias de atención y métricas

**Tareas principales:**
- Programar estrategias de atención: FIFO, prioridad, ventanillas mixtas o especializadas.
- Calcular métricas: tiempo promedio, tasa de éxito y transacciones por hora.
- Implementar funciones para comparar estrategias.

**Documentos trabajados:**
- `/src/EstrategiaFIFO.cs`
- `/src/EstrategiaPrioridad.cs`
- `/metrics/resultados_fifo.txt`


 #### Dev 4: Zelidee – Pruebas, simulación paralela y gestión de Git

**Tareas principales:**
- Diseñar pruebas unitarias para entidades, concurrencia y estrategias.
- Simular escenarios con diferentes cantidades de clientes y sucursales.
- Documentar hallazgos y mantener actualizado el archivo `README.md`.
- Manejar el repositorio y los commits del proyecto.

**Documentos trabajados:**
- `/tests/test_transacciones.cs`
- `/tests/test_concurrencia.cs`
- `/tests/TestUtils.cs`
- `/docs/manual_usuario.md`
- `/docs/especificacion.md`
- `/docs/escenarios_simulacion_.md`
- `/docs/resultados_pruebas.txt`
- `readme.md`
- `/src/main.cs`

### Herramientas utilizadas para coordinación

GitHub para manejo de repositorio y tareas por del proyecto por integrante.

---

## **8. Conclusiones**

### Principales aprendizajes técnicos
- Implementación de concurrencia con hilos y sincronización.
- Diseño de estrategias de atención para optimizar colas.
- Cálculo de métricas avanzadas para evaluar paralelismo.

### Retos enfrentados y superados


### Posibles mejoras o líneas futuras

---

## **9. Referencias**

### Fuentes bibliográficas, técnicas o académicas consultadas

---

## **10. Anexos**

### Manual de ejecución del sistema
Ir a: `docs/manual_usuario.md`

### Capturas adicionales, pruebas complementarias
Ir a: `tests/` y `docs/`

### Enlace al repositorio de Git (público): 
https://github.com/lovecrimecode/SimulacionSucursalesBanco
