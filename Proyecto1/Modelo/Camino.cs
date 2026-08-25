using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto1.Modelo
{
    public class Camino
    {
        public int Fila { get; set; }
        public int Columna { get; set; }

        public Camino(int fila, int columna)
        {
            Fila = fila;
            Columna = columna;
        }
    }
}
