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
        List<Animal> GetAll();
        Animal? GetById(int id);
        List<Animal> GetFiltered(AnimalFilter filter);
        string? Add(Animal animal);
        AnimalUpdateResult Update(int id, Animal updatedAnimal);
        bool Delete(int id);
    }
}

