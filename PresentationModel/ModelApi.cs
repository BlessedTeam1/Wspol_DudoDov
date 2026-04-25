using System.Collections.ObjectModel;
using System.Collections.Specialized;
using BusinessLogic;
using Data;

namespace PresentationModel
{
    public class ModelApi
    {
        private readonly LogicAbsApi _logicApi;

       
        public ObservableCollection<BallModel> Balls { get; } = new ObservableCollection<BallModel>();

        public ModelApi(LogicAbsApi logicApi = null)
        {
            
            _logicApi = logicApi ?? LogicAbsApi.CreateApi();

            
            _logicApi.GetBalls().CollectionChanged += LogicBalls_CollectionChanged;
        }

        public void Start(double width, double height, int count)
        {
            _logicApi.StartSimulation(width, height, count);
        }

        public void Stop()
        {
            _logicApi.StopSimulation();
        }

        
        private void LogicBalls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (Iballs ball in e.NewItems)
                {
                    Balls.Add(new BallModel(ball)); 
                }
            }
            
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                Balls.Clear(); 
            }
        }
    }
}