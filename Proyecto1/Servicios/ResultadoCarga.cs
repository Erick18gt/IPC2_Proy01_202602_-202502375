using Proyecto1.Modelos;
using Proyecto1.TDA;

namespace Proyecto1.Servicios
{
    // Contenedor con lo que se extrae de UN archivo de configuración.
    // SistemaControl decide después cómo fusionarlo con lo que ya tenía cargado.
    public class ResultadoCarga
    {
        public Lista<Ciudad> Ciudades { get; set; }
        public Lista<Robot> Robots { get; set; }

        public ResultadoCarga()
        {
            Ciudades = new Lista<Ciudad>();
            Robots = new Lista<Robot>();
        }
    }
}
