using System.Collections;
using System.ComponentModel;

namespace EDH.Presentation.Common.ViewModels;

public abstract class BaseViewModel : BindableBase, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _errors = [];
    
    public bool HasErrors => _errors.Count > 0;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (String.IsNullOrEmpty(propertyName) || !_errors.TryGetValue(propertyName, out var value))
            return Array.Empty<string>();
        
        return value;
    }

    protected void SetError(string propertyName, string errorMessage)
    {
        _errors[propertyName] = [errorMessage];
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
    
    protected void ClearError(string propertyName)
    {
        if (!_errors.Remove(propertyName)) return;

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}