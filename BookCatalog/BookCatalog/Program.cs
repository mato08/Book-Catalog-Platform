using BookCatalog.Api.ExceptionHandling;
using BookCatalog.Application.Common.Behaviors;
using BookCatalog.Application.Common.Repositories;
using BookCatalog.Application.Features.Books.CreateBooks;
using BookCatalog.Infrastructure.Persistence;
using BookCatalog.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssemblyContaining<CreateBooksHandler>());
builder.Services.AddValidatorsFromAssemblyContaining<CreateBooksHandler>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("BookCatalog"));
builder.Services.AddScoped<IBooksRepository, BookRepository>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
