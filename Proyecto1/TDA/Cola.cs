using System;

namespace Proyecto1.TDA
{
    public class Cola<T>
    {
        private NodoCola<T> primero;
        private NodoCola<T> ultimo;
        private int cantidad;

        public int Cantidad
        {
            get { return cantidad; }
        }

        public Cola()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }

        public bool EstaVacia()
        {
            return primero == null;
        }

        public void Encolar(T dato)
        {
            NodoCola<T> nuevo = new NodoCola<T>(dato);

            if (ultimo == null)
            {
                primero = nuevo;
                ultimo = nuevo;
            }
            else
            {
                ultimo.Siguiente = nuevo;
                ultimo = nuevo;
            }

            cantidad++;
        }

        public T Desencolar()
        {
            if (EstaVacia())
            {
                throw new InvalidOperationException("La cola está vacía.");
            }

            T dato = primero.Dato;
            primero = primero.Siguiente;

            if (primero == null)
            {
                ultimo = null;
            }

            cantidad--;

            return dato;
        }
    }
}
