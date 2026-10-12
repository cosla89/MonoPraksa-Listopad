using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.model;

namespace Zoo.service
{
    public interface IAnimalService
    {
        Task<List<Animal>> GetAllAsync();
        Task<Animal?> GetByIdAsync(int id);
        Task<List<Animal>> GetFilteredAsync(AnimalFilter filter);
        Task<string?> AddAsync(Animal animal);
        Task<AnimalUpdateResult> UpdateAsync(int id, Animal updatedAnimal);
        Task<bool> DeleteAsync(int id);
    }
}

