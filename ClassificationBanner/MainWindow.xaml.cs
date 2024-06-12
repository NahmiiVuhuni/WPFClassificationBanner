using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAppBar;

namespace ClassificationBanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, string> runtimeArgs = new Dictionary<string, string>();
        public MainWindow()
        {
            string[] args = Environment.GetCommandLineArgs();

            InitializeComponent();

            for (int index = 1; index < args.Length; index += 2)
            {
                runtimeArgs.Add(args[index], args[index + 1]);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AppBarFunctions.SetAppBar(this, ABEdge.Top);
            string inBgcolor;
            if (runtimeArgs.TryGetValue("-b", out inBgcolor) ||
                runtimeArgs.TryGetValue("--bgcolor", out inBgcolor))
            {
                ContainerWindow.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(inBgcolor);
            }
        }

        private void ClassificationText_Loaded(object sender, RoutedEventArgs e)
        {
            string inMessage;
            if (runtimeArgs.TryGetValue("-m", out inMessage) ||
                runtimeArgs.TryGetValue("--message", out inMessage))
            {
                ClassificationText.Text = inMessage;
            }

            string inFgcolor;
            if (runtimeArgs.TryGetValue("-f", out inFgcolor) ||
                runtimeArgs.TryGetValue("--fgcolor", out inFgcolor))
            {
                ClassificationText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(inFgcolor);
            }
        }
    }
}
