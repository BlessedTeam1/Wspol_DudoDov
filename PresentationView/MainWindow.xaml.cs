using System.Windows;
using PresentationViewModel; // Niezbędne odwołanie do ViewModelu

namespace PresentationView
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }
    }
}
