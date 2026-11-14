using GrpcPort.Server.Actions;
using GrpcPort.Server.Services;
using GrpcPort.Server.Sessions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5081, listen =>
    {
        listen.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.MaxReceiveMessageSize = 16 * 1024 * 1024;
});

builder.Services.AddSingleton<CertificateService>();
builder.Services.AddSingleton<SessionManager>();
builder.Services.AddSingleton<ActionsContext>();
builder.Services.AddSingleton<ActionBroker>();

var app = builder.Build();

app.MapGrpcService<PortServiceImpl>();
app.MapGet("/", () =>
    "GrpcPort.Server: http://localhost:5081 | Auth: secret-{client_id} | Actions: publish/request/response");

app.Run();
