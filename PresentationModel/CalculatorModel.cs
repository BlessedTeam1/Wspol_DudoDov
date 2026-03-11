namespace PresentationModel
{
    public class CalculatorModel
    {
        public double FirstNumber { get; set; }
        public double SecondNumber { get; set; }
        public string SelectedOperation { get; set; } = "+";
        public double Result { get; set; }
        public string ErrorMessage { get; set; }
    }
}
