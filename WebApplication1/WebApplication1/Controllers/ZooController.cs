using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/zoo")]
    public class ZooController : ControllerBase
    {
        private static readonly List<Animal> animals = new List<Animal>
        {
            new Animal{Id = 1, Name = "Mihael", Species = "Lion", Age = 1},
            new Animal{Id = 2, Name = "Crni", Species = "Lion", Age = 5},
            new Animal{Id = 3, Name = "Beli", Species = "Tiger", Age = 2},
            new Animal{Id = 4, Name = "Mićo", Species = "Lion", Age = 8}
        };

        private static int nextId = 5;

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(animals);
        }
        [HttpGet("{id:int}")]
            public IActionResult GetId(int id)
        {
            var animal = animals.FirstOrDefault(a => a.Id == id);
            if (animal == null)
            {
                return NotFound("error animal not in system");
            }
            return Ok(animal);

        }
        [HttpGet ("filter")]
            public IActionResult GetFiltered([FromQuery] AnimalFilter filter)
        {
            var result = animals.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                result = result.Where(a  => a.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(filter.Species))
            {
                result = result.Where(a => a.Species.Equals(filter.Species, StringComparison.OrdinalIgnoreCase));

            }
            if (filter.MinAge.HasValue)
            {
                result = result.Where(a => a.Age >= filter.MinAge.Value);

            }
            return Ok(result.ToList());
        }
        [HttpPost]
        public IActionResult Add([FromBody] Animal animal)
        {
            animal.Id = nextId++;
            animals.Add(animal);

            return StatusCode(201, animal);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Animal updatedAnimal)
        {
            var animal = animals.FirstOrDefault(a => a.Id == id);

            if (animal == null)
            {
                return NotFound("Error animal not found");
            }

            animal.Name = updatedAnimal.Name;

            return Ok(animal);
        }
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var animal = animals.FirstOrDefault(a => a.Id == id);

            if (animal == null)
            {
                return NotFound("Životinja nije pronađena.");
            }

            animals.Remove(animal);

            return NoContent();
        }


    }
}
