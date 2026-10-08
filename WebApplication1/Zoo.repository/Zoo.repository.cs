using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.model;

namespace Zoo.repository
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly List<Animal> animals = new List<Animal>
        {
            new Animal{Id = 1, Name = "Mihael", Species = "Lion", Age = 1},
            new Animal{Id = 2, Name = "Crni", Species = "Lion", Age = 5},
            new Animal{Id = 3, Name = "Beli", Species = "Tiger", Age = 2},
            new Animal{Id = 4, Name = "Mićo", Species = "Lion", Age = 8}
        };
        public List<Animal> GetAll()      
        {
            return new List<Animal>(animals); 
        }
        public Animal? GetById(int id)
        {
            return animals.FirstOrDefault(a => a.Id == id);

        }

        public List<Animal> GetFiltered(AnimalFilter filter)
        {
            var result = animals.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                result = result.Where(a =>
                    a.Name.Contains(
                        filter.Name,
                        StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.Species))
            {
                result = result.Where(a =>
                    a.Species.Equals(
                        filter.Species,
                        StringComparison.OrdinalIgnoreCase));
            }

            if (filter.MinAge.HasValue)
            {
                result = result.Where(a => a.Age >= filter.MinAge.Value);
            }          
            return result.ToList();
        }
        private int nextId = 5;
        public void Add(Animal animal)
        {
            animal.Id = nextId++;
            animals.Add(animal);
        } 

        public Animal? Update(int id, Animal updatedAnimal)
        {
            var animal = animals.FirstOrDefault(a => a.Id == id);

            if (animal == null)
            {
                return null;
            }

            animal.Name = updatedAnimal.Name;
            animal.Species = updatedAnimal.Species;
            animal.Age = updatedAnimal.Age;

            return animal;
        }
        public bool Delete(int id)
        {
            var animal = animals.FirstOrDefault(a => a.Id == id);

            if (animal == null)
            {
                return false;
            }

            animals.Remove(animal);

            return true;
        }
    }
}
