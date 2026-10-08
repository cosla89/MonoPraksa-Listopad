using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zoo.model
{
    public class AnimalUpdateResult
    {
        public Animal? Animal { get; set; }

        public string? Error { get; set; }
    }
}
