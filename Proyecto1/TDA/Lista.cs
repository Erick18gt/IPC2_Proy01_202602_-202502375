using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto1.TDA
{
    public class Lista<T>
    {
        private Nodo<T> primero;
        private int cantidad;
        public int Cantidad {
            get { return cantidad; }
        }
        public Lista()
        {
            primero = null;
            cantidad = 0;
        }
        public void Agregar(T dato) {
            Nodo<T> nuevo = new Nodo<T>(dato);
            if (primero == null)
            {
                primero = nuevo;
            }
            else
            {
                Nodo<T> actual = primero;

                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }

                actual.Siguiente = nuevo;
            }

            cantidad++;

        }
        public T Obtener(int indice)
        {
            if (indice < 0 || indice >= cantidad)
            {
                throw new IndexOutOfRangeException("Índice fuera de rango.");
            }

            Nodo<T> actual = primero;

            for (int i = 0; i < indice; i++)
            {
                actual = actual.Siguiente;
            }

            return actual.Dato;
        }
        public bool EstaVacia()
        {
            return primero == null;
        }
        public void Eliminar(int indice)
        {
            if (indice < 0 || indice >= cantidad)
            {
                throw new IndexOutOfRangeException("Índice fuera de rango.");
            }

            if (indice == 0)
            {
                primero = primero.Siguiente;
            }
            else
            {
                Nodo<T> anterior = primero;

                for (int i = 0; i < indice - 1; i++)
                {
                    anterior = anterior.Siguiente;
                }

                anterior.Siguiente = anterior.Siguiente.Siguiente;
            }

            cantidad--;
        }

    }

}
