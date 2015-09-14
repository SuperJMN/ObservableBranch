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
            var changeSource = new DummyViewModel { Child = new DummyViewModel { Text = "Old text" } };
            var sut = new ObservablePropertyChain(changeSource, "Child.Text");
            object actualText = null;

            sut.Subscribe(o => actualText = o);
            changeSource.Child = new DummyViewModel { Text = "This is the real thing" };

            Assert.AreEqual("This is the real thing", actualText);
        }

        [TestMethod]
        public void GivenSubscriptionToTwoLevelPath_WhenRootChangesButValuesAreSame_NoNotificationIsSent()
        {
            var changeSource = new DummyViewModel { Child = new DummyViewModel { Text = "Same" } };
            var sut = new ObservablePropertyChain(changeSource, "Child.Text");

            bool hit = false;
            sut.Subscribe(o => hit = true);
            changeSource.Child = new DummyViewModel { Text = "Same" };

            Assert.IsFalse(hit);
        }

        [TestMethod]
        public void GivenSubscriptionToTwoLevelPath_WhenRootChangesInDifferentSteps_NotificationsShouldArrive()
        {
            var changeSource = new DummyViewModel { Child = new DummyViewModel { Text = "Old text" } };
            var sut = new ObservablePropertyChain(changeSource, "Child.Text");
            object actualText = null;

            sut.Subscribe(o => actualText = o);
            changeSource.Child = new DummyViewModel();
            changeSource.Child.Text = "This is the real thing";

            Assert.AreEqual("This is the real thing", actualText);
        }
    }
}
