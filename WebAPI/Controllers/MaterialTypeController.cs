using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Services.MaterialsType;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialTypeController : ControllerBase
    {
        private readonly IMaterialsTypeService _materialTypeService;

        public MaterialTypeController(IMaterialsTypeService materialTypeService)
        {
            _materialTypeService = materialTypeService;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CreateMaterialType([FromBody] MaterialType materialType)
        {
            var createdMaterial = await _materialTypeService.CreateMaterialType(materialType);
            return createdMaterial is not null
                ? CreatedAtAction(nameof(GetMaterialTypeById), new { id = createdMaterial.MaterialTypeId }, createdMaterial)
                : BadRequest("Error al crear el tipo de material");
        }

        // READ
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterialTypeById(int id) //Obtener un tipo de material por su id
        {
            var materialType = await _materialTypeService.GetMaterialTypeById(id);
            return materialType is not null
                ? Ok(materialType)
                : NotFound("Tipo de material no encontrado");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMaterialTypes() //Obtener todos los tipos de material
        {
            var materialTypes = await _materialTypeService.GetAllMaterialTypes();
            return materialTypes is not null && materialTypes.Any()
                ? Ok(materialTypes)
                : NoContent();
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterialType(int id, [FromBody] MaterialType materialType)
        {
            if (id != materialType.MaterialTypeId)
                return BadRequest("ID no coincide");

            var updatedMaterial = await _materialTypeService.UpdateMaterialType(materialType);
            return Ok(updatedMaterial);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialType(int id)
        {
             var getUserType = await _materialTypeService.GetMaterialTypeById(id);

            if (getUserType is null)
            {
                return NotFound("Tipo de material no encontrado");
            }

            var deleted = await _materialTypeService.DeleteMaterialType(id);
            return deleted
                ? NoContent()
                : BadRequest("Error al eliminar el tipo de material");
        }
    }
}
