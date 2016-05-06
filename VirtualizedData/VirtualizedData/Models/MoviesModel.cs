using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using VirtualizedData.Services;

namespace VirtualizedData.Models
{
    public class MoviesModel : IMoviesModel
    {
        private readonly IDataService _dataService;
        private readonly SourceList<Movie> _moviesInternalList;
        private CancellationTokenSource _cts;
        private string _query;
        private bool _searchActive;


        public MoviesModel(IDataService dataService)
        {
            _dataService = dataService;
            _moviesInternalList = new SourceList<Movie>();
            RemainingPages = new Subject<int>();
            _cts = new CancellationTokenSource();
        }

        public IObservableList<Movie> Movies => _moviesInternalList;
        public Subject<int> RemainingPages { get; set; }

        public async Task GetPage(string query, int pageNumber)
        {
            _searchActive = true;
            if (string.IsNullOrEmpty(query))
            {
                RemainingPages.OnNext(0);
                _moviesInternalList.Clear();
                _searchActive = false;
                return;
            }

            // Cancel search
            if (_searchActive)
            {
                _cts.Cancel();
                _cts = new CancellationTokenSource();
            }

            try
            {
                var result = await _dataService.GetItemsAsync(query, pageNumber, _cts.Token);
                if (_query != query)
                {
                    _moviesInternalList.Clear();
                }
                if (result.Response)
                {
                    _moviesInternalList.Edit(list => list.AddRange(result.Movies));
                    RemainingPages.OnNext(result.RemainingPages);
                    _query = query;
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Cancelled previous search");
                _searchActive = false;
                await GetPage(query, pageNumber);

            }
            catch (Exception e)
            {
                Debug.WriteLine("Something unexpected happened while fetching data");
            }
            finally
            {
                _searchActive = false;
            }
        }
    }
}

public interface IMoviesModel
{
    IObservableList<Movie> Movies { get; }
    Subject<int> RemainingPages { get; set; }
    Task GetPage(string query, int pageNumber);
}