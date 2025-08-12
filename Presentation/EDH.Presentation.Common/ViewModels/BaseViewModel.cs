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
        if (String.IsNullOrEmpty(propertyName))
            return _errors.Values.SelectMany(list => list);

        return _errors.TryGetValue(propertyName, out var messages)
            ? messages
            : Array.Empty<string>();
    }

    public int GetErrorCount(string? propertyName = null) =>
        String.IsNullOrWhiteSpace(propertyName)
            ? _errors.Values.Sum(list => list.Count)
            : _errors.TryGetValue(propertyName, out var messages) ? messages.Count : 0;

    protected void SetError(string propertyName, string errorMessage)
    {
        _errors[propertyName] = [errorMessage];
        RaiseErrorsChanged(propertyName);
        RaiseErrorsChanged(String.Empty);
        RaisePropertyChanged(nameof(HasErrors));
    }
    
    protected void ClearError(string propertyName)
    {
        if (!_errors.Remove(propertyName)) return;
        
        RaiseErrorsChanged(propertyName);
        RaiseErrorsChanged(String.Empty);
        RaisePropertyChanged(nameof(HasErrors));
        RaisePropertyChanged(String.Empty);
    }
    
    private void RaiseErrorsChanged(string? propertyName) =>
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
}