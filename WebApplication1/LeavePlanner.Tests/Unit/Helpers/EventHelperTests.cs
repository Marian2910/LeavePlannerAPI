using FluentAssertions;
using LeavePlanner.Domain.Helper;
using Xunit;

namespace LeavePlanner.Tests.Unit.Helpers;

public class EventHelperTests
{
    private static DateTime CreateUtcDate(int year, int month, int day) =>
        DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc);

    [Fact]
    public void GetWorkDaysBetweenDates_ShouldCountOnlyWeekdays()
    {
        var start = CreateUtcDate(2026, 4, 6); // Monday
        var end = CreateUtcDate(2026, 4, 12); // Sunday

        var result = EventHelper.GetWorkDaysBetweenDates(start, end);

        result.Should().Be(5);
    }

    [Fact]
    public void GetWorkDaysBetweenDates_ShouldIncludeSingleWeekday()
    {
        var date = CreateUtcDate(2026, 4, 8); // Wednesday

        var result = EventHelper.GetWorkDaysBetweenDates(date, date);

        result.Should().Be(1);
    }

    [Fact]
    public void GetWorkDaysBetweenDates_ShouldThrow_WhenStartIsAfterEnd()
    {
        var act = () => EventHelper.GetWorkDaysBetweenDates(
            CreateUtcDate(2026, 4, 10),
            CreateUtcDate(2026, 4, 9));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Start date cannot be after the end date.");
    }
}
