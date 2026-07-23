using BookingSystem.API.Shared.Database;
using Dapper;

namespace BookingSystem.API.Features.Services;

public class ServiceRepository : IServiceRepository
{
    private readonly DbSession _session;

    public ServiceRepository(DbSession session) => _session = session;

    public async Task<List<Service>> GetAllAsync(bool includeInactive)
    {
        var conn = await _session.GetConnectionAsync();
        var sql = "SELECT id, name, description, duration, price, is_active FROM services";
        if (!includeInactive)
            sql += " WHERE is_active = TRUE";
        sql += " ORDER BY id";

        var services = await conn.QueryAsync<Service>(sql);
        return services.AsList();
    }

    public async Task<Service?> GetByIdAsync(int id)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<Service>(
            "SELECT id, name, description, duration, price, is_active FROM services WHERE id = @id",
            new { id });
    }

    public async Task<Service> CreateAsync(string name, string? description, int duration, decimal price)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleAsync<Service>(@"
            INSERT INTO services (name, description, duration, price, is_active)
            VALUES (@name, @description, @duration, @price, TRUE)
            RETURNING id, name, description, duration, price, is_active",
            new { name, description, duration, price });
    }

    public async Task<Service?> UpdateAsync(int id, string name, string? description, int duration, decimal price, bool isActive)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<Service>(@"
            UPDATE services
            SET name = @name, description = @description, duration = @duration, price = @price, is_active = @isActive
            WHERE id = @id
            RETURNING id, name, description, duration, price, is_active",
            new { id, name, description, duration, price, isActive });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.ExecuteAsync("DELETE FROM services WHERE id = @id", new { id }) > 0;
    }
}
