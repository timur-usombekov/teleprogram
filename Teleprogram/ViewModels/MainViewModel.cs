using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Teleprogram.Commands;
using Teleprogram.Models;
using Teleprogram.Services;

namespace Teleprogram.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TvProgramDataService _dataService;

        #region Collections
        public ObservableCollection<TvShow> AllShows { get; set; } = new();
        public ObservableCollection<TvShow> FilteredShows { get; set; } = new();
        public ObservableCollection<TvShow?> FavoritesShows { get; set; } = new();
        public ObservableCollection<PlannedShow> PlannedShows { get; set; } = new(); 

        public ObservableCollection<TVChannel> ChannelList { get; set; } = new(); 
        public ObservableCollection<string> Genres { get; set; } = new();
        public ObservableCollection<string> DaysOfWeek { get; set; } = new();
        public ObservableCollection<string> Times { get; set; } = new();
        # endregion Collections

        public string? SelectedDay { get; set; }
        public string? SelectedTime { get; set; }
        public TVChannel? SelectedChannel { get; set; } 
        public string? SelectedGenre { get; set; }

        public TvShow? SelectedShow { get; set; }
        public TvShow? SelectedFavorite { get; set; }
        public PlannedShow? SelectedPlannedShow { get; set; }

        public DateTime PlannedDate { get; set; } = DateTime.Today;
        public string PlannedTime { get; set; } = "9:00";

        #region ICommands
        public ICommand SearchCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand FavoriteCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand AddToPlanCommand { get; set; }
        public ICommand ChannelInfoCommand { get; set; }
        public ICommand RemovePlannedShowCommand { get; }
        #endregion ICommands

        public MainViewModel()
        {
            _dataService = new TvProgramDataService();

            SearchCommand = new RelayCommand(FilterShows);
            ClearCommand = new RelayCommand(ClearFilters);
            FavoriteCommand = new RelayCommand(MakeFavorite);
            RemoveCommand = new RelayCommand(RemoveFavorite);
            AddToPlanCommand = new RelayCommand(AddToPlan);
            ChannelInfoCommand = new RelayCommand(GetChannelDescription);
            RemovePlannedShowCommand = new RelayCommand(RemovePlannedShow);

            LoadInitialData();
            PopulateFilters();
            FilterShows();
        }
        private void LoadInitialData()
        {
            try
            {
                AllShows = _dataService.LoadAllShows();

                FavoritesShows.Clear();
                foreach (var fav in _dataService.LoadFavorites())
                {
                    FavoritesShows.Add(fav);
                }

                PlannedShows.Clear();
                foreach (var planned in _dataService.LoadPlannedShows())
                {
                    PlannedShows.Add(planned);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при початковому завантаженні даних: {ex.Message}", "Помилка завантаження", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

            Times.Add("00:00 - 05:59");
            Times.Add("06:00 - 11:59");
            Times.Add("12:00 - 17:59");
            Times.Add("18:00 - 23:59");

            var allChannels = AllShows.Select(s => s.Channel)
                                      .DistinctBy(c => c.Name)
                                      .OrderBy(c => c.Name);
            ChannelList.Clear();
            foreach (var ch in allChannels)
                ChannelList.Add(ch);

            var allGenres = AllShows.Select(s => s.Genre).Distinct();
            Genres.Clear();
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
            {
                var parts = SelectedTime.Split('-');
                if (parts.Length == 2 &&
                    TimeSpan.TryParse(parts[0].Trim(), out var from) &&
                    TimeSpan.TryParse(parts[1].Trim(), out var to))
                {
                    filtered = filtered.Where(s =>
                        s.Date.TimeOfDay >= from && s.Date.TimeOfDay <= to);
                }
            }

            if (SelectedChannel != null)
                filtered = filtered.Where(s => s.Channel.Name == SelectedChannel.Name);

            if (!string.IsNullOrEmpty(SelectedGenre))
                filtered = filtered.Where(s => s.Genre == SelectedGenre);

            FilteredShows.Clear();
            foreach (var show in filtered)
                FilteredShows.Add(show);
        }

        private void GetChannelDescription()
        {
            if (SelectedShow != null)
            {
                var desk = SelectedShow.Channel.Description is null ? "Опис каналу не знайдено" : SelectedShow.Channel.Description;
                if (desk is null)
                {
                    MessageBox.Show($"Опис канал ще не додано", "Інформація про канал", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                MessageBox.Show($"Опис каналу:\n'{SelectedShow.Channel.Description}'", "Інформація про канал", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else if (SelectedShow is null)
            {
                MessageBox.Show($"Ви не обрали канал!", "Інформація про канал", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBox.Show($"Йой! Сталася помилка, спробуйте ще раз пізніше", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
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
            {
                FavoritesShows.Add(SelectedShow);
                _dataService.SaveFavorites(FavoritesShows);
                MessageBox.Show($"'{SelectedShow.Title}' додано в улюблені!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else if (SelectedShow != null && FavoritesShows.Contains(SelectedShow))
            {
                MessageBox.Show($"Ви вже додали '{SelectedShow.Title}' до обраних!", "Попередження!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else if (SelectedShow is null)
            {
                MessageBox.Show($"Ви не обрали телепередачу!", "Оберіть телепередачу!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBox.Show($"Йой! Сталася помилка, спробуйте ще раз пізніше", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void RemoveFavorite()
        {
            if (SelectedShow != null)
            {
                FavoritesShows.Remove(SelectedShow);
                _dataService.SaveFavorites(FavoritesShows);
                OnPropertyChanged(nameof(SelectedShow));
                return;
            }
            MessageBox.Show($"Ви не обрали телепередачу!", "Оберіть телепередачу!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void AddToPlan()
        {
            if (SelectedFavorite == null || string.IsNullOrWhiteSpace(PlannedTime))
            {
                MessageBox.Show("Будь ласка, оберіть передачу та введіть час для планування.", "Помилка введення", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TimeSpan time;
            string[] formats = { @"hh\:mm", @"h\:mm" };
            if (!TimeSpan.TryParseExact(PlannedTime.Trim(), formats, System.Globalization.CultureInfo.InvariantCulture, out time))
            {
                MessageBox.Show("Формат часу має бути HH:mm (наприклад, 09:00 або 14:30). Будь ласка, введіть час з двокрапкою.", "Некоректний формат часу", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedFavorite.Date > PlannedDate + time)
            {
                MessageBox.Show("Ця передача ще не відбулася. Ви не можете її планувати.",
                              "Помилка планування", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var plannedFullDateTime = DateTime.Today.Date + time; 

            if (plannedFullDateTime < DateTime.Now)
            {
                MessageBox.Show("Не можна планувати передачу на минулий час поточного дня. Оберіть майбутній час.", "Помилка планування", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool timeSlotOccupied = PlannedShows.Any(ps =>
                    ps.PlannedDateTime.Date == PlannedDate.Date && 
                    ps.PlannedDateTime.TimeOfDay == PlannedDate.TimeOfDay
                );

            if (timeSlotOccupied)
            {
                MessageBox.Show($"О {plannedFullDateTime:HH:mm} вже запланована інша передача. Оберіть інший час.", "Час зайнятий", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            bool exactDuplicate = PlannedShows.Any(ps => ps.Show.Equals(SelectedFavorite) && ps.PlannedDateTime.Equals(plannedFullDateTime));
            if (exactDuplicate)
            {
                MessageBox.Show("Ця передача вже запланована на цей час.", "Повторне планування", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            PlannedShows.Add(new PlannedShow
            {
                Show = SelectedFavorite,
                PlannedDateTime = PlannedDate,
            });

            _dataService.SavePlannedShows(PlannedShows); 
            MessageBox.Show("Передачу успішно додано до плану перегляду!", "Планування успішне", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void RemovePlannedShow()
        {
            if (SelectedPlannedShow != null)
            {
                PlannedShows.Remove(SelectedPlannedShow);
                _dataService.SavePlannedShows(PlannedShows);
                OnPropertyChanged(nameof(SelectedPlannedShow));
                return;
            }
            MessageBox.Show($"Ви не обрали телепередачу!", "Оберіть телепередачу!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string? prop) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
