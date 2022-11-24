using System.Reactive;
using System.Reactive.Linq;
using AspNetCoreSocketServer.SocketService;
using ReactiveUI;

namespace AspNetCoreSocketServer.BlazorServer.ViewModel;

public class SessionView:ReactiveObject
{
   private readonly ObservableAsPropertyHelper<List<SessionData>> _sessionDatas;
   private readonly ISessionManager _sessionManager;

   public SessionView(ISessionManager sessionManager)
   {
      _sessionManager = sessionManager;
      _sessionManager.OnChange += async () =>
      {
         await this.LoadSessions.Execute();
         Console.WriteLine(_sessionDatas.Value.Count);
      };
      LoadSessions = ReactiveCommand.CreateFromTask(LoadSessionsAsync);
      _sessionDatas= LoadSessions.ToProperty(this, x => x.SessionDatas);
   }
   
   public ReactiveCommand<Unit, List<SessionData>> LoadSessions { get; }
   public List<SessionData> SessionDatas => _sessionDatas.Value;

   private async Task<List<SessionData>> LoadSessionsAsync()
   {
      return await Task.FromResult(_sessionManager.Sessions.ToList());
   }
}