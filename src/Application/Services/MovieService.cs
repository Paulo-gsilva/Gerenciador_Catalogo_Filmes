using System;
using Flix.Application.DTOs;
using Flix.Domain;
using Flix.Domain.Repositories;

namespace Flix.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _repository;


    public MovieService(IMovieRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }


    public async Task<MovieDto> CreateAsync(CreateMovieRequest request)
    {
        ValidateCreate(request);


        var movie = new Movie
        {
            // Id será gerado no repositório se necessário
            Title = request.Title,
            Description = request.Description,
            ReleaseDate = request.ReleaseDate,
            Gender = request.Gender,
            Rating = request.Rating,
            DurationMinutes = request.DurationMinutes
        };


        var created = await _repository.CreateAsync(movie);
        return MapToDto(created);
    }


    public async Task<MovieDto?> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        var movie = await _repository.GetAsync(id);
        return movie == null ? null : MapToDto(movie);
    }


    public async Task<IEnumerable<MovieDto>> ListAsync(int page = 0, int pageSize = 50)
    {
        var movies = await _repository.ListAsync(page, pageSize);
        return movies.Select(MapToDto);
    }


    public async Task<MovieDto> UpdateAsync(string id, UpdateMovieRequest request)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id inválido", nameof(id));
        ValidateUpdate(request);


        var existing = await _repository.GetAsync(id);
        if (existing == null) throw new Exception($"Filme com id '{id}' não encontrado.");


        // aplicar mudanças
        existing.Title = request.Title;
        existing.Description = request.Description;
        existing.ReleaseDate = request.ReleaseDate;
        existing.Gender = request.Gender;
        existing.Rating = request.Rating;
        existing.DurationMinutes = request.DurationMinutes;


        var updated = await _repository.UpdateAsync(id, existing);
        return MapToDto(updated);
    }


    public async Task DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id inválido", nameof(id));
    }

    #region Helpers
    private static MovieDto MapToDto(Movie m) => new MovieDto
    {
        Id = m.Id,
        Title = m.Title,
        Description = m.Description,
        ReleaseDate = m.ReleaseDate,
        Gender = m.Gender,
        Rating = m.Rating,
        DurationMinutes = m.DurationMinutes
    };


    private static void ValidateCreate(CreateMovieRequest req)
    {
        if (req == null) throw new ArgumentNullException(nameof(req));
        if (string.IsNullOrWhiteSpace(req.Title)) throw new ArgumentException("Título é obrigatório.");
        if (req.Rating < 0 || req.Rating > 10) throw new ArgumentException("Rating deve estar entre 0 e 10.");
        if (req.DurationMinutes < 0) throw new ArgumentException("DurationMinutes não pode ser negativo.");
    }


    private static void ValidateUpdate(UpdateMovieRequest req)
    {
        if (req == null) throw new ArgumentNullException(nameof(req));
        if (string.IsNullOrWhiteSpace(req.Title)) throw new ArgumentException("Título é obrigatório.");
        if (req.Rating < 0 || req.Rating > 10) throw new ArgumentException("Rating deve estar entre 0 e 10.");
        if (req.DurationMinutes < 0) throw new ArgumentException("DurationMinutes não pode ser negativo.");
    }
    #endregion
}