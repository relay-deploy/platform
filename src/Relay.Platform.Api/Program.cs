using Relay.Chassis.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.AddRelayServiceDefaults();

var app = builder.Build();
app.MapRelayServiceDefaults();

await app.RunAsync();
