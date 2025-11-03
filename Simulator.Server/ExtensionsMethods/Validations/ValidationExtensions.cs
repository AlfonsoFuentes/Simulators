using Simulator.Server.Databases.Contracts;
using Simulator.Shared.Intefaces;


namespace Simulator.Server.ExtensionsMethods.Validations
{
    public static class ValidationExtensions
    {
        public static Expression<Func<TEntity, bool>> BuildStringCriteria<TEntity, TDto>(
            this TDto dto,
            string dtoPropertyName,
            string entityPropertyName)
            where TEntity : IEntity
            where TDto : IDto
        {
            // 1. Obtener el valor del DTO
            var dtoProperty = typeof(TDto).GetProperty(dtoPropertyName);
            if (dtoProperty == null)
                return x => false;

            var dtoValue = dtoProperty.GetValue(dto) as string;
            if (string.IsNullOrEmpty(dtoValue))
                return x => false;

            // 2. Crear la expresión
            var parameter = Expression.Parameter(typeof(TEntity), "x");

            // x.{entityPropertyName}
            var entityProperty = Expression.Property(parameter, entityPropertyName);
            var valueConstant = Expression.Constant(dtoValue, typeof(string));

            // x.{entityPropertyName}.Equals(dtoValue, OrdinalIgnoreCase)
            var equalsMethod = typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(StringComparison) })!;
            var equalsCall = Expression.Call(entityProperty, equalsMethod, valueConstant, Expression.Constant(StringComparison.OrdinalIgnoreCase));

            // 3. Condición de exclusión de ID (si es actualización)
            Expression idCondition = Expression.Constant(true);
            if (dto.IsCreated && dto.Id != Guid.Empty)
            {
                var idProperty = Expression.Property(parameter, "Id");
                var idConstant = Expression.Constant(dto.Id, typeof(Guid));
                idCondition = Expression.NotEqual(idProperty, idConstant);
            }

            // 4. Combinar condiciones
            var body = Expression.AndAlso(equalsCall, idCondition);
            return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
        }
    }
}