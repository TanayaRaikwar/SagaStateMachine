using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using SagaService.Models;
using SagaStateMachine;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Register SagaContext
builder.Services.AddControllers();

var connectionstring= builder.Configuration.GetConnectionString("DbConnection");

builder.Services.AddDbContext<AppDbContext>(options=>
    options.UseMySql(connectionstring,ServerVersion.AutoDetect(connectionstring))
);

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddBus(provider=> MessageBrokers.RabbitMQ.ConfigureBus(provider));
    cfg.AddSagaStateMachine<TicketStateMachine, TicketStateData>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion

            r.ExistingDbContext<AppDbContext>();
        });
});


var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
