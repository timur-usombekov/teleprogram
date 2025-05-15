using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Teleprogram.Models;

namespace Teleprogram.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TvShow> AllShows { get; set; }
        public ObservableCollection<TvShow> FilteredShows { get; set; }
        public ObservableCollection<TvShow> Favorites { get; set; }
        public ObservableCollection<TvShow> Planned { get; set; }
        public Array GenreList => Enum.GetValues(typeof(Genre));

        private Genre? selectedGenre;

        public Genre? SelectedGenre
        {
            get => selectedGenre;
            set
            {
                selectedGenre = value;
                OnPropertyChanged(nameof(SelectedGenre));
                FilterByGenre();
            }
        }

        public MainViewModel()
        {
            AllShows = new ObservableCollection<TvShow>();
            FilteredShows = new ObservableCollection<TvShow>();
            Favorites = new ObservableCollection<TvShow>();
            Planned = new ObservableCollection<TvShow>();

            LoadMockData();
            FilterByGenre();
        }

        private void LoadMockData()
        {
            AllShows.Add(new TvShow { Title = "Новини", Channel = "1+1", StartTime = DateTime.Today.AddHours(9), Duration = TimeSpan.FromMinutes(30), Genre = Genre.News });
            AllShows.Add(new TvShow { Title = "Футбол", Channel = "ICTV", StartTime = DateTime.Today.AddHours(20), Duration = TimeSpan.FromMinutes(90), Genre = Genre.Sports });
            AllShows.Add(new TvShow { Title = "Фільм: Форрест Гамп", Channel = "СТБ", StartTime = DateTime.Today.AddHours(22), Duration = TimeSpan.FromMinutes(120), Genre = Genre.Movie });
            // ... додайте ще
        }

        private void FilterByGenre()
        {
            FilteredShows.Clear();
            var shows = selectedGenre == null ? AllShows : AllShows.Where(s => s.Genre == selectedGenre);
            foreach (var show in shows)
            {
                FilteredShows.Add(show);
            }
        }

        public void AddToFavorites(TvShow show)
        {
            if (!Favorites.Contains(show))
                Favorites.Add(show);
        }

        public void AddToPlanned(TvShow show)
        {
            if (!Planned.Contains(show))
                Planned.Add(show);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
