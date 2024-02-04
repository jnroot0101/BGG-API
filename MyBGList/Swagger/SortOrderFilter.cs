using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MyBGList.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyBGList.Swagger;

public class SortOrderFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        var attributes = context.ParameterInfo?
            .GetCustomAttributes(true)
            .OfType<SortOrderValidatorAttribute>();

        if (attributes != null)
            foreach (var attribute in attributes)
                parameter.Schema.Extensions.Add(
                    "pattern",
                    new OpenApiString(string.Join("|",
                        attribute.AllowedValues.Select(v => $"^{v}$"))));
    }
}