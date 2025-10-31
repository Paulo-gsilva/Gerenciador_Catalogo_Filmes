using System;

namespace Flix.Domain;

public class Movie
{
    public string Id { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public DateTime? ReleaseDate { get; set; }
    
    public string Gender { get; set; }

    public double Rating { get; set; }
    
    public int DurationMinutes { get; set; }
}
