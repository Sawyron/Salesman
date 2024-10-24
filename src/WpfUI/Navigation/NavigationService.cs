using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfUI.Navigation;

public class NavigationService : ObservableObject
{
    private readonly Func<Type, ObservableObject> _viewModelFactory;

    public NavigationService(Func<Type, ObservableObject> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    private ObservableObject? _currentViewModel;
    public ObservableObject? CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
    {
        if (_currentViewModel is IDisposable disposable)
        {
            disposable.Dispose();
        }
        ObservableObject viewModel = _viewModelFactory(typeof(TViewModel));
        if (viewModel != _currentViewModel)
        {
            CurrentViewModel = viewModel;
        }
    }
}
