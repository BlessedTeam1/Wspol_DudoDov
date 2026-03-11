using PresentationViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PresentationView
{
    public partial class MainWindow : Window
    {
        private readonly CalculatorViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = CalculatorViewModel.CreateDefault();
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(TxtFirstNumber.Text, out double a) ||
                !double.TryParse(TxtSecondNumber.Text, out double b))
            {
                TxtResult.Text = "Invalid input!";
                return;
            }

            _viewModel.Model.FirstNumber = a;
            _viewModel.Model.SecondNumber = b;
            _viewModel.Model.SelectedOperation = ((ComboBoxItem)CmbOperation.SelectedItem).Content.ToString();

            _viewModel.Calculate();

            if (_viewModel.Model.ErrorMessage != null)
                TxtResult.Text = $"Error: {_viewModel.Model.ErrorMessage}";
            else
                TxtResult.Text = _viewModel.Model.Result.ToString();
        }
    }
}
