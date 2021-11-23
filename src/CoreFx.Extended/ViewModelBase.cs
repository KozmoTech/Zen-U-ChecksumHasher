using System.ComponentModel;

namespace KozmoTech.System.ComponentModel;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
}
