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
            var sut = new ObservablePropertyChain(changeSource, "Text");
            object actualText = null;

            sut.Subscribe(o => actualText = o);
            changeSource.Text = "Hello world";

            Assert.AreEqual("Hello world", actualText);
        }

        [TestMethod]
        public void SubscriptionToTwoLevelPath()
        {
            var changeSource = new DummyViewModel { Child = new DummyViewModel() };
            var sut = new ObservablePropertyChain(changeSource, "Child.Text");
            object actualText = null;

            sut.Subscribe(o => actualText = o);
            changeSource.Child.Text = "Hello world";

            Assert.AreEqual("Hello world", actualText);
        }

        [TestMethod]
        public void GivenSubscriptionToTwoLevelPath_WhenRootChanges_NotificationsShouldArrive()
        {
            var changeSource = new DummyViewModel { Child = new DummyViewModel() { Text = "Old text" } };
            var sut = new ObservablePropertyChain(changeSource, "Child.Text");
            object actualText = null;

            sut.Subscribe(o => actualText = o);
            changeSource.Child = new DummyViewModel { Text = "This is the real thing" };

            Assert.AreEqual("This is the real thing", actualText);
        }
    }
}
