using WebSocketSharp.Server;
using WebSocketSharp;
using System.Xml.Linq;
using Hammer.Iiko.WsServer.Rpc;
using Hammer.Iiko.WsServer.Dto;
using System.Text;

namespace Hammer.Iiko.WsServer
{
    internal class Program
    {
        //public class Laputa : WebSocketBehavior
        //{
        //    protected override void OnMessage(MessageEventArgs e)
        //    {
        //        var msg = e.Data == "BALUS"
        //                  ? "Are you kidding?"
        //                  : "I'm not available now.";
        //        Send(msg);
        //    }
        //}

        public class Input : WebSocketBehavior
        {
            private string _key;

            public Dictionary<string, string> Clients { get; set; } = new();

            public Input()
            {
                _key = string.Empty;
            }

            protected override void OnMessage(MessageEventArgs e)
            {
                Console.WriteLine($"Input: {(e.IsText ? e.Data : Encoding.UTF8.GetString(e.RawData))}");
            }

            protected override void OnOpen()
            {
                _key = Context.QueryString["key"] ?? "NO_KEY";
                Console.WriteLine($"Socket Open: key: {_key}");

                Clients.Add(_key, ID);
            }

            protected override void OnClose(CloseEventArgs e)
            {
                Console.WriteLine($"Socket Close. Code: {e.Code}, Reason: {e.Reason}");
            }

            protected override void OnError(WebSocketSharp.ErrorEventArgs e)
            {
                Console.WriteLine($"Socket Error. Msg: {e.Message}, Ex: {e.Exception}");
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Start WebSocket Server.");

            Dictionary<string, string> clients = new();

            var wssv = new WebSocketServer("ws://localhost");

            //wssv.AddWebSocketService<Laputa>("/Laputa");
            wssv.AddWebSocketService<Input>("/", s => s.Clients = clients);
            var sessions = wssv.WebSocketServices["/"].Sessions;

            wssv.Start();

            Console.WriteLine("Press key for send to KEY_API ...");
            Console.ReadKey();

            if (clients.TryGetValue("KEY_API", out string? id))
            {
                sessions.SendTo("Hello KEY_API!", id);

                var rpcJson = new RpcRequestConstructor<TableStatusInfo>("GetTables", new());
                sessions.SendTo(rpcJson.ResultRequest, id);
            }

            Console.ReadKey(true);
            sessions.Broadcast("Bye everyone!");
            wssv.Stop();
        }
    }
}