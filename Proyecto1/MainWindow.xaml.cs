using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Proyecto1.Modelos;
using Proyecto1.Servicios;
using Proyecto1.TDA;

namespace Proyecto1
{
    public partial class MainWindow : Window
    {
        private SistemaControl sistema;
        private BuscadorCaminos buscador;
        private GeneradorGraphviz generadorGraphviz;

        private readonly string carpetaSalidaGraphviz =
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GraphvizOutput");

        public MainWindow()
        {
            InitializeComponent();

            sistema = new SistemaControl();
            buscador = new BuscadorCaminos();
            generadorGraphviz = new GeneradorGraphviz();

            DibujarLeyenda();
        }

        private void CargarXML_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialogo = new OpenFileDialog();
            dialogo.Filter = "Archivos XML (*.xml)|*.xml";
            dialogo.Multiselect = true;

            if (dialogo.ShowDialog() == true)
            {
                foreach (string ruta in dialogo.FileNames)
                {
                    sistema.CargarArchivo(ruta);
                }

                txtEstado.Text = "Configuración cargada: " + sistema.Ciudades.Cantidad + " ciudad(es), "
                    + sistema.Robots.Cantidad + " robot(s).";

                PoblarCiudades();
            }
        }

        private void PoblarCiudades()
        {
          
            cmbCiudades.Items.Clear();

            for (int i = 0; i < sistema.Ciudades.Cantidad; i++)
            {
                cmbCiudades.Items.Add(sistema.Ciudades.Obtener(i));
            }

            if (cmbCiudades.Items.Count > 0)
            {
                cmbCiudades.SelectedIndex = 0;
            }
        }

  
        private void cmbCiudades_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Ciudad ciudad = cmbCiudades.SelectedItem as Ciudad;

            cmbTipoMision.Items.Clear();
            cmbRobots.Items.Clear();
            cmbObjetivo.Items.Clear();
            txtResultado.Text = "";
            imgMalla.Source = null;

            if (ciudad == null) return;


            if (ciudad.TieneCiviles())
            {
                cmbTipoMision.Items.Add("Rescate");
            }

            if (ciudad.TieneRecursos())
            {
                cmbTipoMision.Items.Add("Extracción de recursos");
            }

            if (cmbTipoMision.Items.Count > 0)
            {
                cmbTipoMision.SelectedIndex = 0;
            }

