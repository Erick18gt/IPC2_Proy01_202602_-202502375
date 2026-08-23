using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto1.Modelo
{
    public class UnidadMilitar
    {
        public int Fila { get; set; }
        public int Columna { get; set; }
        public int CapacidadCombate { get; set; }

        public UnidadMilitar(int fila, int columna, int capacidadCombate) {
            Fila = fila;
            Columna = columna;
            CapacidadCombate = capacidadCombate;
        }
    }
}

