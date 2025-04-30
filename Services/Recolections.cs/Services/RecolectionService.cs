using System.Text.Json;
using Data.DBConext;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Services.Recolections.cs.DTOs;
using Services.Recolections.cs.Enumerables;

namespace Services.Recolections.cs.Services
{
    public class RecolectionService : IRecolectionService
    {
        private readonly RecollectionProjectContext _context;
        public RecolectionService(RecollectionProjectContext context)
        {
            _context = context;
        }

        //CREATE
        public async Task<Recolection?> CreateAsync(RecolectionForAdditionDTO recolectionForAddition)
        {
            if (recolectionForAddition == null)
            {
                return null;
            }

            if (recolectionForAddition.UserId == 0)
            {
                return null;
            }

            if (!await _context.Users.AnyAsync(u => u.UserId == recolectionForAddition.UserId))
            {
                return null;
            }

            Recolection recolection = new()
            {
                DateRecolection = recolectionForAddition.DateRecolection ?? default,
                MaterialTypeId = recolectionForAddition.MaterialTypeId,
                UserId = recolectionForAddition.UserId,
                Weight = recolectionForAddition.Weight
            };

            SetDefaultRecollectionDate(recolection);

            // Validar la recolección antes de agregarla
            var recolectionUpdateDTO = new RecolectionUpdateDTO
            {
                DateRecolection = recolection.DateRecolection,
                MaterialTypeId = recolection.MaterialTypeId,
                Weight = recolection.Weight
            };

            if (!await ValidateRecollection(recolectionUpdateDTO))
            {
                return null;
            }

            await _context.Recolections.AddAsync(recolection);
            await _context.SaveChangesAsync();

            return recolection;
        }


        //READ
        public async Task<Recolection?> GetRecolectionById(int id)
        {
            //Buscar una recolección por su id
            var recolection = await _context.Recolections
                .Include(r => r.User)
                .Include(r => r.MaterialType)
                .FirstOrDefaultAsync(r => r.RecolectionId == id);

            if (recolection == null)
            {
                return null; // Si no se encuentra la recolección, retornar null
            }
            return recolection;
        }
        public async Task<IEnumerable<Recolection>> GetAllRecolections(
            int? userId = null,
            RecollectionTypeEnum? type = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)

        {
            //Obtener todas las recolecciones
            var query = _context.Recolections.AsQueryable();

            if (userId.HasValue)
                query = query.Where(r => r.UserId == userId);

            if (type.HasValue)
                query = query.Where(r => r.MaterialTypeId == (int)type);

            if (fromDate.HasValue)
                query = query.Where(r => r.DateRecolection >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.DateRecolection <= toDate.Value);

            return await query
                .Include(r => r.User)
                .Include(r => r.MaterialType)
                .OrderByDescending(r => r.DateRecolection)
                .ToListAsync();
        }

        //UPDATE
        public async Task<Recolection?> UpdateRecolection(RecolectionUpdateDTO recolectionUpdate)
        {
            if (recolectionUpdate == null)
            {
                return null;
            }

            var existingRecolection = await _context.Recolections
                .FirstOrDefaultAsync(r => r.RecolectionId == recolectionUpdate.RecolectionId);

            if (existingRecolection == null)
            {
                return null;
            }


            existingRecolection.DateRecolection = recolectionUpdate.DateRecolection ?? existingRecolection.DateRecolection;
            existingRecolection.MaterialTypeId = recolectionUpdate.MaterialTypeId;
            existingRecolection.Weight = recolectionUpdate.Weight;
            existingRecolection.UserId = recolectionUpdate.UserId;

            _context.Recolections.Update(existingRecolection);
            await _context.SaveChangesAsync();

            return existingRecolection;
        }

        //DELETE
        public async Task<bool> DeleteRecolection(int id)
        {
            var recollection = await GetRecolectionById(id);

            if (recollection == null)
            {
                return false;
            }

            if (recollection.DateRecolection.Date == DateTime.Today)
            {
                return false; // No se puede eliminar una recolección programada para hoy
            }

            _context.Recolections.Remove(recollection);
            await _context.SaveChangesAsync();

            // Si la recolección fue eliminada, se retorna true
            return true;
        }

