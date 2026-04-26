using System.Collections.ObjectModel;
using System.Collections.Specialized;
using BusinessLogic;
using Data;

namespace PresentationModel
{
    public class ModelApi
    {
        private readonly LogicAbsApi _logicApi;
        private double _logicWidth, _logicHeight;
        private double _canvasWidth, _canvasHeight;

        public ObservableCollection<BallModel> Balls { get; } = new ObservableCollection<BallModel>();

        public ModelApi(LogicAbsApi logicApi = null)
        {
            _logicApi = logicApi ?? LogicAbsApi.CreateApi();
            _logicApi.GetBalls().CollectionChanged += OnLogicBallsChanged;
        }

        public void Start(double canvasWidth, double canvasHeight, int count)
        {
            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;
            _logicWidth = canvasWidth;
            _logicHeight = canvasHeight;
            _logicApi.StartSimulation(_logicWidth, _logicHeight, count);
        }

        public void Stop() => _logicApi.StopSimulation();

        private void OnLogicBallsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
                foreach (Iballs ball in e.NewItems)
                    Balls.Add(new BallModel(ball, _logicWidth, _logicHeight,
                                                  _canvasWidth, _canvasHeight));
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
                Balls.Clear();
        }
    }
}