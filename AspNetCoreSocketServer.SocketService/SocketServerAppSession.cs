using System.Net;
using Microsoft.AspNetCore.Components;
using SuperSocket;
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
    void Add(EndPoint remoteEndPoint);
    void Remove(EndPoint remoteEndPoint);
}

public class SessionManager : ISessionManager
{
    public Action OnChange { get; set; }
    public List<SessionData> Sessions { get; set; } = new List<SessionData>(); 
    public void Add(EndPoint remoteEndPoint)
    {
        Sessions.Add(new SessionData()
        {
            Address = remoteEndPoint
        });
        Dispatcher.CreateDefault().InvokeAsync(OnChange);
    }

    public void Remove(EndPoint remoteEndPoint)
    {
        Sessions.Remove(Sessions.First(x => x.Address.Equals(remoteEndPoint)));
        Dispatcher.CreateDefault().InvokeAsync(OnChange);
    }
}

public class SessionData
{
    public Guid Id { get; set; }
    public EndPoint Address { get; set; }
    public IEnumerable<byte[]> Type { get; set; }
}