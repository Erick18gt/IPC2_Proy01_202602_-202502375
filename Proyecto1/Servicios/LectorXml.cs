using System;
using System.Xml;
using Proyecto1.Modelos;
using Proyecto1.TDA;

namespace Proyecto1.Servicios
{
    public class LectorXML
    {
        public ResultadoCarga Cargar(string ruta)
        {
            XmlDocument documento = new XmlDocument();
            documento.Load(ruta);

            ResultadoCarga resultado = new ResultadoCarga();

            CargarCiudades(documento, resultado);
            CargarRobots(documento, resultado);

            return resultado;
        }

        private void CargarCiudades(XmlDocument documento, ResultadoCarga resultado)
        {
            XmlNodeList ciudades = documento.SelectNodes("/configuracion/listaCiudades/ciudad");
            if (ciudades == null) return;

            foreach (XmlNode nodoCiudad in ciudades)
            {
                XmlNode nodoNombre = nodoCiudad.SelectSingleNode("nombre");
                if (nodoNombre == null) continue;

                string nombre = nodoNombre.InnerText.Trim();
                int filas = int.Parse(nodoNombre.Attributes["filas"].Value);
                int columnas = int.Parse(nodoNombre.Attributes["columnas"].Value);

                Ciudad ciudad = new Ciudad(nombre, filas, columnas);

                XmlNodeList nodosFila = nodoCiudad.SelectNodes("fila");
                if (nodosFila != null)
                {
                    foreach (XmlNode nodoFila in nodosFila)
                    {
                        int numeroFila = int.Parse(nodoFila.Attributes["numero"].Value);
                        string contenido = nodoFila.InnerText.Trim('\r', '\n');

                        

                        for (int columna = 0; columna < contenido.Length; columna++)
                        {
                            char simbolo = contenido[columna];
                            TipoCelda tipo = ConvertirTipo(simbolo);

                            Celda celda = new Celda(numeroFila, columna + 1, tipo);
                            ciudad.Malla.Insertar(celda);
                        }
                    }
                }

 
                XmlNodeList nodosMilitar = nodoCiudad.SelectNodes("unidadMilitar");
                if (nodosMilitar != null)
                {
                    foreach (XmlNode nodoMilitar in nodosMilitar)
                    {
                        int filaMilitar = int.Parse(nodoMilitar.Attributes["fila"].Value);
                        int columnaMilitar = int.Parse(nodoMilitar.Attributes["columna"].Value);
                        int capacidad = int.Parse(nodoMilitar.InnerText.Trim());

                        Celda celdaMilitar = new Celda(filaMilitar, columnaMilitar, TipoCelda.Militar);
                        celdaMilitar.CapacidadMilitar = capacidad;

                        
                        ciudad.Malla.ActualizarCelda(celdaMilitar);

                        ciudad.UnidadesMilitares.Agregar(
                            new UnidadMilitar(filaMilitar, columnaMilitar, capacidad)
                        );
                    }
                }

                resultado.Ciudades.Agregar(ciudad);
            }
        }

        private void CargarRobots(XmlDocument documento, ResultadoCarga resultado)
        {
            XmlNodeList robots = documento.SelectNodes("/configuracion/robots/robot");
            if (robots == null) return;

            foreach (XmlNode nodoRobot in robots)
            {
                XmlNode nodoNombre = nodoRobot.SelectSingleNode("nombre");
                if (nodoNombre == null) continue;

                string nombreRobot = nodoNombre.InnerText.Trim();
                string tipoRobot = nodoNombre.Attributes["tipo"].Value;

                Robot robot;

                if (tipoRobot == "ChapinFighter")
                {
                    int capacidad = int.Parse(nodoNombre.Attributes["capacidad"].Value);
                    robot = new RobotFighter(nombreRobot, capacidad);
                }
                else if (tipoRobot == "ChapinRescue")
                {
                    robot = new RobotRescue(nombreRobot);
                }
                else
                {
                   
                    continue;
                }

                resultado.Robots.Agregar(robot);
            }
        }

        private TipoCelda ConvertirTipo(char simbolo)
        {
            switch (simbolo)
            {
                case 'E': return TipoCelda.Entrada;
                case 'C': return TipoCelda.Civil;
                case 'R': return TipoCelda.Recurso;
                case '*': return TipoCelda.Intransitable;
                case ' ': return TipoCelda.Camino;
                default: return TipoCelda.Intransitable;
            }
        }
    }
}
