using System;
using System.Windows;
using System.Windows.Media;
using WpfAppBar;
using CommandLine;
using Banner;
using System.Net.Sockets;
using System.Net;

namespace ClassificationBanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Options options = Parser.Default.ParseArguments<Options>(Environment.GetCommandLineArgs()).Value;
        public MainWindow()
        {
            InitializeComponent();
        }

        public class Options
        {
            [Option('b', "bgcolor", Required = false, HelpText = "Set background color of banner.")]
            public string? BgColor { get; set; }

            [Option('f', "fgcolor", Required = false, HelpText = "Set text color of banner.")]
            public string? FgColor { get; set; }

            [Option('m', "message", Required = false, HelpText = "Set center text of banner.")]
            public string? CValue { get; set; }

            [Option('l', "left", Required = false, HelpText = "Set left text of banner.")]
            public string? LValue { get; set; }

            [Option('r', "right", Required = false, HelpText = "Set right text of banner.")]
            public string? RValue { get; set; }

            [Option('x', "no-sysinfo", Required = false, HelpText = "Disables system info on the left side.")]
            public bool NoSysinfo { get; set; }

            [Option('t', "top-bar-only", Required = false, HelpText = "Disables border on all four sides.")]
            public bool TopbarOnly { get; set; }

            [Option('p', "preset", Required = false, HelpText = "Choose a preset to load.")]
            public string? Preset { get; set; }
        }

        private void Main()
        {
            string? Preset = options.Preset;

            SolidColorBrush? BgColor = new BrushConverter().ConvertFrom("#007A33") as SolidColorBrush;
            SolidColorBrush? FgColor = new BrushConverter().ConvertFrom("#000000") as SolidColorBrush;
            string CValue = "UNCLASSIFIED";
            string LValue = "";
            string RValue = "";

            switch (Preset)
            {
                case "1":
                case "u":
                case "unclassified":
                    break;
                case "2":
                case "c":
                case "confidential":
                    BgColor = new BrushConverter().ConvertFrom("#0033A0") as SolidColorBrush;
                    FgColor = new BrushConverter().ConvertFrom("#FFFFFF") as SolidColorBrush;
                    CValue = "CONFIDENTIAL";
                    break;
                case "3":
                case "s":
                case "secret":
                    BgColor = new BrushConverter().ConvertFrom("#C8102E") as SolidColorBrush;
                    FgColor = new BrushConverter().ConvertFrom("#FFFFFF") as SolidColorBrush;
                    CValue = "SECRET";
                    break;
                case "4":
                case "ts":
                case "topsecret":
                case "top-secret":
                    BgColor = new BrushConverter().ConvertFrom("#FF671F") as SolidColorBrush;
                    FgColor = new BrushConverter().ConvertFrom("#FFFFFF") as SolidColorBrush;
                    CValue = "TOP SECRET";
                    break;
                case "5":
                case "tssci":
                case "ts-sci":
                case "topsecret-sci":
                case "top-secret-sci":
                    BgColor = new BrushConverter().ConvertFrom("#F7EA48") as SolidColorBrush;
                    FgColor = new BrushConverter().ConvertFrom("#000000") as SolidColorBrush;
                    CValue = "TOP SECRET // SCI";
                    break;
                default:
                    break;
            }

            if (options.BgColor != null) { BgColor = new BrushConverter().ConvertFrom(options.BgColor) as SolidColorBrush; }
            if (options.FgColor != null) { FgColor = new BrushConverter().ConvertFrom(options.FgColor) as SolidColorBrush; }
            if (options.CValue != null) { CValue = options.CValue; }
            if (options.LValue != null) { LValue = options.LValue; }
            if (options.RValue != null) { RValue = options.RValue; }

            bool Sysinfo = !options.NoSysinfo;
            bool TopbarOnly = options.TopbarOnly;

            LeftWindow LWindow = new();
            RightWindow RWindow = new();
            BottomWindow BWindow = new();

            if (!TopbarOnly)
            {
                LWindow.Owner = this; LWindow.Show();
                RWindow.Owner = this; RWindow.Show();
                BWindow.Owner = this; BWindow.Show();

                LWindow.Background = BgColor;
                RWindow.Background = BgColor;
                BWindow.Background = BgColor;
            }

            MWindow.Background = BgColor;
            CText.Foreground = FgColor;
            LText.Foreground = FgColor;
            RText.Foreground = FgColor;
            CText.Text = CValue;
            LText.Text = LValue;
            RText.Text = RValue;

            if (Sysinfo)
            {
                if (LValue == "") { LValue = "FNIC"; }
                LValue += " | ";
                LValue += Environment.UserName;
                LValue += " | ";
                LValue += Environment.OSVersion.ToString().Replace("Microsoft Windows ", "");
                LValue += " | ";
                LValue += GetLocalIPAddress();

                LText.Text = LValue;
            }

            AppBarFunctions.SetAppBar(this, ABEdge.Top);
            if (!TopbarOnly)
            {
                AppBarFunctions.SetAppBar(LWindow, ABEdge.Left);
                AppBarFunctions.SetAppBar(RWindow, ABEdge.Right);
                AppBarFunctions.SetAppBar(BWindow, ABEdge.Bottom);
                LWindow.WindowState = WindowState.Normal;
                RWindow.WindowState = WindowState.Normal;
                BWindow.WindowState = WindowState.Normal;
            }
            MWindow.WindowState = WindowState.Normal;
        }
        private static string GetLocalIPAddress()
        {
            using Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;
            return endPoint?.Address.ToString() ?? "127.0.0.1";
        }
        private void MWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Main();
        }
    }
}
