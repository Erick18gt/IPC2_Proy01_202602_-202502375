using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        private const int TamanoCelda = 26;

        public MainWindow()
        {
            InitializeComponent();

            sistema = new SistemaControl();
            buscador = new BuscadorCaminos();

            DibujarLeyenda();
        }

        // -----------------------------------------------------------
        // Carga de XML
        // -----------------------------------------------------------
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
            // Recorremos nuestro propio TDA (Lista<Ciudad>) manualmente y agregamos
            // cada elemento uno por uno con Items.Add. NO se usa ItemsSource con
            // ningún arreglo/List/ObservableCollection.
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

        // -----------------------------------------------------------
        // Cascada de selección: Ciudad -> Tipo de misión -> Robot / Objetivo
        // -----------------------------------------------------------
        private void cmbCiudades_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Ciudad ciudad = cmbCiudades.SelectedItem as Ciudad;

            cmbTipoMision.Items.Clear();
            cmbRobots.Items.Clear();
            cmbObjetivo.Items.Clear();
            txtResultado.Text = "";
            canvasMalla.Children.Clear();

            if (ciudad == null) return;

            // Aquí no recorremos un TDA porque solo son dos posibles textos fijos,
            // pero los agregamos directo a Items (sin ItemsSource ni colección intermedia).
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

            DibujarMalla(ciudad, null);
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

        // -----------------------------------------------------------
        // Ejecutar misión
        // -----------------------------------------------------------
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
                DibujarMalla(ciudad, null);
                return;
            }

            txtResultado.Text =
                "Tipo de misión: rescate\n" +
                "Unidad civil rescatada: " + civil.Fila + "," + civil.Columna + "\n" +
                "Robot utilizado: " + robot.Nombre + " (ChapinRescue)";

            DibujarMalla(ciudad, camino);
        }

        private void EjecutarExtraccion(Ciudad ciudad, RobotFighter robot, Celda recurso)
        {
            ResultadoExtraccion resultado = buscador.BuscarCaminoExtraccion(
                ciudad, recurso.Fila, recurso.Columna, robot.CapacidadCombate);

            if (resultado == null)
            {
                txtResultado.Text = "Misión Imposible";
                DibujarMalla(ciudad, null);
                return;
            }

            txtResultado.Text =
                "Tipo de misión: extracción de recursos\n" +
                "Recurso extraído: " + recurso.Fila + "," + recurso.Columna + "\n" +
                "Robot utilizado: " + robot.Nombre + " (ChapinFighter - Capacidad de combate inicial "
                + robot.CapacidadCombate + ", Capacidad de combate final " + resultado.CapacidadFinal + ")";

            DibujarMalla(ciudad, resultado.Camino);
        }

        // -----------------------------------------------------------
        // Dibujo del mapa (versión provisional con WPF; se reemplaza por
        // Graphviz en el siguiente paso sin tocar la lógica de arriba)
        // -----------------------------------------------------------
        private void DibujarMalla(Ciudad ciudad, Lista<Celda> camino)
        {
            canvasMalla.Children.Clear();
            canvasMalla.Width = ciudad.Columnas * TamanoCelda;
            canvasMalla.Height = ciudad.Filas * TamanoCelda;

            bool[,] enCamino = new bool[ciudad.Filas, ciudad.Columnas];
            if (camino != null)
            {
                for (int i = 0; i < camino.Cantidad; i++)
                {
                    Celda c = camino.Obtener(i);
                    enCamino[c.Fila - 1, c.Columna - 1] = true;
                }
            }

            for (int fila = 1; fila <= ciudad.Filas; fila++)
            {
                for (int columna = 1; columna <= ciudad.Columnas; columna++)
                {
                    var nodo = ciudad.Malla.ObtenerNodo(fila, columna);
                    if (nodo == null) continue;

                    Celda celda = nodo.Dato;
                    Brush color = ColorDeCelda(celda.Tipo);

                    // Si la celda es parte del camino Y es "de paso" (Camino/Entrada),
                    // la resaltamos en naranja como en el ejemplo del enunciado.
                    bool esDePaso = celda.Tipo == TipoCelda.Camino || celda.Tipo == TipoCelda.Entrada;
                    if (enCamino[fila - 1, columna - 1] && esDePaso)
                    {
                        color = Brushes.Orange;
                    }

                    Rectangle rect = new Rectangle
                    {
                        Width = TamanoCelda - 1,
                        Height = TamanoCelda - 1,
                        Fill = color,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 0.5
                    };

                    Canvas.SetLeft(rect, (columna - 1) * TamanoCelda);
                    Canvas.SetTop(rect, (fila - 1) * TamanoCelda);
                    canvasMalla.Children.Add(rect);
                }
            }
        }

        private Brush ColorDeCelda(TipoCelda tipo)
        {
            switch (tipo)
            {
                case TipoCelda.Intransitable: return Brushes.Black;
                case TipoCelda.Entrada: return Brushes.LightGreen;
                case TipoCelda.Camino: return Brushes.White;
                case TipoCelda.Militar: return Brushes.Red;
                case TipoCelda.Civil: return Brushes.DodgerBlue;
                case TipoCelda.Recurso: return Brushes.Gray;
                default: return Brushes.White;
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
