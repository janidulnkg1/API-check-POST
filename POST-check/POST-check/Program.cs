using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.MapPost("/receive", async context =>
{
    // Read the request body
    using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
    {
        string requestBody = await reader.ReadToEndAsync();

        // Deserialize JSON
        if (!string.IsNullOrEmpty(requestBody))
        {
            dynamic json = JsonConvert.DeserializeObject(requestBody);
            Log.Information($"Received POST request with JSON data: {JsonConvert.SerializeObject(json, Formatting.Indented)}");
            Console.WriteLine($"Received POST request with JSON data: {JsonConvert.SerializeObject(json, Formatting.Indented)}");
        }
        else
        {
            Log.Warning("Received POST request with no JSON data.");
            Console.WriteLine("Received POST request with no JSON data.");
        }

        // Respond to the client

        await context.Response.WriteAsync("POST request received successfully");
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
