using System.Windows;
using System.Windows.Input;

namespace WpfUi;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        MessageBox.Show(
            $"{e.Source}{Environment.NewLine}{e.OriginalSource}",
            "Canvas clicked",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}