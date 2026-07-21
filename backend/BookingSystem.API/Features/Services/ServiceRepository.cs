using BookingSystem.API.Shared.Database;
using Dapper;

namespace BookingSystem.API.Features.Services;

public class ServiceRepository : IServiceRepository
{
    private readonly DatabaseConnection _db;

    public ServiceRepository(DatabaseConnection db) => _db = db;

    public async Task<List<Service>> GetAllAsync(bool includeInactive)
    {
        await using var conn = await _db.OpenAsync();
        var sql = "SELECT id, name, description, duration, price, is_active FROM services";
        if (!includeInactive)
            sql += " WHERE is_active = TRUE";
        sql += " ORDER BY id";

        var services = await conn.QueryAsync<Service>(sql);
        return services.ToList();
    }

    public async Task<Service?> GetByIdAsync(int id)
    {
        await using var conn = await _db.OpenAsync();
        return await conn.QueryFirstOrDefaultAsync<Service>(
            "SELECT id, name, description, duration, price, is_active FROM services WHERE id = @id",
            new { id });
    }

    public async Task<Service> CreateAsync(string name, string? description, int duration, decimal price)
    {
        await using var conn = await _db.OpenAsync();
        return await conn.QuerySingleAsync<Service>(@"
            INSERT INTO services (name, description, duration, price, is_active)
            VALUES (@name, @description, @duration, @price, TRUE)
            RETURNING id, name, description, duration, price, is_active",
            new { name, description, duration, price });
    }

    public async Task<Service?> UpdateAsync(int id, string name, string? description, int duration, decimal price, bool isActive)
    {
        await using var conn = await _db.OpenAsync();
        return await conn.QueryFirstOrDefaultAsync<Service>(@"
            UPDATE services
            SET name = @name, description = @description, duration = @duration, price = @price, is_active = @isActive
            WHERE id = @id
            RETURNING id, name, description, duration, price, is_active",
            new { id, name, description, duration, price, isActive });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var conn = await _db.OpenAsync();
        var affected = await conn.ExecuteAsync("DELETE FROM services WHERE id = @id", new { id });
        return affected > 0;
    }
}
