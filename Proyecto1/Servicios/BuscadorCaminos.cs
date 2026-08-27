using Proyecto1.Modelos;
using Proyecto1.TDA;

namespace Proyecto1.Servicios
{
    public class BuscadorCaminos
    {
  
   

        public Lista<Celda> BuscarCaminoRescate(
            Ciudad ciudad,
            int filaCivil, int columnaCivil)
        {
            ciudad.Malla.ReiniciarVisitados();

            Cola<NodoMatriz> cola = new Cola<NodoMatriz>();

            Lista<Celda> entradas = ciudad.ObtenerPuntosEntrada();
            for (int i = 0; i < entradas.Cantidad; i++)
            {
                Celda entrada = entradas.Obtener(i);
                NodoMatriz nodoEntrada = ciudad.Malla.ObtenerNodo(entrada.Fila, entrada.Columna);
                if (nodoEntrada != null && !nodoEntrada.Visitado)
                {
                    nodoEntrada.Visitado = true;
                    cola.Encolar(nodoEntrada);
                }
            }

            NodoMatriz destino = null;

            while (!cola.EstaVacia())
            {
                NodoMatriz actual = cola.Desencolar();

                if (actual.Dato.Fila == filaCivil && actual.Dato.Columna == columnaCivil)
                {
                    destino = actual;
                    break;
                }

                foreach (NodoMatriz vecino in ObtenerVecinos(actual))
                {
                    if (vecino == null || vecino.Visitado) continue;

                    bool esDestino = vecino.Dato.Fila == filaCivil && vecino.Dato.Columna == columnaCivil;

                  
                    if (!esDestino && !EsTransitableParaRescate(vecino.Dato)) continue;
                    if (esDestino && vecino.Dato.Tipo != TipoCelda.Civil) continue;

                    vecino.Visitado = true;
                    vecino.Padre = actual;
                    cola.Encolar(vecino);
                }
            }

            if (destino == null) return null;

            return ReconstruirCaminoSimple(destino);
        }

        private bool EsTransitableParaRescate(Celda celda)
        {
            return celda.Tipo == TipoCelda.Camino
                || celda.Tipo == TipoCelda.Entrada
                || celda.Tipo == TipoCelda.Civil;
        }

 
        public ResultadoExtraccion BuscarCaminoExtraccion(
            Ciudad ciudad,
            int filaRecurso, int columnaRecurso,
            int capacidadInicial)
        {
            int filas = ciudad.Malla.Filas;
            int columnas = ciudad.Malla.Columnas;

            int[,] mejorCapacidad = new int[filas, columnas];
            for (int f = 0; f < filas; f++)
                for (int c = 0; c < columnas; c++)
                    mejorCapacidad[f, c] = -1;

            Cola<NodoBusqueda> cola = new Cola<NodoBusqueda>();

       
            Lista<Celda> entradas = ciudad.ObtenerPuntosEntrada();
            for (int i = 0; i < entradas.Cantidad; i++)
            {
                Celda entrada = entradas.Obtener(i);
                NodoMatriz nodoEntrada = ciudad.Malla.ObtenerNodo(entrada.Fila, entrada.Columna);
                if (nodoEntrada == null) continue;

                int filaIdxEntrada = entrada.Fila - 1;
                int columnaIdxEntrada = entrada.Columna - 1;

                if (capacidadInicial > mejorCapacidad[filaIdxEntrada, columnaIdxEntrada])
                {
                    mejorCapacidad[filaIdxEntrada, columnaIdxEntrada] = capacidadInicial;
                    cola.Encolar(new NodoBusqueda(nodoEntrada, capacidadInicial, null));
                }
            }

            NodoBusqueda encontrado = null;

            while (!cola.EstaVacia())
            {
                NodoBusqueda actual = cola.Desencolar();

                if (actual.Nodo.Dato.Fila == filaRecurso && actual.Nodo.Dato.Columna == columnaRecurso)
                {
                    encontrado = actual;
                    break;
                }

                foreach (NodoMatriz vecino in ObtenerVecinos(actual.Nodo))
                {
                    if (vecino == null) continue;

                    Celda celdaVecino = vecino.Dato;
                    bool esDestino = celdaVecino.Fila == filaRecurso && celdaVecino.Columna == columnaRecurso;

                    int capacidadResultante = actual.CapacidadRestante;

                    if (esDestino)
                    {
                
                        if (celdaVecino.Tipo != TipoCelda.Recurso) continue;
                    }
                    else
                    {
                        if (celdaVecino.Tipo == TipoCelda.Intransitable) continue;
                        if (celdaVecino.Tipo == TipoCelda.Recurso) continue; 
                        if (celdaVecino.Tipo == TipoCelda.Militar)
                        {
                            if (actual.CapacidadRestante <= celdaVecino.CapacidadMilitar)
                            {
                                continue;
                            }

                            capacidadResultante = actual.CapacidadRestante - celdaVecino.CapacidadMilitar;
                        }

                       
                    }

                    int filaIdx = celdaVecino.Fila - 1;
                    int columnaIdx = celdaVecino.Columna - 1;

                    if (capacidadResultante <= mejorCapacidad[filaIdx, columnaIdx])
                    {
                        continue; 
                    }

                    mejorCapacidad[filaIdx, columnaIdx] = capacidadResultante;

                    NodoBusqueda nuevoEstado = new NodoBusqueda(vecino, capacidadResultante, actual);
                    cola.Encolar(nuevoEstado);
                }
            }

            if (encontrado == null) return null;

            Lista<Celda> camino = ReconstruirCaminoBusqueda(encontrado);
            return new ResultadoExtraccion(camino, encontrado.CapacidadRestante);
        }

    
        private NodoMatriz[] ObtenerVecinos(NodoMatriz nodo)
        {
            return new NodoMatriz[] { nodo.Arriba, nodo.Abajo, nodo.Izquierda, nodo.Derecha };
        }

        private Lista<Celda> ReconstruirCaminoSimple(NodoMatriz destino)
        {
            Lista<Celda> invertido = new Lista<Celda>();
            NodoMatriz actual = destino;

            while (actual != null)
            {
                invertido.Agregar(actual.Dato);
                actual = actual.Padre;
            }

            return InvertirLista(invertido);
        }

        private Lista<Celda> ReconstruirCaminoBusqueda(NodoBusqueda destino)
        {
            Lista<Celda> invertido = new Lista<Celda>();
            NodoBusqueda actual = destino;

            while (actual != null)
            {
                invertido.Agregar(actual.Nodo.Dato);
                actual = actual.Padre;
            }

            return InvertirLista(invertido);
        }

        private Lista<Celda> InvertirLista(Lista<Celda> lista)
        {
            Lista<Celda> resultado = new Lista<Celda>();

            for (int i = lista.Cantidad - 1; i >= 0; i--)
            {
                resultado.Agregar(lista.Obtener(i));
            }

            return resultado;
        }
    }
}
