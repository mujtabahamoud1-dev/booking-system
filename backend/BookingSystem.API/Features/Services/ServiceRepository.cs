using BookingSystem.API.Shared.Database;
using Npgsql;

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

        await using var cmd = new NpgsqlCommand(sql, conn);

        var services = new List<Service>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            services.Add(MapService(reader));

        return services;
    }

    public async Task<Service?> GetByIdAsync(int id)
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT id, name, description, duration, price, is_active FROM services WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapService(reader) : null;
    }

    public async Task<Service> CreateAsync(string name, string? description, int duration, decimal price)
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = new NpgsqlCommand(@"
            INSERT INTO services (name, description, duration, price, is_active)
            VALUES (@name, @description, @duration, @price, TRUE)
            RETURNING id, name, description, duration, price, is_active", conn);

        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("description", description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("duration", duration);
        cmd.Parameters.AddWithValue("price", price);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return MapService(reader);
    }

    public async Task<Service?> UpdateAsync(int id, string name, string? description, int duration, decimal price, bool isActive)
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = new NpgsqlCommand(@"
            UPDATE services
            SET name = @name, description = @description, duration = @duration, price = @price, is_active = @isActive
            WHERE id = @id
            RETURNING id, name, description, duration, price, is_active", conn);

        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("description", description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("duration", duration);
        cmd.Parameters.AddWithValue("price", price);
        cmd.Parameters.AddWithValue("isActive", isActive);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapService(reader) : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = new NpgsqlCommand("DELETE FROM services WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static Service MapService(NpgsqlDataReader r) => new()
    {
        Id          = r.GetInt32(0),
        Name        = r.GetString(1),
        Description = r.IsDBNull(2) ? null : r.GetString(2),
        Duration    = r.GetInt32(3),
        Price       = r.GetDecimal(4),
        IsActive    = r.GetBoolean(5)
    };
}
