namespace Proyecto1.Modelos
{
    public class Celda
    {
        public int Fila { get; set; }
        public int Columna { get; set; }
        public TipoCelda Tipo { get; set; }

        public int CapacidadMilitar { get; set; }

        public Celda(int fila, int columna, TipoCelda tipo)
        {
            Fila = fila;
            Columna = columna;
            Tipo = tipo;
            CapacidadMilitar = 0;
        }

        public override string ToString()
        {
            return "Fila " + Fila + ", Columna " + Columna;
        }
    }
}
