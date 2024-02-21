using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MyBGList.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using ICustomAttributeProvider = System.Reflection.ICustomAttributeProvider;

namespace MyBGList.Swagger;

public class CustomKeyValueFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var caProvider = context.MemberInfo
                         ?? context.ParameterInfo
                             as ICustomAttributeProvider;

        var attributes = caProvider?
            .GetCustomAttributes(true)
            .OfType<CustomKeyValueAttribute>();

        if (attributes != null)
            foreach (var attribute in attributes)
                schema.Extensions.Add(
                    attribute.Key,
                    new OpenApiString(attribute.Value)
                );
    }
}