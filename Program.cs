
using future_chat_server.Models;
using MongoDB.Driver.Core.Connections;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace future_chat_server
{
    public class Program
    {
        private static List<TcpClient> activeConnections = new List<TcpClient>();
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<ChatConfig>(
                builder.Configuration.GetSection("ChatMain")
            );
            builder.Services.AddSingleton<Connector>(provider =>
                {
                    var configuration = provider.GetRequiredService<IConfiguration>();
                    var connectionString = configuration["ChatMain:conString"];
                    var databaseName = configuration["ChatMain:chatDatabase"];
                    return new Connector(connectionString, databaseName);
                });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            //prepara thread para api
            var apiThread = new Thread(() =>
            {
                app.Run();
            });
            apiThread.Start();

            //inicia servidor tcp
            TcpServer();

            //inicia api
            apiThread.Join();
        }
        private static void TcpServer()
        {
            int porta = 12346; //porta default

            TcpListener tcpListener = new TcpListener(IPAddress.Any, porta);
            tcpListener.Start();

            Console.WriteLine($"Servidor TCP iniciado na porta {porta}...");

            while (true)
            {
                try
                {
                    TcpClient client = tcpListener.AcceptTcpClient();

                    // Inicie uma nova thread para lidar com a conexão do cliente
                    Thread clientThread = new Thread(() => HandleTcpConnection(client));
                    clientThread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao aceitar conexão TCP: {ex.Message}");
                }
            }
        }

        private static void HandleTcpConnection(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.ASCII);
            try
            {
                // Adicione a conexão do cliente à lista de conexões ativas
                lock (activeConnections)
                {
                    activeConnections.Add(client);
                }

                Console.WriteLine($"Cliente conectado: {((IPEndPoint)client.Client.RemoteEndPoint).Address}:{((IPEndPoint)client.Client.RemoteEndPoint).Port}");

                string clientMessage;
                while ((clientMessage = reader.ReadLine()) != null)
                {
                    Console.WriteLine($"Mensagem do cliente: {clientMessage}");

                    // Envie a mensagem para todos os outros clientes
                    BroadcastMessage(clientMessage, client);

                    if (clientMessage.ToLower() == "exit")
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na conexão TCP: {ex.Message}");
            }
            finally
            {
                // Remova a conexão do cliente da lista de conexões ativas quando ele desconectar
                lock (activeConnections)
                {
                    activeConnections.Remove(client);
                }

                Console.WriteLine($"Cliente desconectado: {((IPEndPoint)client.Client.RemoteEndPoint).Address}:{((IPEndPoint)client.Client.RemoteEndPoint).Port}");

                client.Close();
            }
        }

        private static void BroadcastMessage(string message, TcpClient sender)
        {
            lock (activeConnections)
            {
                foreach (TcpClient client in activeConnections)
                {
                    if (client != sender) // Não envie a mensagem de volta para o remetente original
                    {
                        try
                        {
                            NetworkStream stream = client.GetStream();
                            StreamWriter writer = new StreamWriter(stream, Encoding.ASCII);

                            // Envie a mensagem para o cliente atual
                            writer.WriteLine(message);
                            writer.Flush();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro ao enviar mensagem para um cliente: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}