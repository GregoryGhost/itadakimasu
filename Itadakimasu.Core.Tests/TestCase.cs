namespace Itadakimasu.Core.Tests;

using JetBrains.Annotations;

[PublicAPI]
public record TestCase<TExpected, TInputData>
    where TExpected : notnull
    where TInputData : notnull
{
    public TExpected Expected { get; init; } = default!;

    public TInputData InputData { get; init; } = default!;

    public string TestCaseName { get; init; } = null!;
}