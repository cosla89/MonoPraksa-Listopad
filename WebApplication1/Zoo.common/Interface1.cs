using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.model;

namespace Zoo.common
{
    public interface IAnimalValidator
    {
        string? Validate(Animal animal);
    }
}
