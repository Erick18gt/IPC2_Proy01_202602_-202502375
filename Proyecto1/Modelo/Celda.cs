using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto1.Modelo
{
    public class Celda
    {
        public int Fila { get; set; }
        public int Columna { get; set; }
        public TipoCelda Tipo { get; set; }
        public bool Visitada { get; set; }
        public Celda(int fila, int columna, TipoCelda tipo) {
           Fila = fila;
           Columna = columna;
           Tipo = tipo;
            Visitada = false;

        }

    }
}
