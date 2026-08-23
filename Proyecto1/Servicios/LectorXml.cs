using System;
using System.Collections.Generic;
using System.Text;

using System;
using System.Xml;
using Proyecto1.Modelo;
using Proyecto1.TDA;

namespace Proyecto1.Servicios
{
    public class LectorXML
    {
        public Lista<Ciudad> Ciudades { get; private set; }
        public Lista<Robot> Robots { get; private set; }

        public LectorXML()
        {
            Ciudades = new Lista<Ciudad>();
            Robots = new Lista<Robot>();
        }
        private TipoCelda ConvertirTipoCelda(char simbolo)
        {
            switch (simbolo)
            {
                case '*':
                    return TipoCelda.Camino;

                case 'E':
                    return TipoCelda.Entrada;

                case 'C':
                    return TipoCelda.Civil;

                case 'R':
                    return TipoCelda.Recurso;

                default:
                    return TipoCelda.Intransitable;
            }
        }
        private void LeerMalla(
    XmlNode nodoCiudad,
    Ciudad ciudad)
        {
            XmlNodeList filas =
                nodoCiudad.SelectNodes("fila");

            foreach (XmlNode nodoFila in filas)
            {
                int numeroFila =
                    int.Parse(
                        nodoFila.Attributes["numero"].Value
                    );

                string contenido =
                    nodoFila.InnerText.Trim();

                Lista<Celda> fila =
                    new Lista<Celda>();

                for (int columna = 0;
                     columna < contenido.Length;
                     columna++)
                {
                    char simbolo =
                        contenido[columna];

                    TipoCelda tipo =
                        ConvertirTipoCelda(simbolo);

                    Celda celda =
                        new Celda(
                            numeroFila,
                            columna + 1,
                            tipo
                        );

                    fila.Agregar(celda);
                }

                ciudad.Malla.Agregar(fila);
            }
        }
        private void LeerUnidadesMilitares(
    XmlNode nodoCiudad,
    Ciudad ciudad)
        {
            XmlNodeList militares =
                nodoCiudad.SelectNodes("unidadMilitar");

            foreach (XmlNode nodoMilitar in militares)
            {
                int fila =
                    int.Parse(
                        nodoMilitar.Attributes["fila"].Value
                    );

                int columna =
                    int.Parse(
                        nodoMilitar.Attributes["columna"].Value
                    );

                int capacidad =
                    int.Parse(
                        nodoMilitar.InnerText.Trim()
                    );

                UnidadMilitar militar =
                    new UnidadMilitar(
                        fila,
                        columna,
                        capacidad
                    );

                ciudad.UnidadesMilitares.Agregar(militar);
            }
        }
        private void LeerCiudades(XmlDocument documento)
        {
            XmlNodeList nodosCiudades =
                documento.SelectNodes(
                    "/configuracion/listaCiudades/ciudad"
                );

            foreach (XmlNode nodoCiudad in nodosCiudades)
            {
                XmlNode nodoNombre =
                    nodoCiudad.SelectSingleNode("nombre");

                string nombreCiudad =
                    nodoNombre.InnerText.Trim();

                int filas =
                    int.Parse(nodoNombre.Attributes["filas"].Value);

                int columnas =
                    int.Parse(nodoNombre.Attributes["columnas"].Value);

                Ciudad ciudad =
                    new Ciudad(
                        nombreCiudad,
                        filas,
                        columnas
                    );

                LeerMalla(nodoCiudad, ciudad);

                LeerUnidadesMilitares(nodoCiudad, ciudad);

                Ciudades.Agregar(ciudad);
            }
        }
        private void LeerRobots(XmlDocument documento)
        {
            XmlNodeList nodosRobots =
                documento.SelectNodes(
                    "/configuracion/robots/robot"
                );

            foreach (XmlNode nodoRobot in nodosRobots)
            {
                XmlNode nodoNombre =
                    nodoRobot.SelectSingleNode("nombre");

                string nombre =
                    nodoNombre.InnerText.Trim();

                string tipo =
                    nodoNombre.Attributes["tipo"].Value;

                if (tipo == "ChapinFighter")
                {
                    int capacidad =
                        int.Parse(
                            nodoNombre.Attributes["capacidad"].Value
                        );

                    RobotFire robot =
                        new RobotFire(
                            nombre,
                            capacidad
                        );

                    Robots.Agregar(robot);
                }
                else if (tipo == "ChapinRescue")
                {
                    RobotRescue robot =
                        new RobotRescue(nombre);

                    Robots.Agregar(robot);
                }
            }
        }
        public void CargarArchivo(string ruta)
        {
            XmlDocument documento = new XmlDocument();

            documento.Load(ruta);

            LeerCiudades(documento);
            LeerRobots(documento);
        }
    }
}