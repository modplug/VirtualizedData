using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using VirtualizedData.Annotations;
using VirtualizedData.Services;
using Xamarin.Forms;

namespace VirtualizedData.ViewModels
{
    public class MoviesViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly CompositeDisposable _cds = new CompositeDisposable();
        private readonly ReadOnlyObservableCollection<MovieViewModel> _movies;
        private readonly IMoviesModel _moviesModel;
        private bool _isBusy;
        private int _lastLoadedPage;
        private bool _morePagesAvailable;
        private string _searchText;
        private MovieViewModel _selectedMovie;

        public MoviesViewModel(IMoviesModel moviesModel)
        {
            _moviesModel = moviesModel;
            _cds.Add(_moviesModel.RemainingPages.Subscribe(OnRemainingPagesChanged));
            var itemsObservable = _moviesModel.Movies.Connect();
            var operations = itemsObservable
                .Transform(CreateEntryViewModel)
                .Bind(out _movies)
                .Subscribe();
            _cds.Add(operations);

            // Throttle search for 100ms
            var searchTextChanged = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                ev => PropertyChanged += ev,
                ev => PropertyChanged -= ev)
                .Where(ev => ev.EventArgs.PropertyName == "SearchText")
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Select(args => SearchText);

            _cds.Add(searchTextChanged.Subscribe(pattern => Search()));
        }

        public ReadOnlyObservableCollection<MovieViewModel> Movies => _movies;

        public MovieViewModel SelectedMovie
        {
            get { return _selectedMovie; }
            set
            {
                _selectedMovie = value;
                OnPropertyChanged(nameof(SelectedMovie));
            }
        }

        public ICommand LoadNextPageCommand
        {
            get { return new Command(async () => await LoadPage(_lastLoadedPage + 1)); }
        }

        public ICommand SearchCommand => new Command(Search);

        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public bool MorePagesAvailable
        {
            get { return _morePagesAvailable; }
            private set
            {
                _morePagesAvailable = value;
                OnPropertyChanged(nameof(MorePagesAvailable));
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText == value)
                    return;
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        public void Dispose()
        {
            _cds.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnRemainingPagesChanged(int remainingPages)
        {
            MorePagesAvailable = remainingPages != 0;
        }

        private void Search()
        {
            Task.Run(async () =>
            {
                try
                {
                    IsBusy = true;
                    await _moviesModel.GetPage(_searchText, 1);
                    _lastLoadedPage = 1;
                    IsBusy = false;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Something went wrong while searching for movie");
                }
            });
        }

        public async Task LoadPage(int pageNumber)
        {
            IsBusy = true;
            await _moviesModel.GetPage(_searchText, pageNumber);
            _lastLoadedPage = pageNumber;
            Debug.WriteLine("Current page: " + pageNumber);
            IsBusy = false;
        }

        private MovieViewModel CreateEntryViewModel(Movie movie)
        {
            return new MovieViewModel {ImageUrl = movie.Poster, Title = movie.Title, Year = movie.Year};
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}