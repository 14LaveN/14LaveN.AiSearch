using TeamTasks.Domain.Common.ValueObjects;

namespace Domain.UnitTests.TestData;

public sealed class NameTestData
{
    public static readonly Name ValidName = Name.Create(nameof(Name)).Value;

    public static readonly string LongerThanAllowedName = new('*', Name.MaxLength + 1);
}