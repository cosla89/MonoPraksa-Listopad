using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using Zoo.model;
using Zoo.repository;
using Zoo.common;
using Microsoft.Extensions.Logging;

namespace Zoo.service
{
    public class AnimalService : IAnimalService
    {
        private readonly IAnimalRepository _repository;
        private readonly IAnimalValidator _validator;
        private readonly ILogger<AnimalService> _logger;

        public AnimalService(IAnimalRepository repository, IAnimalValidator validator, ILogger<AnimalService> logger)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
        }

        public  Task<List<Animal>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }
        public Task<Animal?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }
        public Task<List<Animal>> GetFilteredAsync(AnimalFilter filter)
        {
            return _repository.GetFilteredAsync(filter);
        }
        public async Task<string?> AddAsync(Animal animal)
        {
            var error = _validator.Validate(animal);

            if (error != null)
            {
                _logger.LogWarning(
                    "Error wrong ifnormation");

                return error;
            }

            await _repository.AddAsync(animal);

            _logger.LogInformation(
                "Animal with ID {AnimalId} added",
                animal.Id);

            return null;
        }
        public async Task<AnimalUpdateResult> UpdateAsync(
            int id,
            Animal updatedAnimal)
        {
            var error = _validator.Validate(updatedAnimal);

            if (error != null)
            {
                return new AnimalUpdateResult
                {
                    Error = error
                };
            }

            var animal = await _repository.UpdateAsync(id, updatedAnimal);

            return new AnimalUpdateResult
            {
                Animal = animal
            };
        }
        public Task<bool> DeleteAsync(int id)
        {
            return _repository.DeleteAsync(id);
        }
    }
}

