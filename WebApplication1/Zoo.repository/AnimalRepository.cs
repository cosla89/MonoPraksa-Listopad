using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.model;
using Npgsql;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Zoo.repository.Data;

namespace Zoo.repository
{
    public class AnimalRepository : IAnimalRepository
    {

        private readonly NpgsqlDataSource _dataSource;
        private readonly ZooDbContext _context;

        public AnimalRepository(NpgsqlDataSource dataSource, ZooDbContext context)
        {
            _dataSource = dataSource;
            _context = context;
        }


        public Task<List<Animal>> GetAllAsync()
        {
            return _context.Animals
                .AsNoTracking()
                .OrderBy(a => a.Id)
                .ToListAsync();
        }
        public async Task<Animal?> GetByIdAsync(int id)
        {
            await using var command = _dataSource.CreateCommand(
                @"SELECT id, name, species, age
          FROM animals
          WHERE id = @id");

            command.Parameters.AddWithValue("id", id);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Animal
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Species = reader.GetString(2),
                Age = reader.GetInt32(3)
            };
        }
        public async Task<List<Animal>> GetFilteredAsync(AnimalFilter filter)
        {
            var result = new List<Animal>();

            await using var command = _dataSource.CreateCommand(
                "SELECT id, name, species, age FROM animals WHERE TRUE");

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                command.CommandText +=
                    " AND strpos(lower(name), lower(@name)) > 0";

                command.Parameters.AddWithValue("name", filter.Name);
            }

            if (!string.IsNullOrWhiteSpace(filter.Species))
            {
                command.CommandText +=
                    " AND lower(species) = lower(@species)";

                command.Parameters.AddWithValue("species", filter.Species);
            }

            if (filter.MinAge.HasValue)
            {
                command.CommandText += " AND age >= @minAge";

                command.Parameters.AddWithValue(
                    "minAge", filter.MinAge.Value);
            }

            command.CommandText += " ORDER BY id";

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var animal = new Animal
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Species = reader.GetString(2),
                    Age = reader.GetInt32(3)
                };

                result.Add(animal);
            }

            return result;
        }

        public async Task AddAsync(Animal animal)
        {
            await using var command = _dataSource.CreateCommand(
                @"INSERT INTO animals (name, species, age)
          VALUES (@name, @species, @age)
          RETURNING id");

            command.Parameters.AddWithValue("name", animal.Name);
            command.Parameters.AddWithValue("species", animal.Species);
            command.Parameters.AddWithValue("age", animal.Age);

            var newId = await command.ExecuteScalarAsync();

            animal.Id = (int)newId!;
        }

        public async Task<Animal?> UpdateAsync(int id, Animal updatedAnimal)
        {
            await using var command = _dataSource.CreateCommand(
                @"UPDATE animals
          SET name = @name,
              species = @species,
              age = @age
          WHERE id = @id
          RETURNING id, name, species, age");

            command.Parameters.AddWithValue("id", id);
            command.Parameters.AddWithValue("name", updatedAnimal.Name);
            command.Parameters.AddWithValue("species", updatedAnimal.Species);
            command.Parameters.AddWithValue("age", updatedAnimal.Age);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Animal
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Species = reader.GetString(2),
                Age = reader.GetInt32(3)
            };
        }
        public async Task<bool> DeleteAsync(int id)
        {
            await using var command = _dataSource.CreateCommand(
                "DELETE FROM animals WHERE id = @id");

            command.Parameters.AddWithValue("id", id);

            var affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }
    }
}
