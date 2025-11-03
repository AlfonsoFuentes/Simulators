using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Intefaces;
using System.Linq.Expressions;

namespace Simulator.Client.Infrastructure.ExtensionMethods
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> MustBeUnique<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            IClientCRUDService service,
            Expression<Func<T, string>> propertySelector)
            where T : IDto, IValidationRequest
        {
            // Obtener el nombre de la propiedad usando reflection
            var propertyName = GetPropertyName(propertySelector);

            return ruleBuilder.MustAsync(async (dto, value, ct) =>
            {
                // Establecer la clave de validación
                dto.ValidationKey = propertyName;

                // Llamar al servicio de validación
                var result = await service.Validate(dto);
                return result.Succeeded;
            });
        }

        private static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            return expression.Body switch
            {
                MemberExpression member => member.Member.Name,
                _ => throw new ArgumentException("Expression must be a property selector")
            };
        }
    }
}
