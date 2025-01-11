-This assignment is a Pokédex REST API that integrates with MongoDB to manage Pokémon data. The API provides CRUD operations for managing Pokémon and uses asynchronous methods for efficient database interactions.

###Here is some Explanation

#Install MongoDB locally
#verify installation: mongo --version

-Connecting to MongoDB:
#mongodb://localhost:27017

-Installing MongoDB Driver for .NET

Configuring MongoDB as follows
#add MongoDB Settings to appsettings.json:

"MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "PokemonDb",
    "CollectionName": "Pokemons"
  }

  -Configuring MongoDB in .NET
  #Update Pokemon.cs file in Model folder using the following code

  public class Pokemon
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Ability { get; set; }
        public int? Level { get; set; }
    }

    -Configuring MongoDB in .NET
    #Setup a connection to MongoDB with the following code in the PokemonService.cs folder

    public class PokemonService : IPokemonService
    {
        private readonly IMongoCollection<Pokemon> _pokemonCollection;

        public PokemonService(IOptions<MongoDbSettings> mongoDbSettings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _pokemonCollection = database.GetCollection<Pokemon>(mongoDbSettings.Value.CollectionName);
        }
    }

-Update Sercive Methods
#Get All
 public async Task<List<Pokemon>> GetAllAsync()
        {
            return await _pokemonCollection.Find(p => true).ToListAsync();
        }

#GetById
 public async Task<Pokemon?> GetByIdAsync(int id)
        {
            return await _pokemonCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

#Add
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

#Update
 public async Task<Pokemon?> UpdateAsync(int id, Pokemon updatedPokemon)
        {
            var result = await _pokemonCollection.ReplaceOneAsync(p => p.Id == id, updatedPokemon);
            return result.IsAcknowledged ? updatedPokemon : null;
        }
#Delete
 public async Task<bool> DeleteAsync(int id)
        {
            var result = await _pokemonCollection.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }


        -If you have a question with this work,please feel free to ask me!
