namespace ObservableBranch
{
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
}