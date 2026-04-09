using LeavePlanner.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;

namespace LeavePlanner.Infrastructure.Validators
{
    public static class Validator
    {
        private const string SingleEntityNotFoundMessage = "No entity of type {EntityType} was found.";
        private const string MultipleEntitiesNotFoundMessage = "No entities of type {EntityType} were found.";

        private static void LogMissingEntity<T>(ILogger logger, string messageTemplate)
        {
            logger.LogError(messageTemplate, typeof(T).Name);
        }

        public static Task ValidEntities<T>(IEnumerable<T>? entities, ILogger logger)
        {
            if (entities != null && entities.Any())
                return Task.CompletedTask;

            LogMissingEntity<T>(logger, MultipleEntitiesNotFoundMessage);

            throw new NullEntityException($"No entities of type {typeof(T).Name} were found.");
        }

        public static Task ValidEntity<T>(T? entity, ILogger logger)
            where T : class
        {
            if (entity is not null)
                return Task.CompletedTask;

            LogMissingEntity<T>(logger, SingleEntityNotFoundMessage);

            throw new NullEntityException($"No entity of type {typeof(T).Name} was found.");
        }
    }
}