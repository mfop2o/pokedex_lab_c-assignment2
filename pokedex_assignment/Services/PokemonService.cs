using MongoDB.Driver;
using MongoDB.Driver.Linq;
using pokedex.Models;
using pokedex.Settings;
using Microsoft.Extensions.Options;

namespace pokedex.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly IMongoCollection<Pokemon> _pokemonCollection;

        public PokemonService(IOptions<MongoDbSettings> mongoDbSettings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _pokemonCollection = database.GetCollection<Pokemon>(mongoDbSettings.Value.CollectionName);
        }

        public async Task<List<Pokemon>> GetAllAsync()
        {
            return await _pokemonCollection.Find(p => true).ToListAsync();
        }

        public async Task<Pokemon?> GetByIdAsync(int id)
        {
            return await _pokemonCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Pokemon?> AddAsync(Pokemon newPokemon)
        {
            // Ensure Id is unique by finding the max existing Id
            var maxId = await _pokemonCollection.AsQueryable().MaxAsync(p => (int?)p.Id) ?? 0;
            newPokemon.Id = maxId + 1;

            try
            {
                await _pokemonCollection.InsertOneAsync(newPokemon);
                return newPokemon;
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new InvalidOperationException($"A Pokémon with the Id {newPokemon.Id} already exists.", ex);
            }
        }

        public async Task<Pokemon?> UpdateAsync(int id, Pokemon updatedPokemon)
        {
            var result = await _pokemonCollection.ReplaceOneAsync(p => p.Id == id, updatedPokemon);
            return result.IsAcknowledged ? updatedPokemon : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _pokemonCollection.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
