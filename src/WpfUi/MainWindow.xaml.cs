using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace WpfUI.UI.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (Application.Current is App app)
            {
                DataContext = app.Services.GetRequiredService<MainViewModel>();
            }
        }

        private void Window_Closed(object sender, EventArgs e) => Environment.Exit(0);
    }
}
