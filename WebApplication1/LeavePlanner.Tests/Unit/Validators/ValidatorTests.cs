using FluentAssertions;
using LeavePlanner.Infrastructure.Exceptions;
using LeavePlanner.Infrastructure.Validators;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

using FluentAssertions;
using LeavePlanner.Infrastructure.Exceptions;
using LeavePlanner.Infrastructure.Validators;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace LeavePlanner.Tests.Unit.Validators;

public class ValidatorTests
{
    [Fact]
    public async Task ValidEntities_ShouldComplete_WhenEntitiesExist()
    {
        var entities = new[] { "a", "b" };

        var act = () => Validator.ValidEntities(entities, NullLogger.Instance);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidEntities_ShouldThrow_WhenEntitiesAreNull()
    {
        IEnumerable<string>? entities = null;

        var act = () => Validator.ValidEntities(entities, NullLogger.Instance);

        await act.Should().ThrowAsync<NullEntityException>()
            .WithMessage("*String*");
    }

    [Fact]
    public async Task ValidEntities_ShouldThrow_WhenEntitiesAreEmpty()
    {
        var act = () => Validator.ValidEntities(Array.Empty<int>(), NullLogger.Instance);

        await act.Should().ThrowAsync<NullEntityException>()
            .WithMessage("*Int32*");
    }

    [Fact]
    public async Task ValidEntity_ShouldComplete_WhenEntityExists()
    {
        var act = () => Validator.ValidEntity(new object(), NullLogger.Instance);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidEntity_ShouldThrow_WhenEntityIsNull()
    {
        object? entity = null;

        var act = () => Validator.ValidEntity(entity, NullLogger.Instance);

        await act.Should().ThrowAsync<NullEntityException>()
            .WithMessage("*Object*");
    }
}
