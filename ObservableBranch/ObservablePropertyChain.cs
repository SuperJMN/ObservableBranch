namespace ObservableBranch
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Runtime.InteropServices;

    public class ObservablePropertyChain : IObservable<object>
    {
        private readonly string path;
        private ObservableProperty observableProperty;
        private ObservablePropertyChain child;
        private readonly ObservableProperty property;
        private readonly string restOfPath;
        private IDisposable childChanged;
        private ISubject<object> subject = new Subject<object>();

        public ObservablePropertyChain(INotifyPropertyChanged owner, string path)
        {
            this.path = path;
            var propertyParts = path.Split('.');
            var propertyName = propertyParts.First();
            property = new ObservableProperty(owner, propertyName);
            restOfPath = string.Concat(propertyParts.Skip(1));

            SetChild();

            property.Subscribe(OnNewValue);
        }

        private void OnNewValue(object o)
        {
            SetChild();

            subject.OnNext(Value);
        }

        public object Value => child != null ? child.Value : property.Value;


        public string Path
        {
            get { return path; }
        }

        private void SetChild()
        {
            var notifyPropertyChanged = property.Value as INotifyPropertyChanged;
            childChanged?.Dispose();

            if (notifyPropertyChanged != null && restOfPath.Length > 0)
            {
                this.child = new ObservablePropertyChain(notifyPropertyChanged, restOfPath);
                this.childChanged = child.Subscribe(OnChildValueChanged);
            }
        }

        private void OnChildValueChanged(object o)
        {
            subject.OnNext(o);
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}