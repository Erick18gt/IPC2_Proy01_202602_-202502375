using Proyecto1.TDA;

namespace Proyecto1.Servicios
{
    // Representa un "estado" durante la búsqueda de extracción: en qué celda estoy
    // y cuánta capacidad de combate me queda. No podemos usar solo NodoMatriz.Padre
    // porque la misma celda puede visitarse con distinta capacidad restante según
    // el camino tomado, y aquí guardamos la cadena de padres propia de la búsqueda.
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
