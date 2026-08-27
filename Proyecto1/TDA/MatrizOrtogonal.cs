using Proyecto1.Modelos;

namespace Proyecto1.TDA
{
    public class MatrizOrtogonal
    {
        private NodoMatriz[,] nodos;

        public int Filas { get; private set; }
        public int Columnas { get; private set; }

        public MatrizOrtogonal(int filas, int columnas)
        {
            Filas = filas;
            Columnas = columnas;

            nodos = new NodoMatriz[filas, columnas];
        }

        public void Insertar(Celda celda)
        {
            int fila = celda.Fila - 1;
            int columna = celda.Columna - 1;

            if (fila < 0 || fila >= Filas || columna < 0 || columna >= Columnas)
            {
                return;
            }

            NodoMatriz nuevo = new NodoMatriz(celda);
            nodos[fila, columna] = nuevo;

            if (fila > 0)
            {
                nuevo.Arriba = nodos[fila - 1, columna];
                if (nuevo.Arriba != null) nuevo.Arriba.Abajo = nuevo;
            }

            if (fila < Filas - 1)
            {
                nuevo.Abajo = nodos[fila + 1, columna];
                if (nuevo.Abajo != null) nuevo.Abajo.Arriba = nuevo;
            }

            if (columna > 0)
            {
                nuevo.Izquierda = nodos[fila, columna - 1];
                if (nuevo.Izquierda != null) nuevo.Izquierda.Derecha = nuevo;
            }

            if (columna < Columnas - 1)
            {
                nuevo.Derecha = nodos[fila, columna + 1];
                if (nuevo.Derecha != null) nuevo.Derecha.Izquierda = nuevo;
            }
        }

        public NodoMatriz ObtenerNodo(int fila, int columna)
        {
            if (fila < 1 || fila > Filas) return null;
            if (columna < 1 || columna > Columnas) return null;

            return nodos[fila - 1, columna - 1];
        }

        // NUEVO: permite actualizar una celda ya insertada (lo usa el LectorXML al
        // procesar <unidadMilitar>, ya que esa celda ya existe como Camino y hay que
        // convertirla en Militar sin romper los enlaces Arriba/Abajo/Izquierda/Derecha).
        public void ActualizarCelda(Celda celdaActualizada)
        {
            NodoMatriz nodo = ObtenerNodo(celdaActualizada.Fila, celdaActualizada.Columna);
            if (nodo != null)
            {
                nodo.Dato = celdaActualizada;
            }
        }

        public void ReiniciarVisitados()
        {
            for (int fila = 0; fila < Filas; fila++)
            {
                for (int columna = 0; columna < Columnas; columna++)
                {
                    if (nodos[fila, columna] != null)
                    {
                        nodos[fila, columna].Visitado = false;
                        nodos[fila, columna].Padre = null;
                    }
                }
            }
        }

        // NUEVO: recorre toda la malla y devuelve las celdas de un tipo específico.
        // Se usa para encontrar puntos de entrada, civiles y recursos.
        public Lista<Celda> ObtenerCeldasPorTipo(TipoCelda tipo)
        {
            Lista<Celda> resultado = new Lista<Celda>();

            for (int fila = 0; fila < Filas; fila++)
            {
                for (int columna = 0; columna < Columnas; columna++)
                {
                    if (nodos[fila, columna] != null && nodos[fila, columna].Dato.Tipo == tipo)
                    {
                        resultado.Agregar(nodos[fila, columna].Dato);
                    }
                }
            }

            return resultado;
        }
    }
}
