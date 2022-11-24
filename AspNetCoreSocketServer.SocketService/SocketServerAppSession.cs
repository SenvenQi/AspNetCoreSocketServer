using System.Net;
using Microsoft.AspNetCore.Components;
using SuperSocket.Channel;
using SuperSocket.Server;

namespace AspNetCoreSocketServer.SocketService;

public class SocketServerAppSession:AppSession
{
    private readonly ISessionManager _sessionManager;
    public SocketServerAppSession(ISessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }
    protected override ValueTask OnSessionConnectedAsync()
    {
        this._sessionManager.Add(this.RemoteEndPoint);
        return base.OnSessionConnectedAsync();
    }

    protected override ValueTask OnSessionClosedAsync(CloseEventArgs e)
    {
        _sessionManager.Remove(this.RemoteEndPoint);
        return base.OnSessionClosedAsync(e);
    }
}

public interface ISessionManager
{
    Action OnChange { get; set; }
    List<SessionData> Sessions { get; set; }
    Task Add(EndPoint remoteEndPoint);
    void Remove(EndPoint remoteEndPoint);
}

public class SessionManager : ISessionManager
{
    public Action OnChange { get; set; }
    public List<SessionData> Sessions { get; set; } = new List<SessionData>(); 
    public async Task Add(EndPoint remoteEndPoint)
    {
        using var httpclient = new HttpClient();
        var result = await httpclient.GetAsync($"http://ip-api.com/json/{remoteEndPoint.ToString()?.Split(":")[0]}");
        Sessions.Add(new SessionData()
        {
            Id = Guid.NewGuid(),
            Ip = remoteEndPoint,
            Address = await result.Content.ReadAsStringAsync() 
        });
        Dispatcher.CreateDefault().InvokeAsync(OnChange);
    }

    public void Remove(EndPoint remoteEndPoint)
    {
        Sessions.Remove(Sessions.First(x => x.Ip.Equals(remoteEndPoint)));
        Dispatcher.CreateDefault().InvokeAsync(OnChange);
    }
}

public class SessionData
{
    public Guid Id { get; set; }
    public EndPoint Ip{ get; set; }
    public string Address { get; set; }
}