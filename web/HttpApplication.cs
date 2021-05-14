using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace web
{
    public class HttpApplication : IHttpApplication<HttpContext>
    {
        public HttpContext CreateContext(IFeatureCollection contextFeatures)
        {
            return new DefaultHttpContext(contextFeatures);
        }

        public void DisposeContext(HttpContext context, Exception exception)
        {
        }

        public async Task ProcessRequestAsync(HttpContext context)
        {
            bool emptyBody = false;

            var jsonObjects = new Dictionary<string, object>();

            try
            {
                using var document = await JsonDocument.ParseAsync(context.Request.Body);

                JsonElement root = document.RootElement;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    foreach (JsonProperty property in root.EnumerateObject())
                    {
                        jsonObjects.Add(property.Name, property.Value.ToString());
                    }

                    jsonObjects.Add("feedback", "your json has just gone through a native kestrel");
                }
                else if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement element in root.EnumerateArray())
                    {
                    }
                }
            }
            catch
            {
                emptyBody = true;
            }

            if (emptyBody)
            {
                jsonObjects.Add("firstname", "Natty");
                jsonObjects.Add("lastname", "de Balancet");
                jsonObjects.Add("info", "copy this json and post it back");
            }

            var jsonWriterOptions = new JsonWriterOptions
            {
                Indented = true
            };

            var memoryStream = new MemoryStream();

            using (var utf8JsonWriter = new Utf8JsonWriter(memoryStream, jsonWriterOptions))
            {
                utf8JsonWriter.WriteStartObject();

                foreach (KeyValuePair<string, object> keyValuePair in jsonObjects)
                {
                    utf8JsonWriter.WriteString(keyValuePair.Key, keyValuePair.Value.ToString());
                }

                utf8JsonWriter.WriteEndObject();
                utf8JsonWriter.Flush();
            }

            context.Response.ContentLength = memoryStream.Length;
            context.Response.ContentType = "application/json";

            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(context.Response.Body);
        }
    }
}
