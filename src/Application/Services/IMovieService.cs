using System;
using Flix.Application.DTOs;

namespace Flix.Application;

public interface IMovieService
{
    Task<MovieDto> CreateAsync(CreateMovieRequest request);

    Task<MovieDto?> GetByIdAsync(string id);

    Task<IEnumerable<MovieDto>> ListAsync(int page = 0, int pageSize = 50);

    Task<MovieDto> UpdateAsync(string id, UpdateMovieRequest request);

    Task DeleteAsync(string id);
}
