# **Simulación de Sucursales de banco**

# Programación Paralela

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
 
## Integrantes del equipo: Grupo 1D

```
| Zelidee Güémez Henríquez      | 2023-1706 |
| Jesus Elias Diaz Feliz        | 2023-1683 |
| Jayfry Manuel	Guerrero Váldez | 2023-1107 |
| Gianrobert Ramirez Dominguez  | 2023-1743 |
```

## Nombre del líder:
Zelidee Güémez

### Fecha de entrega: 20-ago.-2025

---

# **Introducción**

## Presentación general del proyecto

El presente proyecto consiste en el desarrollo de una **simulación computacional de un sistema bancario multi-sucursal**, diseñado para modelar el funcionamiento paralelo de **ventanillas físicas** y **cajeros automáticos** en la atención de clientes. Cada cliente posee una cuenta de ahorro (personal) o corriente (empresarial), y puede realizar operaciones como depósitos, retiros o consultas de saldo.

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
5. Obtener métricas cuantitativas de desempeño para identificar cuellos de botella.

## **Descripción del Problema**

En un banco real, múltiples clientes esperan en filas para ser atendidos en ventanillas o cajeros automáticos. Cada punto de atención funciona de forma simultánea, pero comparte recursos como el fondo de efectivo de la sucursal o la cola de espera. Una mala distribución de estos recursos puede provocar **tiempos de espera elevados, congestión en ciertos puntos y baja eficiencia global**.

### Contexto del problema seleccionado

Se recrea el flujo de atención en sucursales bancarias, con múltiples ventanillas y cajeros, procesando transacciones en paralelo.

### Aplicación del problema en un escenario real

**Aplicación real:** El modelo puede ser utilizado por bancos u otras instituciones para **probar estrategias de distribución de clientes** y detectar configuraciones más eficientes antes de aplicarlas en entornos físicos.

### Importancia del paralelismo en la solución

**Importancia del paralelismo:** El uso de hilos permite simular operaciones concurrentes y medir su impacto en métricas como tiempo promedio de espera y número de transacciones procesadas.

## **Cumplimiento de los Requisitos del Proyecto

1. **Ejecución simultánea de múltiples tareas:**  
   Ventanillas y cajeros son hilos independientes, atendiendo clientes en paralelo.

2. **Necesidad de compartir datos entre tareas:**  
   Los hilos comparten la cola de clientes y el fondo de la sucursal, usando sincronización para evitar condiciones de carrera.

3. **Exploración de diferentes estrategias de paralelización:**  
   Se implementan y comparan FIFO, prioridad y atención mixta.

4. **Escalabilidad con más recursos:**  
   El sistema permite aumentar sucursales, ventanillas, cajeros y clientes para pruebas de carga.

5. **Métricas de evaluación del rendimiento:**  
   Se mide tiempo promedio de espera, transacciones por minuto, tasa de éxito y uso de recursos.

6. **Aplicación a un problema del mundo real:**  
   El modelo replica dinámicas bancarias y es adaptable a otros entornos con problemas similares.

--- 

### **Ejecución simultánea de múltiples tareas:**

- Cada ventanilla y cajero automático es implementado como un hilo independiente.
- Esto permite que múltiples clientes sean atendidos de forma paralela, emulando el funcionamiento real de un banco donde varios puntos de servicio operan al mismo tiempo.

### **Necesidad de compartir datos entre tareas:**

- Los hilos comparten estructuras como la **cola de clientes** y el **fondo total de la sucursal**.
- Se utilizan **mecanismos de sincronización para evitar condiciones de carrera** al modificar el saldo o gestionar el turno de cada cliente.

### **Exploración de diferentes estrategias de paralelización:**

Se comparan enfoques:
- *FIFO* (First In, First Out)
- Atención por prioridad (clientes preferenciales)
- Ventanillas especializadas frente a ventanillas mixtas

Esto permite evaluar cómo varía el rendimiento según la forma de asignar clientes a recursos.

### **Escalabilidad con más recursos:**

