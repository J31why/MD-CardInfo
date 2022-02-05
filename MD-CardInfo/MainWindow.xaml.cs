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
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = ViewModel.VM;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int GWL_STYLE = -16;
            int WS_MAXIMIZEBOX = 0x00010000;
            IntPtr handle = new WindowInteropHelper(this).Handle;
            var value = GetWindowLong(handle, GWL_STYLE);
            _=SetWindowLong(handle, GWL_STYLE, (int)(value & ~WS_MAXIMIZEBOX));
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            ViewModel.VM.Close();
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)sender ;
            Process.Start(new ProcessStartInfo("explorer",link.NavigateUri.AbsoluteUri));
        }
    }
}
