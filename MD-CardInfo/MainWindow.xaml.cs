using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Xml.Linq;

namespace MD_CardInfo
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
         static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
         static extern int SetWindowLong(IntPtr hMenu, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
         static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
         static int cardid = 0;
         static Timer timer = new (150);
        static SQLite.SQLiteConnection conn = new("cards.db");
        static MainViewModel vm = new MainViewModel();
        public static readonly string ConfigXMLPath = Environment.CurrentDirectory + "\\config.xml";
        public static XDocument ConfigXML { get; set; } = new();
        public MainWindow()
        {
            InitializeComponent();

            Func.GetMDProcess();
            if (Func.gProcess is null)
            {
                MessageBox.Show("先开游戏吧，大臭猪。", "我游戏呢？", MessageBoxButton.OK, MessageBoxImage.Stop);
                Close();
            }
            Func.GetGameAssembly();
            Func.OpenMDProcess();

            if (!File.Exists("cards.db"))
            {
                MessageBox.Show("数据库不见了。", "我数据库呢？", MessageBoxButton.OK, MessageBoxImage.Stop);
                Close();
            }
            conn.CreateTable<DBTables.Cards>();

            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            LoadConfig();



            this.DataContext = vm;
        }
        private void LoadConfig()
        {
            try
            {
                if (!File.Exists(ConfigXMLPath))
                {
                    XElement root = new("Config");
                    ConfigXML.Add(root);
                    XElement child = new("width");
                    root.Add(child);
                    child = new("height");
                    root.Add(child);
                    child = new("fontSize");
                    root.Add(child);
                    child = new("topMost");
                    root.Add(child);
                    ConfigXML.Save(ConfigXMLPath);
                }
                else
                {
                    ConfigXML = XDocument.Load(ConfigXMLPath);
                    if (ConfigXML.Root == null) return;
                    var e = ConfigXML.Root.Element("width");
                    if (e != null && !string.IsNullOrEmpty(e.Value))
                        Width = double.Parse(e.Value);
                    e = ConfigXML.Root.Element("height");
                    if (e != null && !string.IsNullOrEmpty(e.Value))
                        Height = double.Parse(e.Value);
                    e = ConfigXML.Root.Element("fontSize");
                    if (e != null && !string.IsNullOrEmpty(e.Value))
                        vm.FontSize = double.Parse(e.Value);
                    e = ConfigXML.Root.Element("topMost");
                    if (e != null && !string.IsNullOrEmpty(e.Value))
                        vm.topMost = bool.Parse(e.Value);
                }
            }
            catch
            {
               
            }

        }
        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            var id = Func.GetCardID();
            if (id is null)
            {
                timer.Stop();
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            MessageBox.Show("检测到游戏退出", "我游戏呢？", MessageBoxButton.OK, MessageBoxImage.Stop);
                            Close();
                        }
                        catch
                        {

                        }
                    });
                }
                catch { }

            }
            else if (cardid != id.Value )
            {
                if (id.Value % 8 != 0)
                    Debug.WriteLine(id.Value);
                cardid = id.Value;
                var ret = conn.Table<DBTables.Cards>().Where(v => v.id == id);

                foreach (var item in ret)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        vm.Card = item;
                    });

                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Border b = (Border)sender;
                var vm = (MainViewModel)this.DataContext;
                string name;
                if (b.Name == "cn")
                    name = vm.cn_name;
                else
                    name = vm.en_name;
                Clipboard.SetDataObject(name);
            }
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int GWL_STYLE = -16;
            int WS_MAXIMIZEBOX = 0x00010000;
            int SWP_NOSIZE = 0x0001;
            int SWP_NOMOVE = 0x0002;
            int SWP_FRAMECHANGED = 0x0020;
            IntPtr handle = new WindowInteropHelper(this).Handle;
            int nStyle = GetWindowLong(handle, GWL_STYLE);
            nStyle &= ~(WS_MAXIMIZEBOX);
            SetWindowLong(handle, GWL_STYLE, nStyle);
            SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_FRAMECHANGED);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                timer.Stop();
                Func.CloseMDProcess();
                conn.Close();
                var element = ConfigXML.Root?.Element("width");
                if (element != null)
                    element.Value = Width.ToString();
                element = ConfigXML.Root?.Element("height");
                if (element != null)
                    element.Value = Height.ToString();
                ConfigXML.Save(ConfigXMLPath);
            }
            catch { }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)sender ;
            Process.Start(new ProcessStartInfo("explorer",link.NavigateUri.AbsoluteUri));
        }
    }
}
