using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PresentationModel;

namespace PresentationViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ModelApi _modelApi;

        private int _ballCount = 5; 
        private double _canvasWidth = 800; 
        private double _canvasHeight = 400; 

        public MainViewModel()
        {
            _modelApi = new ModelApi();

            
            StartCommand = new RelayCommand(StartSimulation);
            StopCommand = new RelayCommand(StopSimulation);
        }

        
        public ObservableCollection<BallModel> Balls => _modelApi.Balls;

        
        public int BallCount
        {
            get => _ballCount;
            set
            {
                _ballCount = value;
                OnPropertyChanged();
            }
        }

        
        public double CanvasWidth
        {
            get => _canvasWidth;
            set { _canvasWidth = value; OnPropertyChanged(); }
        }

        public double CanvasHeight
        {
            get => _canvasHeight;
            set { _canvasHeight = value; OnPropertyChanged(); }
        }

       
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        private void StartSimulation()
        {
            _modelApi.Start(CanvasWidth, CanvasHeight, BallCount);
        }

        private void StopSimulation()
        {
            _modelApi.Stop();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
