using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Teleprogram.Commands;
using Teleprogram.Models;

namespace Teleprogram.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TvShow> AllShows { get; set; } = new();
        public ObservableCollection<TvShow> FilteredShows { get; set; } = new();
        public ObservableCollection<TvShow?> FavoritesShows { get; set; } = new();
        public ObservableCollection<TvShow> PlannedShows { get; set; } = new();

        public ObservableCollection<string> Channels { get; set; } = new();
        public ObservableCollection<string> Genres { get; set; } = new();
        public ObservableCollection<string> DaysOfWeek { get; set; } = new();
        public ObservableCollection<string> Times { get; set; } = new();

        public string? SelectedDay { get; set; }
        public string? SelectedTime { get; set; }
        public string? SelectedChannel { get; set; }
        public string? SelectedGenre { get; set; }
        public TvShow? SelectedShow { get; set; }


        public TvShow? SelectedFavorite { get; set; }
        public DateTime PlannedDate { get; set; } = DateTime.Today;
        public string PlannedTime { get; set; } = "18:00";

        public ICommand SearchCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand FavoriteCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand AddToPlanCommand { get; set; }

        public MainViewModel()
        {
            SearchCommand = new RelayCommand(FilterShows);
            ClearCommand = new RelayCommand(ClearFilters);
            FavoriteCommand = new RelayCommand(MakeFavorite);
            RemoveCommand = new RelayCommand(RemoveFavorite);
            AddToPlanCommand = new RelayCommand(AddToPlan);

            LoadMockData();
            PopulateFilters();
            FilterShows();
        }

        private void LoadMockData()
        {
            AllShows.Add(new TvShow { Date = DateTime.Today.AddHours(9), Channel = "1+1", Genre = "Новини", Title = "Ранкові новини" });
            AllShows.Add(new TvShow { Date = DateTime.Today.AddHours(20), Channel = "ICTV", Genre = "Фільм", Title = "Термінатор" });
            AllShows.Add(new TvShow { Date = DateTime.Today.AddHours(18), Channel = "СТБ", Genre = "Серіал", Title = "Кріпосна" });
            AllShows.Add(new TvShow { Date = DateTime.Today.AddDays(1).AddHours(18), Channel = "1+1", Genre = "Спорт", Title = "Футбол LIVE" });
            AllShows.Add(new TvShow { Date = DateTime.Today.AddDays(2).AddHours(21), Channel = "Новий", Genre = "Фільм", Title = "Гаррі Поттер і філософський камінь" });
            AllShows.Add(new TvShow { Date = DateTime.Today.AddDays(3).AddHours(8), Channel = "ICTV", Genre = "Новини", Title = "Ранковий випуск новин" });
            AllShows.Add(new TvShow { Date = DateTime.Today.AddDays(3).AddHours(19), Channel = "СТБ", Genre = "Серіал", Title = "Таємниці" });
            AllShows.Add(new TvShow { Date = DateTime.Today.AddDays(4).AddHours(22), Channel = "Новий", Genre = "Фільм", Title = "Володар перснів: Дві вежі" });
            AllShows.Add(new TvShow { Date = DateTime.Today.AddDays(5).AddHours(17), Channel = "1+1", Genre = "Спорт", Title = "Бокс: Чемпіонський бій" });
            AllShows.Add(new TvShow { Date = DateTime.Today.AddDays(6).AddHours(14), Channel = "СТБ", Genre = "Інше", Title = "Документальний фільм: Природа Карпат" });
            AllShows.Add(new TvShow { Date = DateTime.Today.AddDays(6).AddHours(10), Channel = "ICTV", Genre = "Новини", Title = "Тижневий дайджест" });
        }

        private void PopulateFilters()
        {
            DaysOfWeek.Add("Понеділок");
            DaysOfWeek.Add("Вівторок");
            DaysOfWeek.Add("Середа");
            DaysOfWeek.Add("Четвер");
            DaysOfWeek.Add("П’ятниця");
            DaysOfWeek.Add("Субота");
            DaysOfWeek.Add("Неділя");

            for (int i = 0; i < 24; i++)
            {
                Times.Add(i.ToString("D2") + ":00");
            }

            var allChannels = AllShows.Select(s => s.Channel).Distinct();
            foreach (var ch in allChannels)
                Channels.Add(ch);

            var allGenres = AllShows.Select(s => s.Genre).Distinct();
            foreach (var g in allGenres)
                Genres.Add(g);
        }

        private void FilterShows()
        {
            var filtered = AllShows.AsEnumerable();

            if (!string.IsNullOrEmpty(SelectedDay))
            {
                var dayIndex = DaysOfWeek.IndexOf(SelectedDay);
                filtered = filtered.Where(s => (int)s.Date.DayOfWeek == ((dayIndex + 1) % 7));
            }

            if (!string.IsNullOrEmpty(SelectedTime))
                filtered = filtered.Where(s => s.Time == SelectedTime);

            if (!string.IsNullOrEmpty(SelectedChannel))
                filtered = filtered.Where(s => s.Channel == SelectedChannel);

            if (!string.IsNullOrEmpty(SelectedGenre))
                filtered = filtered.Where(s => s.Genre == SelectedGenre);

            FilteredShows.Clear();
            foreach (var show in filtered)
                FilteredShows.Add(show);
        }

        private void ClearFilters()
        {
            SelectedDay = null;
            SelectedTime = null;
            SelectedChannel = null;
            SelectedGenre = null;

            OnPropertyChanged(nameof(SelectedDay));
            OnPropertyChanged(nameof(SelectedTime));
            OnPropertyChanged(nameof(SelectedChannel));
            OnPropertyChanged(nameof(SelectedGenre));

            FilteredShows.Clear();
            foreach (var show in AllShows)
                FilteredShows.Add(show);
        }
        private void MakeFavorite()
        {
            if (SelectedShow != null && !FavoritesShows.Contains(SelectedShow))
                FavoritesShows.Add(SelectedShow);
        }
        private void RemoveFavorite()
        {
            if (SelectedShow != null)
                FavoritesShows.Remove(SelectedShow);
        }

        private void AddToPlan()
        {
            if (SelectedFavorite == null || string.IsNullOrWhiteSpace(PlannedTime))
                return;

            if (!TimeSpan.TryParse(PlannedTime, out var time))
                return;

            var dateTime = PlannedDate.Date + time;

            PlannedShows.Add(new TvShow
            {
                Title = SelectedFavorite.Title,
                Channel = SelectedFavorite.Channel,
                Genre = SelectedFavorite.Genre,
                Date = dateTime
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string? prop) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
