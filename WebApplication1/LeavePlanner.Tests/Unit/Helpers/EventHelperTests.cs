using FluentAssertions;
using LeavePlanner.Domain.Helper;
using Xunit;

namespace LeavePlanner.Tests.Unit.Helpers;

public class EventHelperTests
{
    [Fact]
    public void GetWorkDaysBetweenDates_ShouldCountOnlyWeekdays()
    {
        var start = new DateTime(2026, 4, 6); // Monday
        var end = new DateTime(2026, 4, 12); // Sunday

        var result = EventHelper.GetWorkDaysBetweenDates(start, end);

        result.Should().Be(5);
    }

    [Fact]
    public void GetWorkDaysBetweenDates_ShouldIncludeSingleWeekday()
    {
        var date = new DateTime(2026, 4, 8); // Wednesday

        var result = EventHelper.GetWorkDaysBetweenDates(date, date);

        result.Should().Be(1);
    }

    [Fact]
    public void GetWorkDaysBetweenDates_ShouldThrow_WhenStartIsAfterEnd()
    {
        var act = () => EventHelper.GetWorkDaysBetweenDates(
            new DateTime(2026, 4, 10),
            new DateTime(2026, 4, 9));

        act.Should().Throw<Exception>()
            .WithMessage("Start date cannot be after the end date.");
    }
}
