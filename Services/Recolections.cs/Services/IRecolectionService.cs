using Data.Models;
using Services.Recolections.cs.DTOs;
using Services.Recolections.cs.Enumerables;

namespace Services.Recolections.cs.Services
{
    public interface IRecolectionService
    {

        //CREATE
        Task<Recolection?> CreateAsync(RecolectionForAdditionDTO recolectionForAddition); //Crear una recolección

        //READ
        Task<Recolection?> GetRecolectionById(int id); //Obtener una recolección por su id
        Task<IEnumerable<Recolection>> GetAllRecolections(int? userId = null,
            RecollectionTypeEnum? type = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        //UPDATE
        Task<Recolection?> UpdateRecolection(RecolectionUpdateDTO recolectionUpdate); //Actualizar una recolección
        Task<Recolection?> RegisterWeight(int id, decimal weight); //Registrar el peso de una recolección

        //DELETE
        Task<bool> DeleteRecolection(int id); //Eliminar una recolección
        Task<IEnumerable<Recolection?>> GetUpComingRecollections(DateTime startDate, DateTime endDate); //Obtener las recolecciones próximas

        //Reportes
        Task<ReportDTO> GetUserRecolectionsReport(int userId, DateTime? startDate, DateTime? endDate);
    }
}
