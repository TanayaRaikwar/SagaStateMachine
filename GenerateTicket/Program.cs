using GenerateTicket.Consumers;
using GenerateTicket.Models;
using GenerateTicket.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register MassTransit 
builder.Services.AddMassTransit(cfg =>
{
    cfg.AddBus(provider => MessageBrokers.RabbitMQ.ConfigureBus(provider));
    cfg.AddConsumer<GenerateTicketConsumer>();
    cfg.AddConsumer<CancelSendingEmailConsumer>();
});

// Connection string
var connectionstring= builder.Configuration.GetConnectionString("DbConnection");

builder.Services.AddDbContext<AppDbContext>(options=>
    options.UseMySql(connectionstring,ServerVersion.AutoDetect(connectionstring))
);


// Register TicketInfo service
builder.Services.AddScoped<ITicketInfoService, TicketInfoService>();

// Register AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
