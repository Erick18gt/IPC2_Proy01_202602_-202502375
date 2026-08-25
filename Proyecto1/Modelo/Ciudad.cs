using Proyecto1.TDA;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto1.Modelo
{
    public class Ciudad
    {
        public string Nombre { get; set; }
        public int Filas { get; set; }
        public int Columnas { get; set; }

        public Lista<Lista<Celda>> Malla { get; set; }
        public Lista<UnidadMilitar> UnidadesMilitares { get; set; }

        public Ciudad(string nombre, int filas, int columnas)
        {
            Nombre = nombre;
            Filas = filas;
            Columnas = columnas;

            Malla = new Lista<Lista<Celda>>();
            UnidadesMilitares = new Lista<UnidadMilitar>();
        }
    }
}

