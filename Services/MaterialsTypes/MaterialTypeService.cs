using System.Reflection.Metadata.Ecma335;
using Data.DBConext;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Services.MaterialsType
{
    public class MaterialTypeService : IMaterialsTypeService
    {
        private readonly RecollectionProjectContext _context;

        public MaterialTypeService(RecollectionProjectContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // CREATE
        public async Task<MaterialType?> CreateMaterialType(MaterialType materialType)
        {
            if (!ValidateMaterialType(materialType))
            {
                return null;
            }

            if (!await ValidateMaterialTypeUniqueness(materialType))
            {
                return null;
            }

            await _context.MaterialTypes.AddAsync(materialType);
            await _context.SaveChangesAsync();
            return materialType;
        }

        //READ
        public async Task<MaterialType?> GetMaterialTypeById(int id) //Obetener por id
        {
            var materialType = await _context.MaterialTypes.FindAsync(id);
            return materialType;
        }

        public async Task<IEnumerable<MaterialType>> GetAllMaterialTypes() //Obtener todos
        {
            return await _context.MaterialTypes.ToListAsync();
        }


        //UPDATE
        public async Task<MaterialType?> UpdateMaterialType(MaterialType materialType)
        {
            if (!ValidateMaterialType(materialType))
            {
                return null;
            }

            var existingMaterial = await GetMaterialTypeById(materialType.MaterialTypeId);

            if (existingMaterial == null)
            {
                return null;
            }

            
            if (await _context.MaterialTypes.AnyAsync(m =>
                m.MaterialName == materialType.MaterialName &&
                m.MaterialTypeId != materialType.MaterialTypeId))
            {
                return null;
            }

            existingMaterial.MaterialName = materialType.MaterialName;
            existingMaterial.WithWeight = materialType.WithWeight;

            _context.MaterialTypes.Update(existingMaterial);
            await _context.SaveChangesAsync();
            return existingMaterial;
        }

        //DELETE
        public async Task<bool> DeleteMaterialType(int id)
        {
            var materialType = await GetMaterialTypeById(id);

            if (materialType == null)
            {
                return false;
            }

            if (await _context.Recolections.AnyAsync(r => r.MaterialTypeId == id))
            {
                return false;
            }

            _context.MaterialTypes.Remove(materialType);
            await _context.SaveChangesAsync();

            return true;
        }

        //Validaciones adicionales
        private bool ValidateMaterialType(MaterialType materialType)
        {
            if (materialType == null)
            {
                return false;
            }
                

            if (string.IsNullOrWhiteSpace(materialType.MaterialName))
            {
                return false;
            }

            if (materialType.MaterialName.Length > 255)
            {
                return false;
            }

            return true;
        }

        // Método para validar si el tipo de material existe
        public async Task<bool> MaterialTypeExists(int id)
            => await _context.MaterialTypes.AnyAsync(m => m.MaterialTypeId == id);

        // Método para validar si el nombre del material existe
        public async Task<bool> MaterialNameExists(string materialName)
            => await _context.MaterialTypes.AnyAsync(m => m.MaterialName == materialName);

        // Método para validar la unicidad del nombre del material
        private async Task<bool> ValidateMaterialTypeUniqueness(MaterialType materialType)
        {
            if (await _context.MaterialTypes.AnyAsync(m => m.MaterialName == materialType.MaterialName))
            {
                return false; // El nombre del material ya existe

            }
            return true;
        }
    }
}
