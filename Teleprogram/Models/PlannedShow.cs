using Teleprogram.Models;

public class PlannedShow
{
    public TvShow Show { get; set; } = null!;
    public DateTime PlannedDateTime { get; set; }
    public override bool Equals(object? obj)
    {
        return Equals(obj as PlannedShow);
    }

    public bool Equals(PlannedShow? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Show != null && other.Show != null && Show.Equals(other.Show) &&
               PlannedDateTime.Equals(other.PlannedDateTime);
    }

}