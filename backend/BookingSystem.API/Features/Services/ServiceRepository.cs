using BookingSystem.API.Shared.Database;
using Dapper;
using SqlSearch = BookingSystem.API.Shared.Database.SqlSearch;

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

    // The admin list. Both filters are optional and applied in the database;
    // parameters are cast explicitly because Postgres cannot infer the type of
    // one that only appears beside IS NULL.
    private const string Filter = @"
        (@Search::text IS NULL
         OR name ILIKE @Search::text ESCAPE '\'
         OR description ILIKE @Search::text ESCAPE '\')";

    public async Task<List<Service>> SearchAsync(ServiceQuery query)
    {
        var conn = await _session.GetConnectionAsync();
        var services = await conn.QueryAsync<Service>($@"
            SELECT id, name, description, duration, price, is_active
            FROM services
            WHERE {Filter}
              AND (@IsActive::boolean IS NULL OR is_active = @IsActive::boolean)
            ORDER BY id",
            new { Search = SqlSearch.ToPattern(query.Search), query.IsActive });
        return services.AsList();
    }

    public async Task<ServiceCounts> CountAsync(ServiceQuery query)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleAsync<ServiceCounts>($@"
            SELECT COUNT(*)::int                                  AS total,
                   COUNT(*) FILTER (WHERE is_active)::int         AS active,
                   COUNT(*) FILTER (WHERE NOT is_active)::int     AS inactive
            FROM services
            WHERE {Filter}",
            new { Search = SqlSearch.ToPattern(query.Search) });
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
