using Microsoft.AspNetCore.Mvc;
using Zoo.model;
using System.Collections.Generic;
using System.Linq;
using Zoo.service;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/zoo")]
    public class ZooController : ControllerBase
    {
        private readonly IAnimalService _service;

        public ZooController(IAnimalService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var animals = _service.GetAll();
            return Ok(animals);
        }

        [HttpGet("{id:int}")]
            public IActionResult GetId(int id)
        {
            var animal = _service.GetById(id);
            if (animal == null)
            {
                return NotFound("error animal not in system");
            }
            return Ok(animal);

        }
        [HttpGet ("filter")]
            public IActionResult GetFiltered([FromQuery] AnimalFilter filter)
        { 
            var animals = _service.GetFiltered(filter);
            
            return Ok(animals);
        }
        [HttpPost]
        public IActionResult Add([FromBody] Animal animal)
        {
            var error = _service.Add(animal);

            if (error != null) { 
                return BadRequest(error);
            }

            return StatusCode(201, animal);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Animal updatedAnimal)
        {
            var result = _service.Update(id, updatedAnimal);

            if (result.Error != null)
            {
                return BadRequest(result.Error);
            }

            if (result.Animal == null)
            {
                return NotFound("error animal not found");
            }

            return Ok(result.Animal);
        }
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var deleted = _service.Delete(id);

            if (!deleted)
            {
                return NotFound("error animal not found");
            }

            return NoContent();
        }


    }
}
