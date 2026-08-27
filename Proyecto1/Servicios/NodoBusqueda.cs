using Proyecto1.TDA;

namespace Proyecto1.Servicios
{
 
    public class NodoBusqueda
    {
        public NodoMatriz Nodo { get; set; }
        public int CapacidadRestante { get; set; }
        public NodoBusqueda Padre { get; set; }

        public NodoBusqueda(NodoMatriz nodo, int capacidadRestante, NodoBusqueda padre)
        {
            Nodo = nodo;
            CapacidadRestante = capacidadRestante;
            Padre = padre;
        }
    }
}
