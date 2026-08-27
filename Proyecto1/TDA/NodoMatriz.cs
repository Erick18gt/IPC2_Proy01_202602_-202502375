using Proyecto1.Modelos;

namespace Proyecto1.TDA
{
    public class NodoMatriz
    {
        public Celda Dato { get; set; }

        public NodoMatriz Arriba { get; set; }
        public NodoMatriz Abajo { get; set; }
        public NodoMatriz Izquierda { get; set; }
        public NodoMatriz Derecha { get; set; }

        public bool Visitado { get; set; }
        public NodoMatriz Padre { get; set; }

        public NodoMatriz(Celda dato)
        {
            Dato = dato;

            Arriba = null;
            Abajo = null;
            Izquierda = null;
            Derecha = null;

            Visitado = false;
            Padre = null;
        }
    }
}
