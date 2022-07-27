// ReSharper disable once CheckNamespace
//NOTE: use in auto-generated protofiles.
namespace CustomTypes;

using JetBrains.Annotations;

[PublicAPI]
public partial class DecimalValue
{
    private const decimal NANO_FACTOR = 1_000_000_000;

    public DecimalValue(long units, int nanos)
    {
        Units = units;
        Nanos = nanos;
    }

    public static implicit operator decimal(DecimalValue grpcDecimal)
    {
        return grpcDecimal.Units + grpcDecimal.Nanos / NANO_FACTOR;
    }

    public static implicit operator DecimalValue(decimal value)
    {
        var units = decimal.ToInt64(value);
        var nanos = decimal.ToInt32((value - units) * NANO_FACTOR);

        return new DecimalValue(units, nanos);
    }
}