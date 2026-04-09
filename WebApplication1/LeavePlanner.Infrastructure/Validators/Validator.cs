using LeavePlanner.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;

namespace LeavePlanner.Infrastructure.Validators
{
    public static class Validator
    {
        public static Task ValidEntities<T>(IEnumerable<T>? entities, ILogger logger)
        {
            if (entities != null && entities.Any()) return Task.CompletedTask;
            logger.LogError(
                "No entities of type {EntityType} were found.",
                typeof(T).Name);

            throw new NullEntityException($"No entities of type {typeof(T).Name} were found.");

        }

        public static Task ValidEntity<T>(T? entity, ILogger logger)
        {
            if (!EqualityComparer<T>.Default.Equals(entity, default)) return Task.CompletedTask;
            logger.LogError(
                "No entity of type {EntityType} was found.",
                typeof(T).Name);

            throw new NullEntityException($"No entity of type {typeof(T).Name} was found.");

        }
    }
}