        //Registrar el peso de una recolección
        public async Task<Recolection?> RegisterWeight(int id, decimal weight)
        {
            if (weight <= 0)
            {
                return null;
            }

            var recolection = await _context.Recolections
                .Include(r => r.MaterialType)
                .FirstOrDefaultAsync(r => r.RecolectionId == id);

            if (recolection == null || (RecollectionTypeEnum)recolection.MaterialTypeId != RecollectionTypeEnum.Inorganic)
            {
                return null;
            }

            if (!recolection.MaterialType.WithWeight)
            {
                return null;
            }

            recolection.Weight = weight;
            await _context.SaveChangesAsync();
            return recolection;
        }



        public async Task<IEnumerable<Recolection?>> GetUpComingRecollections(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                return Enumerable.Empty<Recolection?>();
            }

            return await _context.Recolections
                .Include(r => r.User)
                .Include(r => r.MaterialType)
                .Where(r => r.DateRecolection >= startDate && r.DateRecolection <= endDate)
                .OrderBy(r => r.DateRecolection)
                .ToListAsync();
        }

        //Método para validar una recolección
        public async Task<bool> ValidateRecollection(RecolectionUpdateDTO recolectionUpdate)
        {
            if (recolectionUpdate == null)
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(RecollectionTypeEnum), recolectionUpdate.MaterialTypeId))
            {
                return false;
            }

            if (recolectionUpdate.DateRecolection < DateTime.Today)
            {
                return false;
            }

            var materialType = await _context.MaterialTypes
                .FirstOrDefaultAsync(m => m.MaterialTypeId == recolectionUpdate.MaterialTypeId);

            if (materialType == null)
            {
                return false;
            }

            switch ((RecollectionTypeEnum)recolectionUpdate.MaterialTypeId)
            {
                case RecollectionTypeEnum.Organic:
                    if (recolectionUpdate.Weight.HasValue)
                    {
                        return false;
                    }
                    break;

                case RecollectionTypeEnum.Inorganic:
                    if (!recolectionUpdate.Weight.HasValue)
                    {
                        return false;
                    }
                    break;

                case RecollectionTypeEnum.Hazardous:
                    if (recolectionUpdate.Weight.HasValue)
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }

        //Método para establecer una recolección por defecto 
        private void SetDefaultRecollectionDate(Recolection recolection)
        {
            if (recolection.DateRecolection != default)
            {
                return; // Si ya tiene una fecha, no hacer nada
            }


            var type = (RecollectionTypeEnum)recolection.MaterialTypeId;
            recolection.DateRecolection = type switch
            {
                RecollectionTypeEnum.Organic => DateTime.Today.AddDays(7),       // Semanal
                RecollectionTypeEnum.Inorganic => DateTime.Today.AddDays(14),    // Quincenal
                RecollectionTypeEnum.Hazardous => DateTime.Today.AddDays(30),    // Mensual
                _ =>DateTime.Today
            };
        }


        // Método para obtener el reporte de recolecciones de un usuario
        public async Task<ReportDTO> GetUserRecolectionsReport(int userId, DateTime? startDate, DateTime? endDate)
        {
            // Obtener usuario y validar
            var user = await _context.Users
                .Include(u => u.Recolections)
                .ThenInclude(r => r.MaterialType)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return null; // Si el usuario no existe, retornar null
            }

                // Filtrar recolecciones por rango de fechas
                var filteredRecolections = user.Recolections
                .Where(r => (!startDate.HasValue || r.DateRecolection >= startDate.Value) &&
                            (!endDate.HasValue || r.DateRecolection <= endDate.Value))
                .OrderByDescending(r => r.DateRecolection)
                .ToList();

            // Calcular total de peso (solo para materiales que lo requieren)
            decimal totalWeight = filteredRecolections
                .Where(r => r.MaterialType.WithWeight && r.Weight.HasValue)
                .Sum(r => r.Weight.Value);

            // Construir el resumen de recolecciones
            var recolectionSummary = filteredRecolections
                .GroupBy(r => r.MaterialType.MaterialName)
                .ToDictionary(g => g.Key, g => g.Count());

            // Construir el reporte
            return new ReportDTO
            {
                User = user,
                Recolections = filteredRecolections.Select(r => new RecolectionDetail
                {
                    Date = r.DateRecolection,
                    MaterialType = r.MaterialType.MaterialName,
                    Weight = r.Weight,
                }).ToList(),
                TotalWeight = totalWeight,
                RecolectionSummary = recolectionSummary
            };
        }

    }
}

