using Infrastructure.Entities;
using Infrastructure.Exceptions;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Validators
{
    public class Validator
    {
        public static async Task ValidEntities<T>(List<T> entities, ILogger logger)
        {
            if (entities == null || !entities.Any())
            {
                logger.LogError($"No entities of type {typeof(T).Name}");
                throw new NullEntity($"No entities of type {typeof(T).Name}");
            }
        }

        public static async Task ValidEntity<T>(T entity, ILogger logger)
        {
            if (entity == null)
            {
                logger.LogError($"No entity of type {typeof(T).Name}");
                throw new NullEntity($"No entity of type {typeof(T).Name}");
            }
        }
    }
}