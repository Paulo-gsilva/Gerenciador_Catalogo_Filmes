using System;
using Flix.Domain;
using Flix.Domain.Repositories;
using Microsoft.Azure.Cosmos;

namespace Flix.Infrastructure.Repositories;

public class CosmosMovieRepository : IMovieRepository
{
    private readonly CosmosClient _client;
    private readonly Container _container;

    public CosmosMovieRepository(CosmosClient client, string databaseId, string containerId)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));

        var dbResponse = _client.CreateDatabaseIfNotExistsAsync(databaseId).GetAwaiter().GetResult();
        var containerResponse = dbResponse.Database.CreateContainerIfNotExistsAsync(
            new ContainerProperties(containerId, "/id")).GetAwaiter().GetResult();

        _container = containerResponse.Container;
    }


    public async Task<Movie> CreateAsync(Movie movie)
    {
        if (string.IsNullOrEmpty(movie.Id)) movie.Id = Guid.NewGuid().ToString();
        var response = await _container.CreateItemAsync(movie, new PartitionKey(movie.Id));
        return response.Resource;
    }


    public async Task<Movie?> GetAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Movie>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex)
        {
            return null;
        }
    }


    public async Task<IEnumerable<Movie>> ListAsync(int page = 0, int pageSize = 50)
    {
        var query = new QueryDefinition("SELECT * FROM c ORDER BY c._ts DESC");
        var iterator = _container.GetItemQueryIterator<Movie>(query, requestOptions: new QueryRequestOptions { MaxItemCount = pageSize });
        var results = new List<Movie>();

        while (iterator.HasMoreResults && results.Count < pageSize)
        {
            var pageResponse = await iterator.ReadNextAsync();
            results.AddRange(pageResponse.Resource);
        }
        
        return results.Take(pageSize);
    }


    public async Task<Movie> UpdateAsync(string id, Movie movie)
    {
        movie.Id = id; // ensure id matches
        var response = await _container.UpsertItemAsync(movie, new PartitionKey(id));
        return response.Resource;
    }


    public async Task DeleteAsync(string id)
    {
        await _container.DeleteItemAsync<Movie>(id, new PartitionKey(id));
    }
}