            MostrarMision(ciudad, null);
        }

        private void cmbTipoMision_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Ciudad ciudad = cmbCiudades.SelectedItem as Ciudad;
            string tipoMision = cmbTipoMision.SelectedItem as string;

            cmbRobots.Items.Clear();
            cmbObjetivo.Items.Clear();

            if (ciudad == null || tipoMision == null) return;

            if (tipoMision == "Rescate")
            {
                Lista<Robot> robots = sistema.ObtenerRobotsPorTipo("ChapinRescue");
                for (int i = 0; i < robots.Cantidad; i++)
                {
                    cmbRobots.Items.Add(robots.Obtener(i));
                }

                Lista<Celda> civiles = ciudad.ObtenerCiviles();
                for (int i = 0; i < civiles.Cantidad; i++)
                {
                    cmbObjetivo.Items.Add(civiles.Obtener(i));
                }
            }
            else
            {
                Lista<Robot> robots = sistema.ObtenerRobotsPorTipo("ChapinFighter");
                for (int i = 0; i < robots.Cantidad; i++)
                {
                    cmbRobots.Items.Add(robots.Obtener(i));
                }

                Lista<Celda> recursos = ciudad.ObtenerRecursos();
                for (int i = 0; i < recursos.Cantidad; i++)
                {
                    cmbObjetivo.Items.Add(recursos.Obtener(i));
                }
            }

            if (cmbRobots.Items.Count > 0) cmbRobots.SelectedIndex = 0;
            if (cmbObjetivo.Items.Count > 0) cmbObjetivo.SelectedIndex = 0;
        }


        private void Ejecutar_Click(object sender, RoutedEventArgs e)
        {
            Ciudad ciudad = cmbCiudades.SelectedItem as Ciudad;
            string tipoMision = cmbTipoMision.SelectedItem as string;
            Robot robot = cmbRobots.SelectedItem as Robot;
            Celda objetivo = cmbObjetivo.SelectedItem as Celda;

            if (ciudad == null || tipoMision == null)
            {
                MessageBox.Show("Selecciona una ciudad y un tipo de misión.");
                return;
            }

            if (robot == null)
            {
                MessageBox.Show("No hay robots disponibles de ese tipo. La misión no puede realizarse.");
                txtResultado.Text = "Misión Imposible\n(No hay robots del tipo requerido)";
                return;
            }

            if (objetivo == null)
            {
                MessageBox.Show("Selecciona el civil o recurso objetivo.");
                return;
            }

            if (tipoMision == "Rescate")
            {
                EjecutarRescate(ciudad, (RobotRescue)robot, objetivo);
            }
            else
            {
                EjecutarExtraccion(ciudad, (RobotFighter)robot, objetivo);
            }
        }

        private void EjecutarRescate(Ciudad ciudad, RobotRescue robot, Celda civil)
        {
            Lista<Celda> camino = buscador.BuscarCaminoRescate(ciudad, civil.Fila, civil.Columna);

            if (camino == null)
            {
                txtResultado.Text = "Misión Imposible";
                MostrarMision(ciudad, null);
                return;
            }

            txtResultado.Text =
                "Tipo de misión: rescate\n" +
                "Unidad civil rescatada: " + civil.Fila + "," + civil.Columna + "\n" +
                "Robot utilizado: " + robot.Nombre + " (ChapinRescue)";

            MostrarMision(ciudad, camino);
        }

        private void EjecutarExtraccion(Ciudad ciudad, RobotFighter robot, Celda recurso)
        {
            ResultadoExtraccion resultado = buscador.BuscarCaminoExtraccion(
                ciudad, recurso.Fila, recurso.Columna, robot.CapacidadCombate);

            if (resultado == null)
            {
                txtResultado.Text = "Misión Imposible";
                MostrarMision(ciudad, null);
                return;
            }

            txtResultado.Text =
                "Tipo de misión: extracción de recursos\n" +
                "Recurso extraído: " + recurso.Fila + "," + recurso.Columna + "\n" +
                "Robot utilizado: " + robot.Nombre + " (ChapinFighter - Capacidad de combate inicial "
                + robot.CapacidadCombate + ", Capacidad de combate final " + resultado.CapacidadFinal + ")";

            MostrarMision(ciudad, resultado.Camino);
        }

     
        private void MostrarMision(Ciudad ciudad, Lista<Celda> camino)
        {
            try
            {
                string rutaPng = generadorGraphviz.GenerarImagen(ciudad, camino, carpetaSalidaGraphviz);
                imgMalla.Source = CargarImagenSinBloqueo(rutaPng);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo generar el gráfico con Graphviz:\n" + ex.Message,
                    "Error de Graphviz", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private BitmapImage CargarImagenSinBloqueo(string rutaPng)
        {
            byte[] bytes = File.ReadAllBytes(rutaPng);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }

        private void DibujarLeyenda()
        {
            AgregarItemLeyenda(Brushes.Black, "Intransitable");
            AgregarItemLeyenda(Brushes.LightGreen, "Punto de entrada");
            AgregarItemLeyenda(Brushes.White, "Camino");
            AgregarItemLeyenda(Brushes.Red, "Unidad militar");
            AgregarItemLeyenda(Brushes.DodgerBlue, "Unidad civil");
            AgregarItemLeyenda(Brushes.Gray, "Recurso");
            AgregarItemLeyenda(Brushes.Orange, "Ruta");
        }

        private void AgregarItemLeyenda(Brush color, string texto)
        {
            StackPanel fila = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 2) };

            Rectangle muestra = new Rectangle
            {
                Width = 14,
                Height = 14,
                Fill = color,
                Stroke = Brushes.Gray,
                StrokeThickness = 0.5,
                Margin = new Thickness(0, 0, 6, 0)
            };

            TextBlock etiqueta = new TextBlock { Text = texto, VerticalAlignment = VerticalAlignment.Center };

            fila.Children.Add(muestra);
            fila.Children.Add(etiqueta);
            panelLeyenda.Children.Add(fila);
        }
    }
}
