using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace cs_SocketServer {
    class Program {
        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();
        static readonly List<Socket> Clients = new List<Socket>();
        static async Task MainAsync(string[] args) {
            var listener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );



            var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 44001);
            listener.Bind(ep);

            listener.Listen(100);
            Console.WriteLine("Listening...");
            while (true) {
                var client = listener.Accept(); // client connected
                Clients.Add(client);
                Task.Run(() => {
                    var cEp = (client.RemoteEndPoint as IPEndPoint);
                    Console.WriteLine($"Client connected: {cEp.Address}:{cEp.Port}\n\n");

                    var buffer = new byte[10240];

                    while (true) {
                        try {
                            var len = client.Receive(buffer);
                            if (len == 0) {
                                Console.WriteLine($"{cEp.Port} disconnected");
                                client.Close();
                                break;
                            }

                            var text = Encoding.Default.GetString(buffer, 0, len);

                            if (text == "die") {
                                listener.Close();
                                return;
                            }

                            if (text == "bye") {
                                Console.WriteLine($"{cEp.Port} disconnected");
                                client.Close();
                                break;
                            }

                            var msg = $"{cEp.Port}: {text}";
                            foreach (var c in Clients) {
                                if (c != client) {
                                    var bytes = Encoding.Default.GetBytes(msg);
                                    c.Send(bytes);
                                }
                            }
                            Console.WriteLine(msg);
                        } catch (Exception ex) {
                            Console.WriteLine($"{cEp.Port} disconnected");
                            client.Close();
                            break;
                        }
                    }
                });
            }
        }
    }
}
