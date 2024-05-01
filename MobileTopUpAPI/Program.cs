using Application.Common.Interfaces.IRepositories;
using Application.Common.Mappings;
using Infrastructure.Data;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MobileTopUpAPI.Application.Common.Interfaces.IServices;
using MobileTopUpAPI.Infrastructure.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Confugure Inmemory database 
// Add DbContext using in-memory database
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseInMemoryDatabase(databaseName: "MobileAppTopup_Db"));

// Load the connection string from appsettings.json
//string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//// Configure the database context
//builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));
// Register DbContextOptions as a singleton
builder.Services.AddSingleton<DbContextOptions<ApplicationDbContext>>(provider =>
{
    // Configure the database connection here
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                          .UseSqlServer(connectionString);
    return optionsBuilder.Options;
});

// Register your DbContext as a scoped service
builder.Services.AddDbContext<ApplicationDbContext>();

//httpclient registor
//DI

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(typeof(MappingEntites));
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

//Service DI
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBeneficiaryService, BeneficiaryService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<ITopUpTransactionService, TopUpTransactionService>();
builder.Services.AddHttpClient<IExternalPaymentService, ExternalPaymentService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//JSON
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

//Health checks are a proactive mechanism for monitoring and verifying the health and availability 
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
ApplyMigration();
app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _Db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (_Db != null)
        {
            if (_Db.Database.GetPendingMigrations().Any())
            {
                _Db.Database.Migrate();
            }
        }
    }
}
