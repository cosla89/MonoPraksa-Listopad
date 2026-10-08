using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using Zoo.model;
using Zoo.repository;
using Zoo.common;

namespace Zoo.service
{
    public class AnimalService : IAnimalService
    {
        private readonly IAnimalRepository _repository;
        private readonly IAnimalValidator _validator;

        public AnimalService(IAnimalRepository repository, IAnimalValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public List<Animal> GetAll()
        {
            return _repository.GetAll();
        }
        public Animal? GetById(int id)
        {
            return _repository.GetById(id);
        }
        public List<Animal> GetFiltered(AnimalFilter filter)
        {
            return _repository.GetFiltered(filter);
        }
        public string? Add(Animal animal)
        {
            var error = _validator.Validate(animal);

            if (error != null)
            {
                return error;
            }

            _repository.Add(animal);

            return null;
        }
        public AnimalUpdateResult Update(int id, Animal updatedAnimal)
        {
            var error = _validator.Validate(updatedAnimal);

            if (error != null)
            {
                return new AnimalUpdateResult
                {
                    Error = error
                };
            }

            var animal = _repository.Update(id, updatedAnimal);

            return new AnimalUpdateResult
            {
                Animal = animal
            };
        }
        public bool Delete(int id)
        {
            return _repository.Delete(id);
        }
    }
}

