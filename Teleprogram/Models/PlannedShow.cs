using Teleprogram.Models;

public class PlannedShow
{
    public TvShow Show { get; set; }    
    public DateTime PlannedDateTime { get; set; } 
    public bool IsWatched { get; set; } = false;
}