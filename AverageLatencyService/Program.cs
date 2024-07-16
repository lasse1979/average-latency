using AverageLatencyService.Integration;
using AverageLatencyService.Services;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddScoped<LatencyService>();
//builder.Services.AddScoped<ILatencyClient, LatencyClient>();
builder.Services.AddHttpClient<ILatencyClient, LatencyClient>(
    client =>
    {
        client.BaseAddress = new Uri("http://latencyapi-env.eba-kqb2ph3i.eu-west-1.elasticbeanstalk.com/");
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();

