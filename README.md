# **Simulación de Sucursales de banco**

# Programación Paralela

## Integrantes del equipo:

```
| Zelidee Güémez Henríquez      | 2023-1706 |
| Jesus Elias Diaz Feliz        |  |
| Jayfry Manuel	Guerrero Váldez |  |
| Gianrobert Ramirez Dominguez  |  |

```

## Nombre del líder:

Zelidee Guemez

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

- Desarrollar una simulación bancaria multi-sucursal con capacidad de atención simultánea, administración de recursos compartidos y análisis de rendimiento, utilizando técnicas de programación paralela en C#.

### **Objetivos específicos:**

1. Implementar un modelo concurrente que represente ventanillas y cajeros como hilos independientes.
2. Diseñar mecanismos seguros para el manejo de datos compartidos (fondos y colas de clientes).
3. Analizar y comparar el rendimiento de diferentes estrategias de paralelización (FIFO, prioridad, ventanillas especializadas o mixtas).
4. Evaluar la escalabilidad del sistema aumentando recursos y volumen de clientes.
5. Obtener métricas cuantitativas de desempeño para identificar cuellos de botella y proponer mejoras.

## **Descripción del Problema**

En un banco real, múltiples clientes esperan en filas para ser atendidos en ventanillas o cajeros automáticos. Cada punto de atención funciona de forma simultánea, pero comparte recursos como el fondo de efectivo de la sucursal o la cola de espera. Una mala distribución de estos recursos puede provocar **tiempos de espera elevados, congestión en ciertos puntos y baja eficiencia global**.

### Contexto del problema seleccionado

Se recrea el flujo de atención en sucursales bancarias, con múltiples ventanillas y cajeros, procesando transacciones en paralelo.

### Aplicación del problema en un escenario real

**Aplicación real:** El modelo puede ser utilizado por bancos u otras instituciones para **probar estrategias de distribución de clientes** y detectar configuraciones más eficientes antes de aplicarlas en entornos físicos.

### Importancia del paralelismo en la solución

**Importancia del paralelismo:** El uso de hilos permite simular operaciones concurrentes y medir su impacto en métricas como tiempo promedio de espera, número de transacciones procesadas y uso de recursos, algo fundamental en escenarios de alta concurrencia.

## **Cumplimiento de los Requisitos del Proyecto**

El presente proyecto cumple con los criterios solicitados para un sistema concurrente de simulación, de la siguiente manera:

### **Ejecución simultánea de múltiples tareas:**

- Cada ventanilla y cajero automático es implementado como un hilo independiente, con su propio ciclo de atención.
- Esto permite que múltiples clientes sean atendidos de forma paralela, emulando el funcionamiento real de un banco donde varios puntos de servicio operan al mismo tiempo.

### **Necesidad de compartir datos entre tareas:**

- Los hilos comparten estructuras como la **cola de clientes** y el **fondo total de la sucursal**.
- Se utilizan mecanismos de sincronización para evitar condiciones de carrera al modificar el saldo o gestionar el turno de cada cliente.

### **Exploración de diferentes estrategias de paralelización:**

- Se comparan enfoques como *FIFO* (First In, First Out), atención por prioridad (clientes preferenciales) y ventanillas especializadas frente a ventanillas mixtas.
- Esto permite evaluar cómo varía el rendimiento según la forma de asignar clientes a recursos.

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

---

### Herramientas y Tecnologías empleadas

- **Lenguaje**: C# (.NET)

## **Implementación Técnica**

### Descripción de la estructura del proyecto

Organizada en carpetas `/src`, `/docs`, `/tests` y `/metrics` para separación de código, documentación, pruebas y resultados.

```
SimulacionSucursalesBanco/
│
├── /docs                 # Documentación del proyecto
│    ├── especificacion.md
│    ├── manual_usuario.md
│    ├── diagrama_clases.png
│    └── diagrama_flujo.png
│
├── /src                  # Código fuente
│    ├── main.cs          # Programa principal
│    ├── Cajero.cs
│    ├── Ventanilla.cs
│    ├── Cliente.cs
│    ├── Sucursal.cs
│    └── Simulador.cs
│
├── /tests                # Pruebas unitarias
│    ├── test_colas.cs
│    ├── test_transacciones.cs
│    └── test_multiproceso.cs
│
├── /metrics              # Resultados de comparativas
│    ├── estrategia_fifo.txt
│    ├── estrategia_prioridad.txt
│    └── comparativa_escalabilidad.xlsx
│
├── README.md
└── .gitignore

```

### Explicación del código clave

---

### Uso de mecanismos de sincronización

### Justificación técnica de las decisiones tomadas

- Se eligió C# por su manejo robusto de la concurrencia y la facilidad para implementar estructuras seguras.
- Se usó consola para poder centrarse en el modelo lógico y no en la interfaz gráfica, priorizando rendimiento y simplicidad de pruebas.

## **Evaluación de Desempeño**

### Comparativa entre ejecución secuencial y paralela

### Métricas: tiempo de ejecución, eficiencia, escalabilidad

### Gráficas o tablas con resultados

### Análisis de cuellos de botella o limitaciones

## **Implementación Técnica**

## **Evaluación de Desempeño**

## **Trabajo en Equipo**

### Descripción del reparto de tareas

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

### Capturas adicionales, pruebas complementarias

### Enlace al repositorio de Git (público): https://github.com/lovecrimecode/SimulacionSucursalesBanco

# Control y Evaluación

## Reglas de Entrega en Git (2ptos):

- Repositorio público: https://github.com/lovecrimecode/SimulacionSucursalesBanco

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

### **Cada miembro** debe:

Subir al menos 3 *commits significativos* (implementación o documentación) en las semanas de clases.

### Usar su nombre en los *commits,* identificar con Co-authored-by-<nombreApellidoEnPascalCase>

## Evaluación del trabajo en equipo (3ptos)

Cada elección debe ser una escala entre: No satisfactorio, Satisfactorio, Bueno, Muy bueno

**Por el líder:**

- Evalúa a cada miembro en 3 aspectos:
    - Compromiso y responsabilidad
    - Calidad de su código / contribuciones
    - Colaboración y comunicación

**Entre miembros:**

- Cada miembro evalúa a los otros según:
    - Ayuda en resolución de problemas
    - Participación en decisiones
    - Claridad en la comunicación

**Miembros al líder:**

- Evalúan en:
    - Organización y delegación de tareas
    - Resolución de conflictos
    - Apoyo técnico y emocional

# Exposición. 10ptos.

| **Criterio** | **Descripción** | **Puntos** |
| --- | --- | --- |
| **1. Claridad en la explicación del problema (tema elegido)** | El equipo explica claramente el contexto, la importancia del problema y por qué fue elegido. | 1 |
| **2. Dominio técnico del tema** | Se demuestra comprensión profunda del paralelismo, sincronización, y estrategias aplicadas en su solución. | 1 |
| **3. Presentación del diseño y arquitectura** | El diseño está bien explicado, visualmente apoyado (diagrama), y se entienden las decisiones técnicas. | 1 |
| **4. Demostración del funcionamiento del sistema** | El código se muestra funcionando, se explican los fragmentos relevantes y hay claridad en la ejecución. | 5 |
| **5. Interpretación de resultados y métricas** | Se explican los resultados de forma clara con comparativas significativas (secuencial vs paralela). | 1 |
| **6. Gestión del tiempo** | La exposición se ajusta al tiempo máximo sin omitir ni extender innecesariamente. | 1 |

El líder del equipo debe asegurar el cumplimiento de cada sección descrita en este documento.