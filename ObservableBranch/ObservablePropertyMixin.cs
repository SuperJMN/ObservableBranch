namespace ObservableBranch
{
    using System.ComponentModel;
    using System.Linq;

    public static class ObservablePropertyMixin
    {
        public static ObservablePropertyBranch Join(this ObservablePropertyBranch parent, string propertyName)
        {
            var value = (INotifyPropertyChanged) parent.Property.Value;
            parent.Child = new ObservablePropertyBranch(value, propertyName);
            return parent;
        }

        public static ObservablePropertyBranch CreateObservablePropertyBranch(this INotifyPropertyChanged parent, string path)
        {
            var parts = path.Split('.');

            var first = parts.First();
            var rest = parts.Skip(1);

            var branch = new ObservablePropertyBranch(parent, first);
            foreach (var part in rest)
            {                
                branch.Join(part);
            }

            return branch;
        }
    }
}