using Proyecto1.TDA;

namespace Proyecto1.Modelos
{
    public class Ciudad
    {
        public string Nombre { get; set; }
        public int Filas { get; set; }
        public int Columnas { get; set; }

        public MatrizOrtogonal Malla { get; set; }

        public Lista<UnidadMilitar> UnidadesMilitares { get; set; }

        public Ciudad(string nombre, int filas, int columnas)
        {
            Nombre = nombre;
            Filas = filas;
            Columnas = columnas;

            Malla = new MatrizOrtogonal(filas, columnas);
            UnidadesMilitares = new Lista<UnidadMilitar>();
        }

        public bool TieneCiviles()
        {
            return Malla.ObtenerCeldasPorTipo(TipoCelda.Civil).Cantidad > 0;
        }

        public bool TieneRecursos()
        {
            return Malla.ObtenerCeldasPorTipo(TipoCelda.Recurso).Cantidad > 0;
        }

        public Lista<Celda> ObtenerPuntosEntrada()
        {
            return Malla.ObtenerCeldasPorTipo(TipoCelda.Entrada);
        }

        public Lista<Celda> ObtenerCiviles()
        {
            return Malla.ObtenerCeldasPorTipo(TipoCelda.Civil);
        }

        public Lista<Celda> ObtenerRecursos()
        {
            return Malla.ObtenerCeldasPorTipo(TipoCelda.Recurso);
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
