namespace Proyecto1.TDA
{
    public class NodoCola<T>
    {
        public T Dato { get; set; }
        public NodoCola<T> Siguiente { get; set; }

        public NodoCola(T dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}
