namespace ObservableBranch
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Subjects;

    public class ObservablePropertyChain : IObservable<object>
    {
        private readonly ObservableProperty property;
        private readonly string restOfPath;
        private readonly ISubject<object> subject = new Subject<object>();
        private ObservablePropertyChain child;
        private IDisposable childChanged;
        private object previousValue;

        public ObservablePropertyChain(INotifyPropertyChanged owner, string path)
        {
            var propertyParts = path.Split('.');
            var propertyName = propertyParts.First();
            property = new ObservableProperty(owner, propertyName);
            restOfPath = string.Concat(propertyParts.Skip(1));

            UpdateChild();

            previousValue = Value;
            property.Subscribe(OnNewPropertyValue);
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return subject.Subscribe(observer);
        }

        public object Value => child != null ? child.Value : property.Value;

        private void OnNewPropertyValue(object o)
        {
            UpdateChild();
            PushValue();
        }

        private void PushValue()
        {
            if (PreviousAndNewValuesAreDifferent)
            {
                subject.OnNext(Value);
            }

            previousValue = Value;
        }

        private bool PreviousAndNewValuesAreDifferent => !Equals(previousValue, Value);        

        private void UpdateChild()
        {
            childChanged?.Dispose();
            var notifyPropertyChanged = property.Value as INotifyPropertyChanged;

            if (notifyPropertyChanged != null && restOfPath.Length > 0)
            {
                child = new ObservablePropertyChain(notifyPropertyChanged, restOfPath);
                childChanged = child.Subscribe(OnNewChildValue);
            }
        }

        private void OnNewChildValue(object o)
        {            
            PushValue();
        }
    }
}