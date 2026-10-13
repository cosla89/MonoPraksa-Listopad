using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.model;

namespace Zoo.repository
{
    public interface IAnimalRepository
    {
        Task AddAsync(Animal animal);
        Task<List<Animal>> GetAllAsync();
        Task<Animal?> GetByIdAsync(int id);
        Task<List<Animal>> GetFilteredAsync(AnimalFilter filter);
        Task<Animal?> UpdateAsync(int id, Animal updatedAnimal);
        Task<bool> DeleteAsync(int id);
    }
    
}
