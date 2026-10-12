using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.model;

namespace Zoo.common
{
    public class AnimalValidator : IAnimalValidator
    {
        public string? Validate(Animal animal)
        {
            if (string.IsNullOrWhiteSpace(animal.Name))
            {
                return "name canot be empty";
            }

            if (animal.Age < 0)
            {
                return "Age needs to be positive";
            }

            return null;
        }
    }
}
