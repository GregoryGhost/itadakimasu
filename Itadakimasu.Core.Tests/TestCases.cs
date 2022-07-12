namespace Itadakimasu.Core.Tests;

using System.Collections;

using JetBrains.Annotations;

[PublicAPI]
public abstract class TestCases<TExpected, TInputData> : IEnumerable
    where TExpected : notnull
    where TInputData : notnull
{
    protected abstract IEnumerable<TestCase<TExpected, TInputData>> GetTestCases();

    public IEnumerator GetEnumerator()
    {
        return GetTestCases().Select(x => new object[] {x.TestCaseName, x.InputData, x.Expected})
                             .GetEnumerator();
    }
}