namespace Proyecto1.Modelos
{
    public enum TipoMision
    {
        Rescate,
        Extraccion
    }

    public class Mision
    {
        public TipoMision Tipo { get; set; }
        public Ciudad Ciudad { get; set; }
        public Robot Robot { get; set; }

        public int FilaObjetivo { get; set; }
        public int ColumnaObjetivo { get; set; }

        public Mision(TipoMision tipo, Ciudad ciudad, Robot robot, int filaObjetivo, int columnaObjetivo)
        {
            Tipo = tipo;
            Ciudad = ciudad;
            Robot = robot;
            FilaObjetivo = filaObjetivo;
            ColumnaObjetivo = columnaObjetivo;
        }
    }
}
