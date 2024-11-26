
using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Notifications;
using WebShopSolution.DataAccess.Repositories;
using WebShopSolution.DataAccess.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

var connectionsString = builder.Configuration.GetConnectionString("MyDbContext");

builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(connectionsString);
});


// Add services to the container.

builder.Services.AddControllers();
// Registrera Unit of Work i DI-container

builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ISubject<Product>,ProductSubject>();

builder.Services.AddTransient<INotificationObserver<Product>, EmailNotification >();
builder.Services.AddTransient<INotificationObserver<Product>, SmsNotification>();
builder.Services.AddTransient<INotificationObserver<Product>, PushNotification>();




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



//Apply migrations från Websolution 
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    dbContext.Database.Migrate();
}

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
