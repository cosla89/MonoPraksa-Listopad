using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.model;
using Npgsql;
using System.Xml.Linq;

namespace Zoo.repository
{
    public class AnimalRepository : IAnimalRepository
    {

        private readonly NpgsqlDataSource _dataSource;

        public AnimalRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        
        public List<Animal> GetAll()      
        {
            var result = new List<Animal>();
            using var command = _dataSource.CreateCommand(
        "SELECT id, name, species, age FROM animals ORDER BY id");

            using var reader = command.ExecuteReader();
            while (reader.Read())
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
        public Animal? GetById(int id)
        {
            using var command = _dataSource.CreateCommand(
                "SELECT id, name, species, age FROM animals WHERE id = @id");

            command.Parameters.AddWithValue("id", id);

            using var reader = command.ExecuteReader();

            if (!reader.Read())
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

        public List<Animal> GetFiltered(AnimalFilter filter)
        {
            var result = new List<Animal>();

            using var command = _dataSource.CreateCommand(
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

            using var reader = command.ExecuteReader();

            while (reader.Read())
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
      
        public void Add(Animal animal)
        {
            using var command = _dataSource.CreateCommand(
                @"INSERT INTO animals (name, species, age)
          VALUES (@name, @species, @age)
          RETURNING id");

            command.Parameters.AddWithValue("name", animal.Name);
            command.Parameters.AddWithValue("species", animal.Species);
            command.Parameters.AddWithValue("age", animal.Age);

            var newId = command.ExecuteScalar();

            animal.Id = (int)newId!;
        }

        public Animal? Update(int id, Animal updatedAnimal)
        {
            using var command = _dataSource.CreateCommand(
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

            using var reader = command.ExecuteReader();

            if (!reader.Read())
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
        public bool Delete(int id)
        {
            using var command = _dataSource.CreateCommand(
                "DELETE FROM animals WHERE id = @id");

            command.Parameters.AddWithValue("id", id);

            var affectedRows = command.ExecuteNonQuery();

            return affectedRows > 0;
        }
    }
}
