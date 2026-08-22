using BookingSystem.API.Shared.Database;
using Dapper;
using SqlSearch = BookingSystem.API.Shared.Database.SqlSearch;

namespace BookingSystem.API.Features.Services;

public class ServiceRepository : IServiceRepository
{
    private const string Columns =
        "id, name, name_ar, description, description_ar, duration, price, is_active";
    private const string ServiceColumns =
        "s.id, s.name, s.name_ar, s.description, s.description_ar, s.duration, s.price, s.is_active";
    private const string DaysColumn = @"
        COALESCE((SELECT array_agg(DISTINCT sl.day_of_week ORDER BY sl.day_of_week)
                  FROM available_slots sl
                  WHERE sl.service_id = s.id), '{}'::int[]) AS days";

    private readonly DbSession _session;

    public ServiceRepository(DbSession session) => _session = session;

    public async Task<List<ServiceWithDays>> GetAllAsync(bool includeInactive)
    {
        var conn = await _session.GetConnectionAsync();
        var sql = $"SELECT {ServiceColumns}, {DaysColumn} FROM services s";
        if (!includeInactive)
            sql += " WHERE s.is_active = TRUE";
        sql += " ORDER BY s.id";

        var services = await conn.QueryAsync<ServiceWithDays>(sql);
        return services.AsList();
    }

    private const string Filter = @"
        (@Search::text IS NULL
         OR s.name ILIKE @Search::text ESCAPE '\'
         OR s.name_ar ILIKE @Search::text ESCAPE '\'
         OR s.description ILIKE @Search::text ESCAPE '\'
         OR s.description_ar ILIKE @Search::text ESCAPE '\')";

    public async Task<List<ServiceWithDays>> SearchAsync(ServiceQuery query)
    {
        var conn = await _session.GetConnectionAsync();
        var services = await conn.QueryAsync<ServiceWithDays>($@"
            SELECT {ServiceColumns}, {DaysColumn}
            FROM services s
            WHERE {Filter}
              AND (@IsActive::boolean IS NULL OR s.is_active = @IsActive::boolean)
            ORDER BY s.id",
            new { Search = SqlSearch.ToPattern(query.Search), query.IsActive });
        return services.AsList();
    }

    public async Task<ServiceCounts> CountAsync(ServiceQuery query)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleAsync<ServiceCounts>($@"
            SELECT COUNT(*)::int                                  AS total,
                   COUNT(*) FILTER (WHERE s.is_active)::int       AS active,
                   COUNT(*) FILTER (WHERE NOT s.is_active)::int   AS inactive
            FROM services s
            WHERE {Filter}",
            new { Search = SqlSearch.ToPattern(query.Search) });
    }

    public async Task<ServiceWithDays?> GetByIdAsync(int id)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<ServiceWithDays>(
            $"SELECT {ServiceColumns}, {DaysColumn} FROM services s WHERE s.id = @id",
            new { id });
    }

    // CTE because RETURNING cannot subquery the row it returns.
    public async Task<ServiceWithDays> CreateAsync(Service service)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleAsync<ServiceWithDays>($@"
            WITH s AS (
                INSERT INTO services (name, name_ar, description, description_ar, duration, price, is_active)
                VALUES (@Name, @NameAr, @Description, @DescriptionAr, @Duration, @Price, TRUE)
                RETURNING {Columns}
            )
            SELECT {ServiceColumns}, {DaysColumn} FROM s",
            service);
    }

    public async Task<ServiceWithDays?> UpdateAsync(Service service)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<ServiceWithDays>($@"
            WITH s AS (
                UPDATE services
                SET name           = @Name,
                    name_ar        = @NameAr,
                    description    = @Description,
                    description_ar = @DescriptionAr,
                    duration       = @Duration,
                    price          = @Price,
                    is_active      = @IsActive
                WHERE id = @Id
                RETURNING {Columns}
            )
            SELECT {ServiceColumns}, {DaysColumn} FROM s",
            service);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.ExecuteAsync("DELETE FROM services WHERE id = @id", new { id }) > 0;
    }
}
