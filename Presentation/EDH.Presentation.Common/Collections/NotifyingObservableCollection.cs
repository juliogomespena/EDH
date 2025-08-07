using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace EDH.Presentation.Common.Collections;

public sealed class NotifyingObservableCollection<T> : ObservableCollection<T> 
    where T : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? ItemPropertyChanged;

    public NotifyingObservableCollection()
    {
    }

    public NotifyingObservableCollection(IEnumerable<T> collection)
        : base(collection)
    {
        foreach (var item in collection)
        {
            item.PropertyChanged += OnItemPropertyChanged;
        }
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (T item in e.OldItems)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
            }
        }

        if (e.NewItems != null)
        {
            foreach (T item in e.NewItems)
            {
                item.PropertyChanged += OnItemPropertyChanged;
            }
        }
        
        base.OnCollectionChanged(e);
    }
    
    protected override void ClearItems()
    {
        foreach (var item in Items)
        {
            item.PropertyChanged -= OnItemPropertyChanged;
        }
        base.ClearItems();
    }
    
    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        ItemPropertyChanged?.Invoke(sender, e);
    }

}