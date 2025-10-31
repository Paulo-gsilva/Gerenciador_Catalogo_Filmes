using System;

namespace Flix.Application.DTOs;

public class CreateMovieRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public string Gender { get; set; }

    public double Rating { get; set; }

    public int DurationMinutes { get; set; }
}