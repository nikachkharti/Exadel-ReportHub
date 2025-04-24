using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ReportHub.Identity.Middlewares;

public class TokenEndpointDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        const string path = "/auth/login";

        if (swaggerDoc.Paths.TryGetValue(path, out var existingPathItem))
        {
            if (existingPathItem.Operations.TryGetValue(OperationType.Post, out var postOperation))
            {
                postOperation.RequestBody = new OpenApiRequestBody
                {
                    Required = true,
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/x-www-form-urlencoded"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["grant_type"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Example = new Microsoft.OpenApi.Any.OpenApiString("password"),
                                       
                                    },
                                    ["client_id"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Default = new Microsoft.OpenApi.Any.OpenApiString("report-hub"),
                                    },
                                    ["client_secret"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Default = new Microsoft.OpenApi.Any.OpenApiString("client_secret_key"),
                                    },
                                    ["scope"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Example = new Microsoft.OpenApi.Any.OpenApiString("report-hub-api-scope roles offline_access"),
                                        
                                    },
                                    ["username"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                    },
                                    ["password"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                    },
                                },
                                Required = new HashSet<string> { "grant_type", "client_id", "client_secret", "scope", "username", "password" }
                            }
                        }
                    }
                };
            }
        }
    }

}
