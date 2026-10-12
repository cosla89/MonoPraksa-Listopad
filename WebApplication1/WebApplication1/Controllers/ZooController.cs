using Microsoft.AspNetCore.Mvc;
using Zoo.model;
using System.Collections.Generic;
using System.Linq;
using Zoo.service;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/zoo")]
    [Authorize]
    public class ZooController : ControllerBase
    {
        private readonly IAnimalService _service;

        public ZooController(IAnimalService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var animals = await _service.GetAllAsync();
            return Ok(animals);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetId(int id)
        {
            var animal = await _service.GetByIdAsync(id);

            if (animal == null)
            {
                return NotFound("Animal not found");
            }

            return Ok(animal);
        }
        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered(
            [FromQuery] AnimalFilter filter)
        {
            var animals = await _service.GetFilteredAsync(filter);

            return Ok(animals);
        }
        [Authorize(Roles = RoleNames.Admin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Animal animal)
        {
            var error = await _service.AddAsync(animal);

            if (error != null)
            {
                return BadRequest(error);
            }

            return StatusCode(201, animal);
        }

        [Authorize(Roles = RoleNames.Admin)]
        [HttpPut("{id:int}")]
            public async Task<IActionResult> Update(
            int id,
            [FromBody] Animal updatedAnimal)
        {
            var result = await _service.UpdateAsync(id, updatedAnimal);

            if (result.Error != null)
            {
                return BadRequest(result.Error);
            }

            if (result.Animal == null)
            {
                return NotFound("Animal not found");
            }

            return Ok(result.Animal);
        }

        [Authorize(Roles = RoleNames.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound("Animal not found");
            }

            return NoContent();
        }


    }
}
