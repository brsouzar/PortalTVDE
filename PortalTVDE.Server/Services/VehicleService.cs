using Microsoft.EntityFrameworkCore;
using PortalTVDE.Server.Data;
using PortalTVDE.Server.Models;
using PortalTVDE.Server.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Server.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly ApplicationDbContext _db;

        public VehicleService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedVehiclesResponse> GetVehiclesAsync(VehicleQueryDto query)
        {
            var q = _db.Vehicles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.LicensePlate))
                q = q.Where(v => v.LicensePlate.Contains(query.LicensePlate));

            if (!string.IsNullOrWhiteSpace(query.Make))
                q = q.Where(v => v.Make.Contains(query.Make));

            if (!string.IsNullOrWhiteSpace(query.Model))
                q = q.Where(v => v.Model.Contains(query.Model));

            var total = await q.CountAsync();

            var items = await q
                .OrderBy(v => v.LicensePlate)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(v => new VehicleDto
                {
                    Id = v.Id,
                    LicensePlate = v.LicensePlate,
                    Make = v.Make,
                    Model = v.Model,
                    PowerKW = v.PowerKW,
                    Year = v.Year,
                    Usage = v.Usage == null ? string.Empty : v.Usage.ToString()
                })
                .ToListAsync();

            return new PaginatedVehiclesResponse
            {
                TotalCount = total,
                Items = items
            };
        }

       public async Task<List<VehicleDto>> GetAllSimpleVehiclesAsync()
        {
            // Acessa a tabela de Vehicles, seleciona apenas os campos necessários
            // para exibição no dropdown e converte para uma lista de VehicleDto.
            var vehicles = await _db.Vehicles
                .OrderBy(v => v.Make) 
                .Select(v => new VehicleDto
                {
                    Id = v.Id,
                    LicensePlate = v.LicensePlate,
                    Make = v.Make,
                    Model = v.Model,
                    PowerKW = v.PowerKW,
                    Year = v.Year,
                    Usage = v.Usage == null ? string.Empty : v.Usage.ToString()
                })
                .ToListAsync();

            return vehicles;
        }

        public async Task<VehicleDto?> GetByIdAsync(int id)
        {
            return await _db.Vehicles
                .Where(v => v.Id == id)
                .Select(v => new VehicleDto
                {
                    Id = v.Id,
                    LicensePlate = v.LicensePlate,
                    Make = v.Make,
                    Model = v.Model,
                    PowerKW = v.PowerKW,
                    Year = v.Year,
                    Usage = v.Usage
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(VehicleCreateDto dto)
        {
            var v = new Vehicle
            {
                LicensePlate = dto.LicensePlate,
                Make = dto.Make,
                Model = dto.Model,
                PowerKW = dto.PowerKW,
                Year = dto.Year,
                Usage = dto.Usage,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                RowVersion = new byte[0]
            };

            _db.Vehicles.Add(v);
            await _db.SaveChangesAsync();

            return v.Id;
        }

        public async Task UpdateAsync(int id, VehicleCreateDto dto)
        {
            var v = await _db.Vehicles.FindAsync(id);
            if (v == null) return;

            v.LicensePlate = dto.LicensePlate;
            v.Make = dto.Make;
            v.Model = dto.Model;
            v.PowerKW = dto.PowerKW;
            v.Year = dto.Year;
            v.Usage = dto.Usage;

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var v = await _db.Vehicles.FindAsync(id);
            if (v == null) return;

            _db.Vehicles.Remove(v);
            await _db.SaveChangesAsync();
        }
    }
}
