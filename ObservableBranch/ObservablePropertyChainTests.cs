namespace ObservableBranch
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ObservablePropertyChainTests
    {
        [TestMethod]
        public void SubscriptionToOneLevelPath()
        {
            var changeSource = new DummyViewModel();
            var observablePropertyBranch = new ObservablePropertyChain(changeSource, "Number");
            object actual = null;            
            observablePropertyBranch.Subscribe(o => actual = o);
            changeSource.Number = 11;
            Assert.AreEqual(11, actual);
        }
    }
}
