using Data.Models;

namespace Services.MaterialsType
{
    public interface IMaterialsTypeService
    {

        //CREATE
        Task<MaterialType?> CreateMaterialType(MaterialType materialType); //Crear un tipo de material

        //READ
        Task<MaterialType?> GetMaterialTypeById(int id); //Obtener un tipo de material por su id
        Task<IEnumerable<MaterialType>> GetAllMaterialTypes(); //Obtener todos los tipos de material

        //UPDATE
        Task<MaterialType?> UpdateMaterialType(MaterialType materialType); //Actualizar un tipo de material

        //DELETE
        Task<bool> DeleteMaterialType(int id); //Eliminar un tipo de material

        //Validaciones adicionales
        Task<bool> MaterialTypeExists(int id);
        Task<bool> MaterialNameExists(string materialName);
    }
}
