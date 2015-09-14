namespace ObservableBranch
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;

    public class ObservablePropertyBranch : IObservable<object>
    {
        private INotifyPropertyChanged owner;
        private readonly string propertyName;
        private ObservableProperty observableProperty;
        private ObservablePropertyBranch child;
        private IDisposable subs;
        private IDisposable childSubs;

        public ObservablePropertyBranch(INotifyPropertyChanged owner, string propertyName)
        {            
            this.propertyName = propertyName;
            this.Owner = owner;
        }

        private void OnNewValueForMyProperty(object o)
        {
            if (Child != null && o is INotifyPropertyChanged)
            {
                Child.Owner = (INotifyPropertyChanged) o;
            }
        }

        private INotifyPropertyChanged Owner
        {
            get { return owner; }
            set
            {
                subs?.Dispose();

                owner = value;

                observableProperty = new ObservableProperty(owner, propertyName);
                subs = observableProperty.Subscribe(OnNewValueForMyProperty);

                var newParent = observableProperty.Value as INotifyPropertyChanged;
                child.Owner = newParent;
            }
        }

        public ObservableProperty Property
        {
            get { return observableProperty; }
            set { observableProperty = value; }
        }

        public ObservablePropertyBranch Child
        {
            get { return child; }
            set
            {
                childSubs?.Dispose();

                child = value;

                if (child != null)
                {
                    childSubs = child.Subscribe(OnNewValueInChildBranch);
                }
            }
        }

        private void OnNewValueInChildBranch(object o)
        {
            
        }


        public IDisposable Subscribe(IObserver<object> observer)
        {
            return observableProperty.Merge(child).Subscribe(observer);
        }
    }
}