using FileSharing.API.Extensions;
using FileSharing.API.Workers;
using FileSharing.Infrastructure.Services;
using FleSharing.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddJWTAuthentication();
builder.Services.AddScoped<IFileDocumentRepository, FileDocumentRepository>();
builder.Services.AddScoped<IRequestsRepository, RequestsRepository>();
builder.Services.AddScoped<IInactiveFileRemoverService, InactiveFileRemoverService>();
builder.Services.AddHostedService<InactiveFilesWorker>();

var app = builder.Build();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.Migrate();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
