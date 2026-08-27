using Proyecto1.Modelos;
using Proyecto1.TDA;

namespace Proyecto1.Servicios
{
    public class BuscadorCaminos
    {
        // ---------------------------------------------------------------
        // MISIÓN DE RESCATE
        // Recorre la malla con BFS desde el punto de entrada hasta la
        // unidad civil, sin pasar NUNCA por una celda de tipo Militar
        // (ni Recurso ni Intransitable). Si no hay camino, regresa null,
        // lo que el llamador debe interpretar como "Misión Imposible".
        // ---------------------------------------------------------------
        public Lista<Celda> BuscarCaminoRescate(
            Ciudad ciudad,
            int filaCivil, int columnaCivil)
        {
            ciudad.Malla.ReiniciarVisitados();

            Cola<NodoMatriz> cola = new Cola<NodoMatriz>();

            // Multi-origen: encolamos TODOS los puntos de entrada de la ciudad.
            // El BFS encontrará el camino más corto sin importar por cuál se entre.
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

                    // El destino solo es válido si realmente es la celda Civil buscada.
                    // Cualquier otra celda debe ser Camino, Entrada o Civil (nunca Militar,
                    // Recurso ni Intransitable) para que el camino sea "seguro".
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

        // ---------------------------------------------------------------
        // MISIÓN DE EXTRACCIÓN DE RECURSOS
        // BFS "con estado": cada elemento de la cola guarda además cuánta
        // capacidad de combate le queda al robot en ese punto del camino.
        // Puede atravesar una celda Militar solo si su capacidad restante
        // es MAYOR a la capacidad de esa unidad militar, y al hacerlo resta
        // esa capacidad. Se poda la búsqueda descartando revisitar una
        // celda si ya la alcanzamos antes con capacidad igual o mejor
        // (así no explota en tamaño y siempre progresa).
        // ---------------------------------------------------------------
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

            // Multi-origen: igual que en rescate, se prueba desde todos los puntos de entrada.
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
                        // Solo se permite "entrar" a una celda Recurso si es justo el
                        // recurso que se quiere extraer (una celda Recurso normal jamás
                        // es transitable, según el enunciado).
                        if (celdaVecino.Tipo != TipoCelda.Recurso) continue;
                    }
                    else
                    {
                        if (celdaVecino.Tipo == TipoCelda.Intransitable) continue;
                        if (celdaVecino.Tipo == TipoCelda.Recurso) continue; // no es el objetivo: no se puede pisar

                        if (celdaVecino.Tipo == TipoCelda.Militar)
                        {
                            if (actual.CapacidadRestante <= celdaVecino.CapacidadMilitar)
                            {
                                continue; // no la puede vencer, este camino no sirve
                            }

                            capacidadResultante = actual.CapacidadRestante - celdaVecino.CapacidadMilitar;
                        }

                        // Camino, Entrada o Civil: transitable sin costo de capacidad.
                    }

                    int filaIdx = celdaVecino.Fila - 1;
                    int columnaIdx = celdaVecino.Columna - 1;

                    if (capacidadResultante <= mejorCapacidad[filaIdx, columnaIdx])
                    {
                        continue; // ya llegamos aquí antes con capacidad igual o mejor
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

        // ---------------------------------------------------------------
        // Utilidades comunes
        // ---------------------------------------------------------------
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

        // Lista<T> solo tiene Agregar (al final), así que para "invertir" el camino
        // (que se arma de destino -> origen siguiendo Padre) recorremos al revés
        // con índices y lo volvemos a construir en el orden correcto (origen -> destino).
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
