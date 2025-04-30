using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Recolections.cs.DTOs;
using Services.Recolections.cs.Enumerables;
using Services.Recolections.cs.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecolectionController : ControllerBase
    {
        //Inyección de dependencias
        private readonly IRecolectionService _recolectionService;

        //Constructor
        public RecolectionController(IRecolectionService recolectionService)
        {
            _recolectionService = recolectionService;
        }

        //CREATE
        [HttpPost(Name = "CreateRecolection")]
        public async Task<IActionResult> CreateRecolection([FromBody] RecolectionForAdditionDTO recolectionForAddition)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newRecolection = await _recolectionService.CreateAsync(recolectionForAddition);

            if (newRecolection is null)
            {
                return BadRequest("Error al crear la recolección");
            }

            var output = new
            {
                newRecolection.RecolectionId,
                newRecolection.DateRecolection,
                newRecolection.MaterialTypeId,
                newRecolection.Weight,
                newRecolection.UserId
            };

            return CreatedAtAction(nameof(GetRecolectionById), new { id = newRecolection.RecolectionId }, output);

        }

        //READ
        [HttpGet("{id}", Name = "GetRecolectionById")] //Obtener una recolección por su id
        public async Task<IActionResult> GetRecolectionById(int id)
        {
            var recolection = await _recolectionService.GetRecolectionById(id);
            if (recolection is null)
            {
                return NotFound();
            }

            var output = new
            {
                recolection.RecolectionId,
                recolection.DateRecolection,
                recolection.MaterialTypeId,
                recolection.Weight,
                recolection.UserId
            };

            return Ok(output);
        }

        [HttpGet(Name = "GetAllRecolections")] //Obtener todas las recolecciones
        public async Task<IActionResult> GetAllRecolections(
            [FromQuery] int? userId = null,
            [FromQuery] RecollectionTypeEnum? type = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var recolections = await _recolectionService.GetAllRecolections(userId, type, fromDate, toDate);

            if (recolections is null || !recolections.Any())
            {
                return NoContent();
            }

            var output = recolections.Select(r => new
            {
                r.RecolectionId,
                r.DateRecolection,
                r.MaterialTypeId,
                r.Weight,
                r.UserId
            });

            return Ok(output);
        }

        //UPDATE
        [HttpPut("{id}", Name = "UpdateRecolection")]
        public async Task<IActionResult> UpdateRecolection(int id, [FromBody] RecolectionUpdateDTO recolectionUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != recolectionUpdate.RecolectionId)
                return BadRequest("ID no coincide");

            //Validar que la recolección existe

            var result = await _recolectionService.UpdateRecolection(recolectionUpdate);
            if (result is null) return NotFound();

            return Ok(new
            {
                result.RecolectionId,
                result.DateRecolection,
                result.MaterialTypeId,
                result.Weight,
                result.UserId
            });
        }

        //DELETE
        [HttpDelete("{id}", Name = "DeleteRecolection")]//Eliminar una recolección por su id
        public async Task<IActionResult> DeleteRecolection(int id)
        {
                var succes = await _recolectionService.DeleteRecolection(id);
                return succes ? NoContent() : NotFound();
        }

        //Obtener las recolecciones próximas
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingRecolections(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var defaultStartDate = DateTime.Today;
            var defaultEndDate = DateTime.Today.AddDays(30);

            var recolections = await _recolectionService.GetUpComingRecollections(
                startDate ?? defaultStartDate,
                endDate ?? defaultEndDate);

            if (recolections is null || !recolections.Any())
            {
                return NoContent();
            }
            var output = recolections.Select(r => new
            {
                r.RecolectionId,
                r.DateRecolection,
                r.MaterialTypeId,
                r.Weight,
                r.UserId
            });

            return Ok(output);
        }

        //Registrar peso
        [HttpPut("{id}/weight", Name = "RegisterWeight")]
        public async Task<IActionResult> RegisterWeight(int id, [FromBody] RegisterWeightDTO registerWeight)
        {
            if (registerWeight.Weight <= 0) // Access the Weight property of RegisterWeightDTO
            {
                return BadRequest("El peso debe ser mayor a cero");
            }

            var result = await _recolectionService.RegisterWeight(id, registerWeight.Weight);

            // Validar que la recolección existe
            if (result == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                result.RecolectionId,
                result.DateRecolection,
                result.MaterialTypeId,
                result.Weight,
                result.UserId
            });
        }

        //REPORTES
        // Reporte específico de un usuario
        [HttpGet("reports/user/{userId}")]
        public async Task<IActionResult> GetUserReport(
            int userId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var report = await _recolectionService.GetUserRecolectionsReport(userId, startDate, endDate);

            if (report is null)
            {
                return NotFound();
            }
            var output = new
            {
                User = new
                {
                    report.User.UserId,
                    report.User.Email
                },
                Recolections = report.Recolections.Select(r => new
                {
                    Date = r.Date,
                    MaterialType = r.MaterialType,
                    Weight = r.Weight,
                }),
                report.TotalWeight,
                report.RecolectionSummary
            };

            return Ok(output);
        }

    }
}
