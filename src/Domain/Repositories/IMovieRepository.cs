using System;

namespace Flix.Domain.Repositories;

public interface IMovieRepository
{
    Task<Movie> CreateAsync(Movie movie);

    Task<Movie?> GetAsync(string id);
    
    Task<IEnumerable<Movie>> ListAsync(int page = 0, int pageSize = 50);
    
    Task<Movie> UpdateAsync(string id, Movie movie);
    
    Task DeleteAsync(string id);
}