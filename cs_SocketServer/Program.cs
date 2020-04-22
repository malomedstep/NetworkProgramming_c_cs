using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Jil;

namespace cs_SocketServer {
    class Program_TCP {
        static readonly ConcurrentBag<Socket> Clients = new ConcurrentBag<Socket>();

        static void Main1(string[] args) {
            var listener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            var ep = new IPEndPoint(IPAddress.Any, 44001);
            listener.Bind(ep);

            listener.Listen(100);

            Console.WriteLine("Listening...");

            while (true) {
                listener.AcceptAsync().ContinueWith(task => {
                    var client = task.Result;

                    Clients.Add(client);

                    var cEp = (client.RemoteEndPoint as IPEndPoint);

                    Console.WriteLine($"Client connected: {cEp.Address}:{cEp.Port}\n\n");

                    var buffer = new byte[10240];
                    var segment = new ArraySegment<byte>(buffer);


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


    class Program_UDP {
        public static void Main2(string[] args) => MainAsync(args).GetAwaiter().GetResult();


        private static async Task MainAsync(string[] args) {
            var listener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp
            );

            var listenerEndPoint = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 45678);
            listener.Bind(listenerEndPoint);

            // accept   => client_socket
            // receive  => data
            var buffer = new byte[ushort.MaxValue];
            EndPoint ep = new IPEndPoint(IPAddress.Any, 0); //192.168.0.1:12312
            var segment = new ArraySegment<byte>(buffer);
            while (true) {
                int len = listener.ReceiveFrom(buffer, ref ep);
                var str = Encoding.Default.GetString(buffer, 0, len);
                Console.WriteLine($"{ep}: {str}");

            }
        }
    }

    class Program_TCP_Listener {
        static void Main5(string[] args) {
            var ip = IPAddress.Parse("127.0.0.1");
            var listener = new TcpListener(ip, 45678);
            listener.Start(100);

            // client.GetType() == typeof(TcpClient)
            var client = listener.AcceptTcpClient();

            var stream = client.GetStream();
            var sr = new StreamReader(stream);

            while (true) {
                var data = sr.ReadLine();
                Console.WriteLine(data);
            }
        }
    }

    public class Command {
        public const string ProcList = "PROCLIST";
        public const string Kill = "KILL";
        public const string Run = "RUN";

        public string Text { get; set; }
        public string Param { get; set; }
    }

    class Program_TCP_Listener_Proto {
        static void Main4(string[] args) {
            var ip = IPAddress.Parse("127.0.0.1");
            var listener = new TcpListener(ip, 45678);
            listener.Start(100);


            while (true) {
                var client = listener.AcceptTcpClient();

                var stream = client.GetStream();
                var br = new BinaryReader(stream);
                var bw = new BinaryWriter(stream);
                while (true) {
                    var input = br.ReadString();
                    var command = JSON.Deserialize<Command>(input);
                    if (command == null)
                        continue;

                    Console.WriteLine(command.Text);
                    Console.WriteLine(command.Param);

                    switch (command.Text) {
                        case Command.ProcList:
                            var processes = Process.GetProcesses();
                            bw.Write(JSON.Serialize(processes.Select(p => p.ProcessName)));
                            break;

                        case Command.Kill:
                            if (command.Param == null) continue;
                            var procs = Process.GetProcessesByName(command.Param);
                            foreach (var process in procs) {
                                try {
                                    process.Kill();
                                } catch { }
                            }
                            bw.Write(JSON.Serialize(true));
                            break;
                        case Command.Run:
                            if (command.Param == null) continue;
                            Process.Start(command.Param);
                            bw.Write(JSON.Serialize(true));
                            break;
                    }
                }
            }
        }
    }


    class Program_UDP_Client {
        static void Main(string[] args) {
            var client = new UdpClient(45678);
            var ep = new IPEndPoint(IPAddress.Any, 0);
            while (true) {
                var bytes = client.Receive(ref ep);
                var str = Encoding.Default.GetString(bytes);
                Console.WriteLine(str);
            }
        }
    }
}
