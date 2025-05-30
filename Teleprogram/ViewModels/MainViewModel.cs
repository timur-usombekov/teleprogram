using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Teleprogram.Commands;
using Teleprogram.Models;

namespace Teleprogram.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
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
        public string PlannedTime { get; set; } = "18:00";
        
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
            SearchCommand = new RelayCommand(FilterShows);
            ClearCommand = new RelayCommand(ClearFilters);
            FavoriteCommand = new RelayCommand(MakeFavorite);
            RemoveCommand = new RelayCommand(RemoveFavorite);
            AddToPlanCommand = new RelayCommand(AddToPlan);
            ChannelInfoCommand = new RelayCommand(GetChannelDescription);
            RemovePlannedShowCommand = new RelayCommand(RemovePlannedShow);

            LoadDataFromJson();
            PopulateFilters();
            FilterShows();
        }


        private void LoadDataFromJson()
        {
            string path = "tvshows.json";

            if (!File.Exists(path))
                return;

            try
            {
                string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
                var shows = JsonSerializer.Deserialize<List<TvShow>>(json);

                if (shows != null)
                {
                    AllShows.Clear();
                    foreach (var show in shows)
                        AllShows.Add(show);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex JSON: {ex.Message}");
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

            /*for (int i = 0; i < 24; i++)
                Times.Add(i.ToString("D2") + ":00");*/

            Times.Add("00:00 - 06:00");
            Times.Add("06:00 - 12:00");
            Times.Add("12:00 - 18:00");
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
                FavoritesShows.Remove(SelectedShow);
        }

        private void AddToPlan()
        {
            if (SelectedFavorite == null || string.IsNullOrWhiteSpace(PlannedTime))
            {
                MessageBox.Show("Будь ласка, оберіть передачу та введіть час.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TimeSpan.TryParse(PlannedTime, out var time))
            {
                MessageBox.Show("Час має бути у форматі HH:mm.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dateTime = PlannedDate.Date + time;

            // Перевірка на дублікати
            bool exists = PlannedShows.Any(ps => ps.Show == SelectedFavorite && ps.PlannedDateTime == dateTime);
            if (exists)
            {
                MessageBox.Show("Ця передача вже запланована на цей час.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            PlannedShows.Add(new PlannedShow
            {
                Show = SelectedFavorite,
                PlannedDateTime = dateTime,
                IsWatched = false
            });
        }
        private void RemovePlannedShow()
        {
            if (SelectedPlannedShow != null)
            {
                PlannedShows.Remove(SelectedPlannedShow);
                SelectedPlannedShow = null;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string? prop) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
