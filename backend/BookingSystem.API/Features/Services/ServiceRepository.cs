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

    public async Task<Service> CreateAsync(Service service)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleAsync<Service>(@"
            INSERT INTO services (name, description, duration, price, is_active)
            VALUES (@Name, @Description, @Duration, @Price, TRUE)
            RETURNING id, name, description, duration, price, is_active",
            service);
    }

    public async Task<Service?> UpdateAsync(Service service)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<Service>(@"
            UPDATE services
            SET name = @Name, description = @Description, duration = @Duration, price = @Price, is_active = @IsActive
            WHERE id = @Id
            RETURNING id, name, description, duration, price, is_active",
            service);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.ExecuteAsync("DELETE FROM services WHERE id = @id", new { id }) > 0;
    }
}
