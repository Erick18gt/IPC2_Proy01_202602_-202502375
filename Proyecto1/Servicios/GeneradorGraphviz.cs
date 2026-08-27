using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Proyecto1.Modelos;
using Proyecto1.TDA;

namespace Proyecto1.Servicios
{

    public class GeneradorGraphviz
    {
        private const double Escala = 0.5;      
        private const double TamanoCelda = 0.45; 
        private const double PuntosPorPulgada = 72.0;

        public string GenerarDot(Ciudad ciudad, Lista<Celda> camino)
        {
            bool[,] enCamino = MarcarCamino(ciudad, camino);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("graph Mision {");
            sb.AppendLine("    node [shape=square, style=filled, fixedsize=true, width="
                + Formatear(TamanoCelda) + ", height=" + Formatear(TamanoCelda) + ", label=\"\"];");

            for (int fila = 1; fila <= ciudad.Filas; fila++)
            {
                for (int columna = 1; columna <= ciudad.Columnas; columna++)
                {
                    var nodo = ciudad.Malla.ObtenerNodo(fila, columna);
                    if (nodo == null) continue;

                    Celda celda = nodo.Dato;
                    string color = ColorDot(celda.Tipo);

                    bool esDePaso = celda.Tipo == TipoCelda.Camino || celda.Tipo == TipoCelda.Entrada;
                    if (enCamino[fila - 1, columna - 1] && esDePaso)
                    {
                        color = "orange";
                    }

                    
                    double x = (columna - 1) * Escala * PuntosPorPulgada;
                    double y = -(fila - 1) * Escala * PuntosPorPulgada;

                    string id = "n_" + fila + "_" + columna;

                    sb.AppendLine("    " + id + " [pos=\"" + Formatear(x) + "," + Formatear(y)
                        + "!\", fillcolor=\"" + color + "\"];");
                }
            }

            sb.AppendLine("}");
            return sb.ToString();
        }


        public string GenerarImagen(Ciudad ciudad, Lista<Celda> camino, string carpetaSalida)
        {
            if (!Directory.Exists(carpetaSalida))
            {
                Directory.CreateDirectory(carpetaSalida);
            }

            string rutaDot = Path.Combine(carpetaSalida, "mision.dot");
            string rutaPng = Path.Combine(carpetaSalida, "mision.png");

            File.WriteAllText(rutaDot, GenerarDot(ciudad, camino));

            string ejecutable = ObtenerRutaEjecutableDot();

            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = ejecutable,
                Arguments = "-Kneato -n2 -Tpng \"" + rutaDot + "\" -o \"" + rutaPng + "\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            };

            using (Process proceso = Process.Start(info))
            {
                string error = proceso.StandardError.ReadToEnd();
                proceso.WaitForExit();

                if (proceso.ExitCode != 0)
                {
                    throw new InvalidOperationException("Graphviz devolvió un error:\n" + error);
                }
            }

            return rutaPng;
        }

        
        private string ObtenerRutaEjecutableDot()
        {
            string[] candidatos =
            {
                "dot",
                @"C:\Program Files\Graphviz\bin\dot.exe",
                @"C:\Program Files (x86)\Graphviz\bin\dot.exe"
            };

            foreach (string candidato in candidatos)
            {
                if (FuncionaComoEjecutable(candidato))
                {
                    return candidato;
                }
            }

            throw new InvalidOperationException(
                "No se encontró Graphviz instalado. Descárgalo de https://graphviz.org/download/ " +
                "e instálalo marcando la opción 'Add Graphviz to the system PATH for all users'.");
        }

        private bool FuncionaComoEjecutable(string ruta)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = ruta,
                    Arguments = "-V",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                using (Process proceso = Process.Start(info))
                {
                    proceso.WaitForExit(2000);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool[,] MarcarCamino(Ciudad ciudad, Lista<Celda> camino)
        {
            bool[,] resultado = new bool[ciudad.Filas, ciudad.Columnas];

            if (camino != null)
            {
                for (int i = 0; i < camino.Cantidad; i++)
                {
                    Celda c = camino.Obtener(i);
                    resultado[c.Fila - 1, c.Columna - 1] = true;
                }
            }

            return resultado;
        }

        private string ColorDot(TipoCelda tipo)
        {
            switch (tipo)
            {
                case TipoCelda.Intransitable: return "black";
                case TipoCelda.Entrada: return "palegreen";
                case TipoCelda.Camino: return "white";
                case TipoCelda.Militar: return "red";
                case TipoCelda.Civil: return "dodgerblue";
                case TipoCelda.Recurso: return "gray";
                default: return "white";
            }
        }

        private string Formatear(double valor)
        {
            return valor.ToString(CultureInfo.InvariantCulture);
        }
    }
}
