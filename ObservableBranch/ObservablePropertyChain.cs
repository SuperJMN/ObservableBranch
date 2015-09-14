namespace ObservableBranch
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Runtime.InteropServices;

    public class ObservablePropertyChain : IObservable<object>
    {
        private readonly string path;
        private ObservableProperty observableProperty;
        private ObservablePropertyChain child;
        private readonly ObservableProperty property;
        private readonly string restOfPath;
        private IObservable<object> observable;

        public ObservablePropertyChain(INotifyPropertyChanged owner, string path)
        {
            this.path = path;
            var propertyParts = path.Split('.');
            var propertyName = propertyParts.First();
            property = new ObservableProperty(owner, propertyName);
            restOfPath = string.Concat(propertyParts.Skip(1));

            SetChild();

            observable = child != null ? property.Merge(child) : property;
        }

        public string Path
        {
            get { return path; }
        }

        private void SetChild()
        {
            var notifyPropertyChanged = property.Value as INotifyPropertyChanged;

            if (notifyPropertyChanged != null && restOfPath.Length > 0)
            {
                this.child = new ObservablePropertyChain(notifyPropertyChanged, restOfPath);
            }
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return observable.Subscribe(observer);
        }
    }
}