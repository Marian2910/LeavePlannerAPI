using LeavePlanner.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;

namespace LeavePlanner.Infrastructure.Validators
{
    public static class Validator
    {
        private const string SingleEntityNotFoundMessage = "No entity of type {EntityType} was found.";
        private const string MultipleEntitiesNotFoundMessage = "No entities of type {EntityType} were found.";

        public static Task ValidEntities<T>(IEnumerable<T>? entities, ILogger logger)
        {
            if (entities != null && entities.Any())
                return Task.CompletedTask;

            logger.LogError(MultipleEntitiesNotFoundMessage, typeof(T).Name);

            throw new NullEntityException($"No entities of type {typeof(T).Name} were found.");
        }

        public static Task ValidEntity<T>(T? entity, ILogger logger)
            where T : class
        {
            if (entity is not null)
                return Task.CompletedTask;

            logger.LogError(SingleEntityNotFoundMessage, typeof(T).Name);

            throw new NullEntityException($"No entity of type {typeof(T).Name} was found.");
        }
    }
}