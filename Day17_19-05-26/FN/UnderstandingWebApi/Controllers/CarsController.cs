using Microsoft.AspNetCore.Mvc;
using UnderstandingWebApi.Models;

namespace UnderstandingWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private static int _nextId = 3;

        private static readonly List<Car> _cars = new()
        {
            new Car(1, "BMW", "M3 Sport", 2023),
            new Car(2, "Volkswagen", "Tiguan", 2024)
        };

        // GET: api/cars
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Car>> GetCars()
        {   
            return Ok(_cars);
        }

        [HttpGet("Throw")]
        public IActionResult Throw() =>
            throw new Exception("Sample exception.");

        // GET: api/cars/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Car> GetCarById(int id)
        {
            var car = _cars.FirstOrDefault(c => c.Id == id);

            if (car is null)
            {
                return NotFound(new
                {
                    message = $"Car with ID {id} not found"
                });
            }

            return Ok(car);
        }

        // POST: api/cars
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Car> AddCar(Car car)
        {
            if (string.IsNullOrWhiteSpace(car.Brand))
            {
                return BadRequest(new
                {
                    message = "Brand is required"
                });
            }

            if (string.IsNullOrWhiteSpace(car.Model))
            {
                return BadRequest(new
                {
                    message = "Model is required"
                });
            }

            var newCar = new Car(
                _nextId++,
                car.Brand,
                car.Model,
                car.Year
            );

            _cars.Add(newCar);

            return CreatedAtAction(
                nameof(GetCarById),
                new { id = newCar.Id },
                newCar
            );
        }

        // PUT: api/cars/1
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateCar(int id, Car updatedCar)
        {
            var existingCar = _cars.FirstOrDefault(c => c.Id == id);

            if (existingCar is null)
            {
                return NotFound(new
                {
                    message = $"Car with ID {id} not found"
                });
            }

            existingCar.Brand = updatedCar.Brand;
            existingCar.Model = updatedCar.Model;
            existingCar.Year = updatedCar.Year;

            return NoContent();
        }

        // DELETE: api/cars/1
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteCar(int id)
        {
            var car = _cars.FirstOrDefault(c => c.Id == id);

            if (car is null)
            {
                return NotFound(new
                {
                    message = $"Car with ID {id} not found"
                });
            }

            _cars.Remove(car);

            return NoContent();
        }
    }
}