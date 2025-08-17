namespace SimulacionSucursalesBanco
{
    public class MainApp
    {
        public void IniciarSimulacion()
        {
            int sucursales = 2;
            int ventanillasPorSucursal = 2;
            int cajerosPorSucursal = 2;
            int clientesTotales = 200;
            int duracionSegundos = 20;

            EstrategiaAtencion estrategia = EstrategiaAtencion.FIFO;

            Console.WriteLine("=== Simulación Bancaria Multi-Sucursal ===");
            Console.WriteLine($"Sucursales: {sucursales}, Ventanillas/Sucursal: {ventanillasPorSucursal}, Cajeros/Sucursal: {cajerosPorSucursal}");
            Console.WriteLine($"Estrategia: {estrategia}, Clientes a generar: {clientesTotales}, Duración: {duracionSegundos}s\n");

            var simulador = new Simulador(
                sucursales,
                ventanillasPorSucursal,
                cajerosPorSucursal,
                estrategia,
                clientesTotales,
                TimeSpan.FromSeconds(duracionSegundos));

            simulador.Iniciar();
            simulador.Ejecutar();
            simulador.Detener();

            Console.WriteLine("\n=== Métricas ===");
            simulador.ImprimirMetricasConsola();
        }
    }
}
