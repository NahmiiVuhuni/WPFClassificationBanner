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
    public partial class MainWindow : Window
    {
        private Options options = Parser.Default.ParseArguments<Options>(Environment.GetCommandLineArgs()).Value;
        private LeftWindow? LWindow;
        private RightWindow? RWindow;
        private BottomWindow? BWindow;

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

            [Option('c', "cyclable", Required = false, HelpText = "Allows clicking on the banner to cycle presets.")]
            public bool Cyclable { get; set; }

            [Option('p', "preset", Required = false, HelpText = "Choose a preset to load.")]
            public string? Preset { get; set; }
        }

        private void Main()
        {
            if (options.Preset != null) { LoadPreset(); }

            if (!options.TopbarOnly) { InitBorder(); }

            if (!options.NoSysinfo) { GetSysinfo(); }

            SetStyle();

            // Dock and un-minimize windows
            AppBarFunctions.SetAppBar(this, ABEdge.Top);
            if (!options.TopbarOnly)
            {
                AppBarFunctions.SetAppBar(LWindow, ABEdge.Left);
                AppBarFunctions.SetAppBar(RWindow, ABEdge.Right);
                AppBarFunctions.SetAppBar(BWindow, ABEdge.Bottom);
                if (LWindow != null) { LWindow.WindowState = WindowState.Normal; }
                if (RWindow != null) { RWindow.WindowState = WindowState.Normal; }
                if (BWindow != null) { BWindow.WindowState = WindowState.Normal; }
            }
            MWindow.WindowState = WindowState.Normal;
        }

        private void InitBorder()
        {
            // Initialize borders and prepare to be shown.
            // At this stage, borders are still minimized and not docked.

            LWindow = new();
            RWindow = new();
            BWindow = new();

            LWindow.Owner = this;
            RWindow.Owner = this;
            BWindow.Owner = this;

            LWindow.Show();
            RWindow.Show();
            BWindow.Show();

            LWindow.Background = MWindow.Background;
            RWindow.Background = MWindow.Background;
            BWindow.Background = MWindow.Background;
        }

        private void SetStyle()
        {
            // Assign values from options object, or set default value if neccessary.
            // Default is the DoD UNCLASSIFIED banner.
            MWindow.Background = new BrushConverter().ConvertFrom(options.BgColor ?? "#007A33") as SolidColorBrush;
            CText.Foreground = new BrushConverter().ConvertFrom(options.FgColor ?? "#000000") as SolidColorBrush;
            LText.Foreground = CText.Foreground;
            RText.Foreground = CText.Foreground;
            CText.Text = options.CValue ?? "UNCLASSIFIED";
            LText.Text = options.LValue ?? "";
            RText.Text = options.RValue ?? "";

            //If the full border is to be drawn, get border color from main window's background.
            if (!options.TopbarOnly)
            {
                if (LWindow != null) { LWindow.Background = MWindow.Background; }
                if (RWindow != null) { RWindow.Background = MWindow.Background; }
                if (BWindow != null) { BWindow.Background = MWindow.Background; }
            }
        }

        private void LoadPreset()
        {
            switch (options.Preset)
            {
                case "1":
                case "u":
                case "unclassified":
                    options.BgColor = "#007A33";
                    options.FgColor = "#000000";
                    options.CValue = "UNCLASSIFIED";
                    break;
                case "2":
                case "c":
                case "confidential":
                    options.BgColor = "#0033A0";
                    options.FgColor = "#FFFFFF";
                    options.CValue = "CONFIDENTIAL";
                    break;
                case "3":
                case "s":
                case "secret":
                    options.BgColor = "#C8102E";
                    options.FgColor = "#FFFFFF";
                    options.CValue = "SECRET";
                    break;
                case "4":
                case "ts":
                case "topsecret":
                case "top-secret":
                    options.BgColor = "#FF671F";
                    options.FgColor = "#FFFFFF";
                    options.CValue = "TOP SECRET";
                    break;
                case "5":
                case "tssci":
                case "ts-sci":
                case "topsecret-sci":
                case "top-secret-sci":
                    options.BgColor = "#F7EA48";
                    options.FgColor = "#000000";
                    options.CValue = "TOP SECRET // SCI";
                    break;
                default:
                    break;
            }
        }
        private void GetSysinfo()
        {
            // Example output: DISA | Nahmii | NT 10.0.22631.0 | 192.168.1.101
            // This is terrible, a string builder would be way better.
            // TODO: Handle custom names properly.
            options.LValue ??= "DISA";
            options.LValue += " | ";
            options.LValue += Environment.UserName;
            options.LValue += " | ";
            options.LValue += Environment.OSVersion.ToString().Replace("Microsoft Windows ", "");
            options.LValue += " | ";
            options.LValue += GetLocalIPAddress();

            LText.Text = options.LValue;
        }
        private static string GetLocalIPAddress()
        {
            // This is a hack, however it's the only reliable way to show the correct local IP when using virt/container networks or VPNs.
            using Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;
            return endPoint?.Address.ToString() ?? "127.0.0.1"; // Default to showing localhost if nothing found. Likely will appear when link down.
        }
        private void MWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // This handler is always active, but does nothing unless cyclable is set.
            // Assumption is that the user has not selected a preset, so first call will switch to switch 2 (as switch 1 is default).
            // If a preset was chosen and it's not a numerical preset, this'll jump out of order on the first click.
            // Order: 2-5, then 1-5 repeating
            if (options.Cyclable)
            {
                _ = Int32.TryParse(options.Preset, out int i);
                if (i == 0) { i = 1; }
                else if (i > 4) { i = 0; }
                i++;
                options.Preset = i.ToString();
                LoadPreset();
                SetStyle();
            }
        }
        private void MWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Main();
        }
    }
}
