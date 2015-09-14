namespace ObservableBranch
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using System.Reflection;

    public class ObservableProperty : IObservable<object>
    {
        private readonly string propertyName;
        private readonly INotifyPropertyChanged notifySource;
        private readonly PropertyInfo propertyInfo;

        public ObservableProperty(INotifyPropertyChanged changeSource, string propertyName)
        {
            this.propertyName = propertyName;
            propertyInfo = changeSource.GetType().GetProperty(propertyName);
            notifySource = changeSource;
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                p => notifySource.PropertyChanged += p,
                p => notifySource.PropertyChanged -= p)
                .Where(pattern => pattern.EventArgs.PropertyName == propertyName)
                .Select(_ => Value)
                .Distinct()
                .Subscribe(observer);
        }

        public object Value
        {
            get { return propertyInfo.GetValue(notifySource); }
            set { propertyInfo.SetValue(notifySource, value); }
        }
    } 
}