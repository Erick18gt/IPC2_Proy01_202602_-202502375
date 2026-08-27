using Proyecto1.Modelos;
using Proyecto1.TDA;

namespace Proyecto1.Servicios
{
    public class ResultadoExtraccion
    {
        public Lista<Celda> Camino { get; set; }
        public int CapacidadFinal { get; set; }

        public ResultadoExtraccion(Lista<Celda> camino, int capacidadFinal)
        {
            Camino = camino;
            CapacidadFinal = capacidadFinal;
        }
    }
}
