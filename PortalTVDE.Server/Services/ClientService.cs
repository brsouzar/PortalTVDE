using Microsoft.EntityFrameworkCore;
using PortalTVDE.Server.Data;
using PortalTVDE.Server.Models;
using PortalTVDE.Server.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;


namespace PortalTVDE.Server.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _db;

        public ClientService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedClientsResponse> GetClientsAsync(ClientQueryDto query)
        {
            var q = _db.Clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                q = q.Where(c => c.Name.Contains(query.Name));

            if (!string.IsNullOrWhiteSpace(query.Email))
                q = q.Where(c => c.Email.Contains(query.Email));

            if (!string.IsNullOrWhiteSpace(query.NIF))
                q = q.Where(c => c.NIF.Contains(query.NIF));

            var total = await q.CountAsync();

            var items = await q
                .OrderBy(c => c.Name)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new ClientDtoWithId
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    NIF = c.NIF,
                    BirthDate = c.BirthDate
                })
                .ToListAsync();

            return new PaginatedClientsResponse
            {
                TotalCount = total,
                Items = items
            };
        }

        public async Task<ClientDtoWithId?> GetByIdAsync(int id)
        {
            return await _db.Clients
                .Where(c => c.Id == id)
                .Select(c => new ClientDtoWithId
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    NIF = c.NIF,
                    BirthDate = c.BirthDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(ClientDto dto)
        {
            var c = new Clientt
            {
                Name = dto.Name,
                Email = dto.Email,
                NIF = dto.NIF,
                BirthDate = dto.BirthDate
            };

            _db.Clients.Add(c);
            await _db.SaveChangesAsync();
            return c.Id;
        }

        public async Task UpdateAsync(int id, ClientDto dto)
        {
            var c = await _db.Clients.FindAsync(id);
            if (c == null) return;

            c.Name = dto.Name;
            c.Email = dto.Email;
            c.NIF = dto.NIF;
            c.BirthDate = dto.BirthDate;

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var c = await _db.Clients.FindAsync(id);
            if (c == null) return;

            _db.Clients.Remove(c);
            await _db.SaveChangesAsync();
        }
    }
}
