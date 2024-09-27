using Domain.MappingProfiles;
using Domain.Services;
using Domain.Services.Schedulers;
using Infrastructure.Configuration;
using Infrastructure.Interfaces;
using Infrastructure.Middleware;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using ProjectBackend.MappingProfiles;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddAutoMapper(typeof(BackendMappingProfiles));

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.ConnectionString.json", optional: false, reloadOnChange: true);

var connectionString = builder.Configuration.GetConnectionString("LeavePlanner");

builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(connectionString);
});


builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IPersonalEventRepository, PersonalEventRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<PersonalEventService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<EventService>();

builder.Services.AddHostedService<AnniversaryScheduler>();


builder.Services.AddControllers();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.json", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 1_000_000, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {ThreadId} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(c => c
.SetIsOriginAllowed(origin => true)
.AllowAnyHeader()
.AllowAnyMethod());
app.UseHttpsRedirection();

app.UseMiddleware<CustomExceptionHandler>();

app.UseAuthorization();

app.MapControllers();

app.Run();
