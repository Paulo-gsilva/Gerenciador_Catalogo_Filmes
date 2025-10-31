using System.Net;
using System.Text.Json;
using Flix.Application.DTOs;
using Flix.Application;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Flix.Functions
{
    public class MoviesFunction
    {
        private readonly ILogger<MoviesFunction> _logger;
        private readonly IMovieService _movieService;

        public MoviesFunction(ILogger<MoviesFunction> logger, IMovieService movieService)
        {
            _logger = logger;
            _movieService = movieService;
        }

        [Function("CreateMovie")]
        public async Task<HttpResponseData> CreateMovieAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "movies")] HttpRequestData req)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                var movie = JsonSerializer.Deserialize<CreateMovieRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (movie == null)
                    return await CreateResponse(req, HttpStatusCode.BadRequest, "Corpo da requisição inválido.");

                var id = await _movieService.CreateAsync(movie);

                var response = req.CreateResponse(HttpStatusCode.Created);
                response.Headers.Add("Location", $"/movies/{id}");
                await response.WriteStringAsync($"Filme criado com ID: {id}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar filme.");
                return await CreateResponse(req, HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [Function("GetMovies")]
        public async Task<HttpResponseData> GetMoviesAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "movies")] HttpRequestData req)
        {
            try
            {
                var movies = await _movieService.ListAsync();
                return await CreateResponse(req, HttpStatusCode.OK, movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar filmes.");
                return await CreateResponse(req, HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Function("GetMovieById")]
        public async Task<HttpResponseData> GetMovieByIdAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "movies/{id}")] HttpRequestData req,
            string id)
        {
            try
            {
                var movie = await _movieService.GetByIdAsync(id);
                return await CreateResponse(req, HttpStatusCode.OK, movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar filme por ID.");
                return await CreateResponse(req, HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Function("UpdateMovie")]
        public async Task<HttpResponseData> UpdateMovieAsync(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "movies/{id}")] HttpRequestData req,
            string id)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                var movie = JsonSerializer.Deserialize<UpdateMovieRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (movie == null)
                    return await CreateResponse(req, HttpStatusCode.BadRequest, "Corpo da requisição inválido.");

                await _movieService.UpdateAsync(id, movie);
                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar filme.");
                return await CreateResponse(req, HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Function("DeleteMovie")]
        public async Task<HttpResponseData> DeleteMovieAsync(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "movies/{id}")] HttpRequestData req,
            string id)
        {
            try
            {
                await _movieService.DeleteAsync(id);
                return req.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar filme.");
                return await CreateResponse(req, HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        
        private static async Task<HttpResponseData> CreateResponse<T>(
            HttpRequestData req, HttpStatusCode status, T body)
        {
            var response = req.CreateResponse(status);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteStringAsync(JsonSerializer.Serialize(body));
            return response;
        }
    }
}