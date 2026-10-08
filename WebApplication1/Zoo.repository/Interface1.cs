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
        void Add(Animal animal);
        List<Animal> GetAll();
        Animal? GetById(int id);
        List<Animal> GetFiltered(AnimalFilter filter);
        Animal? Update(int id, Animal updatedAnimal);
        bool Delete(int id);
    }
    
}
