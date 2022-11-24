using SuperSocket;
using SuperSocket.ProtoBase;
using System.Text;
using AspNetCoreSocketServer.BlazorServer.ViewModel;
using AspNetCoreSocketServer.SocketService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<ISessionManager,SessionManager>();
builder.Services.AddSingleton<SessionView>();

builder.Host.AsSuperSocketHostBuilder<TextPackageInfo>()
    .UsePipelineFilter<LinePipelineFilter>()
    .UsePackageHandler(async (s, p) =>
    {
        // echo message back to client
        await s.SendAsync(Encoding.UTF8.GetBytes(p.Text + "\r\n"));
    })
    .UseSession<SocketServerAppSession>()
    .UseInProcSessionContainer()
    .AsMinimalApiHostBuilder()
    .ConfigureHostBuilder();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
   // app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();