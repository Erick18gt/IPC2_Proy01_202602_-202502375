using Proyecto1.TDA;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Proyecto1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Lista<int> numeros = new Lista<int>();

            numeros.Agregar(10);
            numeros.Agregar(20);
            numeros.Agregar(30);

            Console.WriteLine(numeros.Cantidad);
            Console.WriteLine(numeros.Obtener(0));
            Console.WriteLine(numeros.Obtener(1));
            Console.WriteLine(numeros.Obtener(2));
        }

    }
}