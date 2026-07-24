using System.Data;
using Dapper;

namespace BookingSystem.API.Shared.Database;

// Dapper 2.1.x has no built-in parameter mapping for DateOnly/TimeOnly and throws
// "... cannot be used as a parameter value" when one is passed. Npgsql supports both
// natively, so these handlers just hand the value straight to the parameter (letting
// Npgsql infer date/time) and read the value Npgsql already materialized back out.
public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value) => parameter.Value = value;

    public override DateOnly Parse(object value) => value switch
    {
        DateOnly d  => d,
        DateTime dt => DateOnly.FromDateTime(dt),
        _ => throw new DataException($"Cannot convert {value.GetType()} to DateOnly.")
    };
}

public class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
{
    public override void SetValue(IDbDataParameter parameter, TimeOnly value) => parameter.Value = value;

    public override TimeOnly Parse(object value) => value switch
    {
        TimeOnly t  => t,
        TimeSpan ts => TimeOnly.FromTimeSpan(ts),
        DateTime dt => TimeOnly.FromDateTime(dt),
        _ => throw new DataException($"Cannot convert {value.GetType()} to TimeOnly.")
    };
}
