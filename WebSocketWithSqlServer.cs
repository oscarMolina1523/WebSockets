using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketSqlServer
{
    public class WebSocketWithSqlServer
    {
    }

    public class WebSocketServer
        {
            private const int BufferSize = 1024;

            //aca deberia de injectar el contexto de la base de datos
            private const string ConnectionString = "Data Source=your_server;Initial Catalog=your_database;User ID=your_username;Password=your_password";

            public async Task Start()
            {
                var listener = new HttpListener();
                listener.Prefixes.Add("http://localhost:8080/");
                listener.Start();
                Console.WriteLine("WebSocket server started.");

                while (true)
                {
                    var context = await listener.GetContextAsync();
                    if (context.Request.IsWebSocketRequest)
                    {
                        await ProcessWebSocketRequest(context);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
            }

            private async Task ProcessWebSocketRequest(HttpListenerContext context)
            {
                var webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
                var webSocket = webSocketContext.WebSocket;

                var buffer = new byte[BufferSize];
                var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!receiveResult.CloseStatus.HasValue)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                    Console.WriteLine($"Received message: {message}");

                    // Insert the message into the SQL Server database
                    InsertMessageIntoDatabase(message);

                    receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }

                await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
            }

            private void InsertMessageIntoDatabase(string message)
            {
                //esto es como una muestra de lo que es el repository aca esto no debe de ir 
                //en su lugar solo se injecta el repository qye es en donde esta los insert
                //update select y delete
            }
        }

        public class Program
        {
            public static async Task Main(string[] args)
            {
                var server = new WebSocketServer();
                await server.Start();
            }
        }

}
