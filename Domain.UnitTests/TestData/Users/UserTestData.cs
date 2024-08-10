using TeamTasks.Domain.Common.ValueObjects;

namespace Domain.UnitTests.TestData.Users;

public static class UserTestData
{
    public static readonly FirstName FirstName = UserTestData.FirstName.Create("First").Value;

    public static readonly LastName LastName = UserTestData.LastName.Create("Last").Value;

    public static readonly Email Email = Email.Create("test@expensely.net").Value;

    public static readonly Password Password = UserTestData.Password.Create("123aA!").Value;

    public static User ValidUser => User.Create(FirstName, LastName, Email, Password);
}