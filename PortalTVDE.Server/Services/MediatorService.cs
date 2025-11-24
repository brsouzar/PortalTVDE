using Microsoft.EntityFrameworkCore;
using PortalTVDE.Server.Data;
using PortalTVDE.Server.Models;
using PortalTVDE.Server.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Server.Services
{
    public class MediatorService : IMediatorService
    {
        private readonly ApplicationDbContext _context;
        public MediatorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResultDto<MediatorDto>> GetAllAsync(MediatorQueryDto query)
        {
            var dbQuery = _context.Mediators.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                dbQuery = dbQuery.Where(m => m.Name.Contains(query.Name));

            if (query.Tier.HasValue)
                dbQuery = dbQuery.Where(m => m.Tier == query.Tier.Value);

            var total = await dbQuery.CountAsync();

            var items = await dbQuery
                .OrderBy(m => m.Id)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(m => new MediatorDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Tier = m.Tier,
                    CommissionRate = m.CommissionRate,
                    Email = m.Email
                })
                .ToListAsync();

            return new PaginatedResultDto<MediatorDto>
            {
                Items = items,
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<MediatorDto> UpdateAsync(MediatorUpdateDto dto)
        {
            var mediator = await _context.Mediators.FindAsync(dto.Id);
            if (mediator == null) throw new KeyNotFoundException("Mediator não encontrado.");

            mediator.Tier = dto.Tier;
            mediator.CommissionRate = dto.CommissionRate;

            await _context.SaveChangesAsync();

            return new MediatorDto
            {
                Id = mediator.Id,
                Name = mediator.Name,
                Tier = mediator.Tier,
                CommissionRate = mediator.CommissionRate,
                Email = mediator.Email
            };
        }


        public async Task<MediatorDto> CreateAsync(MediatorCreateDto dto)
        {
            var mediator = new Mediator
            {
                Name = dto.Name!,
                Tier = dto.Tier,
                CommissionRate = dto.CommissionRate,
                Email = dto.Email!
            };

            _context.Mediators.Add(mediator);
            await _context.SaveChangesAsync();

            return new MediatorDto
            {
                Id = mediator.Id,
                Name = mediator.Name,
                Tier = mediator.Tier,
                CommissionRate = mediator.CommissionRate,
                Email = mediator.Email
            };
        }

        
    }
}


