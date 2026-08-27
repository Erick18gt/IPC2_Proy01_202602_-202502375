using Proyecto1.Modelos;
using Proyecto1.TDA;

namespace Proyecto1.Servicios
{
    public class SistemaControl
    {
        public Lista<Ciudad> Ciudades { get; set; }
        public Lista<Robot> Robots { get; set; }

        public SistemaControl()
        {
            Ciudades = new Lista<Ciudad>();
            Robots = new Lista<Robot>();
        }

        
        public void CargarArchivo(string ruta)
        {
            LectorXML lector = new LectorXML();
            ResultadoCarga resultado = lector.Cargar(ruta);

            for (int i = 0; i < resultado.Ciudades.Cantidad; i++)
            {
                Ciudad nueva = resultado.Ciudades.Obtener(i);
                int indice = Ciudades.BuscarIndice(c => c.Nombre == nueva.Nombre);

                if (indice >= 0)
                {
                    Ciudades.Reemplazar(indice, nueva);
                }
                else
                {
                    Ciudades.Agregar(nueva);
                }
            }

            for (int i = 0; i < resultado.Robots.Cantidad; i++)
            {
                Robot nuevo = resultado.Robots.Obtener(i);
                int indice = Robots.BuscarIndice(r => r.Nombre == nuevo.Nombre);

                if (indice >= 0)
                {
                    Robots.Reemplazar(indice, nuevo);
                }
                else
                {
                    Robots.Agregar(nuevo);
                }
            }
        }

        public Ciudad BuscarCiudad(string nombre)
        {
            int indice = Ciudades.BuscarIndice(c => c.Nombre == nombre);
            return indice >= 0 ? Ciudades.Obtener(indice) : null;
        }

        public Robot BuscarRobot(string nombre)
        {
            int indice = Robots.BuscarIndice(r => r.Nombre == nombre);
            return indice >= 0 ? Robots.Obtener(indice) : null;
        }

        public Lista<Robot> ObtenerRobotsPorTipo(string tipo)
        {
            Lista<Robot> resultado = new Lista<Robot>();

            for (int i = 0; i < Robots.Cantidad; i++)
            {
                Robot r = Robots.Obtener(i);
                if (r.Tipo == tipo)
                {
                    resultado.Agregar(r);
                }
            }

            return resultado;
        }
    }
}
