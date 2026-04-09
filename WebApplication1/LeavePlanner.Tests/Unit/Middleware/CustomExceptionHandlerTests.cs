using System.Text;
using FluentAssertions;
using LeavePlanner.Infrastructure.Exceptions;
using LeavePlanner.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace LeavePlanner.Tests.Unit.Middleware;

public class CustomExceptionHandlerTests
{
    [Fact]
    public async Task InvokeAsync_ShouldReturnNotFound_ForNullEntity()
    {
        var middleware = CreateMiddleware(_ => throw new NullEntity("missing"));
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        GetBody(context).Should().Contain("missing");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnForbidden_ForForbiddenMessage()
    {
        var middleware = CreateMiddleware(_ => throw new Exception("forbidden area"));
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        GetBody(context).Should().Contain("forbidden area");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnInternalServerError_ForUnknownException()
    {
        var middleware = CreateMiddleware(_ => throw new Exception("boom"));
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        GetBody(context).Should().Contain("server is having some trouble");
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenNoExceptionOccurs()
    {
        var called = false;
        var middleware = CreateMiddleware(_ =>
        {
            called = true;
            return Task.CompletedTask;
        });
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        called.Should().BeTrue();
    }

    private static CustomExceptionHandler CreateMiddleware(RequestDelegate next)
    {
        return new CustomExceptionHandler(NullLogger<CustomExceptionHandler>.Instance, next);
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static string GetBody(DefaultHttpContext context)
    {
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body, Encoding.UTF8, leaveOpen: true);
        return reader.ReadToEnd();
    }

    private sealed class TestNotNullEntity : NotNullEntity
    {
        public TestNotNullEntity(string message) : base(message, new Exception("inner"))
        {
        }
    }
}
