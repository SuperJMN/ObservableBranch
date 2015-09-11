using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ObservableBranch
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Annotations;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var changeSource = new DummyViewModel();
            var observablePropertyBranch = new ObservableProperty(changeSource, "Number");
            object actual = null;
            observablePropertyBranch.Subscribe(o => actual = o);
            changeSource.Number = 11;
            Assert.AreEqual(11, actual);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var changeSource = new DummyViewModel();
            changeSource.Child = new DummyViewModel() { Number = 1 };


            var observablePropertyBranch = new ObservablePropertyBranch.Create(changeSource, "Child.Number");

            object actual = null;
            observablePropertyBranch.Subscribe(o => actual = o);
            changeSource.Child = new DummyViewModel() { Number = 2 };

            Assert.AreEqual(2, actual);
        }
    }

    public class ObservablePropertyBranch : IObservable<object>
    {
        private ObservableProperty observableProperty;

        public ObservablePropertyBranch Create(INotifyPropertyChanged root, string path)
        {
            
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            throw new NotImplementedException();
        }

    }


    public class ObservableProperty : IObservable<object>
    {
        private readonly string propertyName;
        private readonly INotifyPropertyChanged notifySource;
        private readonly PropertyInfo propertyInfo;

        public ObservableProperty(INotifyPropertyChanged changeSource, string propertyName)
        {
            this.propertyName = propertyName;
            propertyInfo = changeSource.GetType().GetProperty(propertyName);
            notifySource = changeSource as INotifyPropertyChanged;
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                p => notifySource.PropertyChanged += p,
                p => notifySource.PropertyChanged -= p)
                .Where(pattern => pattern.EventArgs.PropertyName == propertyName)
                .Select(_ => Value)
                .Subscribe(observer);
        }

        public object Value
        {
            get { return propertyInfo.GetValue(notifySource); }
            set { propertyInfo.SetValue(notifySource, value); }
        }
    }

    public class DummyViewModel : ViewModel
    {
        private int number;
        private DummyViewModel child;

        public int Number
        {
            get { return number; }
            set
            {
                if (value == number)
                {
                    return;
                }
                number = value;
                OnPropertyChanged();
            }
        }

        public DummyViewModel Child
        {
            get { return child; }
            set
            {
                if (Equals(value, child))
                {
                    return;
                }
                child = value;
                OnPropertyChanged();
            }
        }
    }

    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PropertyPath
    {
        private readonly string[] parts;

        public PropertyPath(string str)
        {
            parts = str.Split('.');
        }

        public IEnumerable<string> Parts => parts;

        public int PartCount => parts.Length;
    }
}