- El sistema permite aumentar la cantidad de sucursales, ventanillas y cajeros, así como el volumen de clientes.
- Esto facilita pruebas de carga para medir si el rendimiento se mantiene o mejora al incrementar los hilos disponibles.

### **Métricas de evaluación del rendimiento:**

- Se registran indicadores como el tiempo promedio de espera por cliente, transacciones procesadas por minuto, tasa de éxito y uso promedio de cada punto de atención.
- Estos datos permiten comparaciones objetivas entre estrategias y escenarios.

### **Aplicación a un problema del mundo real:**

- El modelo simula con precisión un entorno bancario, replicando dinámicas de atención, colas y gestión de recursos críticos.
- Es adaptable a otros entornos con problemas similares de concurrencia y distribución de carga.

## **Diseño de la Solución**

### Arquitectura General del Sistema

La solución se basa en un modelo orientado a objetos donde cada entidad del sistema se representa mediante clases:

- **Cliente**: contiene datos personales, tipo de cuenta, saldo y transacción solicitada.
- **Cuenta**: maneja operaciones básicas (depósito, retiro, consulta).
- **Sucursal**: administra su fondo total y la cola de clientes.
- **Ventanilla y Cajero**: implementan la lógica de atención concurrente.
- **Simulador**: orquesta la ejecución, creación de hilos y recopilación de métricas.

### Diagrama de Componentes y Tareas Paralelas

---

### Estrategia de Paralelización utilizada

- Hilos para cada ventanilla y cajero.
- Sincronización con locks y estructuras thread-safe.
- Estrategias configurables: FIFO, prioridad, mixta.

### Herramientas y Tecnologías empleadas

- **Lenguaje:** C# (.NET 8)
- **Concurrencia:** Threads, TPL (opcional)
- **Control de versiones:** GitHub

---

## **Implementación Técnica**

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
│   ├── test_cola.cs
│   ├── test_transacciones.cs
│   ├── test_concurrencia.cs
│   ├── TestUtils.cs
│   ├── PruebasUnitarias.cs
│
├── /metrics
│
├── /docs
│   ├── especificacion.md
│   ├── manual_usuario.md
│   ├── escenarios_simulacion.md
│
├── Program.cs    
├── SimulacionSucursalesBanco.csproj
├── README.md
└── .gitignore
```

### Explicación del código clave

### Uso de mecanismos de sincronización

### Justificación técnica de las decisiones tomadas

- Se eligió C# por su manejo robusto de la concurrencia y la facilidad para implementar estructuras seguras.
- Se usó consola para poder centrarse en el modelo lógico y no en la interfaz gráfica, priorizando rendimiento y simplicidad de pruebas.


## **Evaluación de Desempeño**
###	Comparativa entre ejecución secuencial y paralela
###	Métricas: tiempo de ejecución, eficiencia, escalabilidad
###	Gráficas o tablas con resultados
###	Análisis de cuellos de botella o limitaciones


## **Trabajo en Equipo**

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

---

### Herramientas utilizadas para coordinación

GitHub para manejo de repositorio y tareas por del proyecto por integrante.


## **Conclusiones**

### Principales aprendizajes técnicos

### Retos enfrentados y superados

### Posibles mejoras o líneas futuras


## **Referencias**

### Fuentes bibliográficas, técnicas o académicas consultadas


## **Anexos**

### Manual de ejecución del sistema
`docs/manual_usuario.md`

### Capturas adicionales, pruebas complementarias
`tests/`

### Enlace al repositorio de Git (público): https://github.com/lovecrimecode/SimulacionSucursalesBanco


# Control y Evaluación

## Reglas de Entrega en Git (2ptos):

Repositorio público: https://github.com/lovecrimecode/SimulacionSucursalesBanco

### Estructura mínima del repositorio:

```
SimulacionSucursalesBanco/
│
├── /docs                 # Documentación del proyecto
│
├── /src                  # Código fuente
│
├── /tests                # Pruebas unitarias
│
└── /metrics              # Resultados de comparativas

```

## **Repartición de tareas (por roles)**
