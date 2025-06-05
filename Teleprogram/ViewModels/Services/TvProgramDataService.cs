using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Teleprogram.Models; 

namespace Teleprogram.Services
{
    public class TvProgramDataService
    {
        private const string FAVORITES_FILE = "favorites.json";
        private const string PLANNED_FILE = "plannedshows.json";
        private const string TVSHOWS_FILE = "tvshows.json"; 

        public ObservableCollection<TvShow> LoadAllShows()
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TVSHOWS_FILE);
                if (File.Exists(filePath))
                {
                    string jsonString = File.ReadAllText(filePath);
                    var shows = JsonSerializer.Deserialize<ObservableCollection<TvShow>>(jsonString);
                    return shows ?? new ObservableCollection<TvShow>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при завантаженні всіх передач: {ex.Message}");
            }
            return new ObservableCollection<TvShow>(); 
        }

        public ObservableCollection<TvShow> LoadFavorites()
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FAVORITES_FILE);
                if (File.Exists(filePath))
                {
                    string jsonString = File.ReadAllText(filePath);
                    var favorites = JsonSerializer.Deserialize<ObservableCollection<TvShow>>(jsonString);
                    return favorites ?? new ObservableCollection<TvShow>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при завантаженні обраних передач: {ex.Message}");
            }
            return new ObservableCollection<TvShow>();
        }

        public void SaveFavorites(ObservableCollection<TvShow> favorites)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FAVORITES_FILE);
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(favorites, options);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при збереженні обраних передач: {ex.Message}");
            }
        }

        public ObservableCollection<PlannedShow> LoadPlannedShows()
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PLANNED_FILE);
                if (File.Exists(filePath))
                {
                    string jsonString = File.ReadAllText(filePath);
                    var planned = JsonSerializer.Deserialize<ObservableCollection<PlannedShow>>(jsonString);
                    return planned ?? new ObservableCollection<PlannedShow>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при завантаженні запланованих передач: {ex.Message}");
            }
            return new ObservableCollection<PlannedShow>();
        }
        public void SavePlannedShows(ObservableCollection<PlannedShow> plannedShows)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PLANNED_FILE);
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(plannedShows, options);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при збереженні запланованих передач: {ex.Message}");
            }
        }
    }
}