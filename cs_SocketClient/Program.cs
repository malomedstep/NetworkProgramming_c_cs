using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Jil;

namespace cs_SocketClient {
    class Program_TCP_HTTP {
        static void Main(string[] args) {
            var client = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            var ep = new IPEndPoint(IPAddress.Loopback, 45678);
            Console.WriteLine("Connecting...");
            client.Connect(ep); // server accepted connection
            Console.WriteLine("Connected!");
            var buffer = new byte[10240];


            var text = "GET / HTTP/1.1\nHost: localhost\n\n";

            var bytes = Encoding.Default.GetBytes(text);

            client.Send(bytes);
            int len = client.Receive(buffer);
            var str = Encoding.Default.GetString(buffer, 0, len);
            Console.WriteLine(str);

            client.Close();
        }
    }




    class Program_TCP {
        static void Main1(string[] args) {
            var client = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 44001);
            Console.WriteLine("Connecting...");
            client.Connect(ep); // server accepted connection
            Console.WriteLine("Connected!");
            var buffer = new byte[10240];
            Task.Run(() => {
                while (true) {
                    var len = client.Receive(buffer);
                    var text = Encoding.Default.GetString(buffer, 0, len);
                    Console.WriteLine(text);
                }
            });
            while (true) {
                var text = Console.ReadLine();
                if (text == "exit" || text == "bye") {
                    client.Send(Encoding.Default.GetBytes("bye"));
                    client.Disconnect(false);
                    break;
                }

                var bytes = Encoding.Default.GetBytes(text);

                client.Send(bytes);
            }

            client.Close();
        }
    }

    class Program_UDP {
        public static void Main32342(string[] args) {
            var client = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp
            );
            var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 45678);
            while (true) {
                var str = Console.ReadLine();
                var bytes = Encoding.Default.GetBytes(str);
                client.SendTo(bytes, ep);
            }
        }
    }
    // Physical
    // Data-link
    // Network
    // Transport
    // Session
    // Presentation
    // Application

    class Program_TCP_Client {
        static void Main123523(string[] args) {
            var client = new TcpClient();
            client.Connect("127.0.0.1", 45678);

            var stream = client.GetStream();
            var sw = new StreamWriter(stream);

            int i = 0;
            while (i < 5) {
                //var str = Console.ReadLine();
                var str = "GET / HTTP/1.1\nHost: localhost\nPort: 45678\n\n";
                sw.Write(str);
                i++;
            }
            // sw.Write('\n');
            // sw.WriteLine();
            sw.Flush();
        }
    }


    public class Command {
        public const string ProcList = "PROCLIST";
        public const string Kill = "KILL";
        public const string Run = "RUN";

        public string Text { get; set; }
        public string Param { get; set; }
    }

    class Program_TCP_Client__Proto {
        static void Main4(string[] args) {
            var client = new TcpClient();
            //client.Connect("127.0.0.1", 45678);
            client.BeginConnect("127.0.0.1", 45678, ar => {
                var stream = client.GetStream();
                var br = new BinaryReader(stream);
                var bw = new BinaryWriter(stream);

                while (true) {

                    var str = Console.ReadLine();
                    if (str.ToUpper() == "HELP") {
                        Console.WriteLine("PROCLIST");
                        Console.WriteLine("KILL <process_name>");
                        Console.WriteLine("RUN <process_name>");
                        Console.WriteLine("HELP");
                        continue;
                    }

                    Command cmd = null;
                    var input = str.Split(' ');
                    string responce = null;
                    switch (input[0]) {
                        case Command.ProcList:
                            cmd = new Command {
                                Text = Command.ProcList
                            };
                            bw.Write(JSON.Serialize(cmd));
                            responce = br.ReadString();
                            var processes = JSON.Deserialize<string[]>(responce);
                            foreach (var process in processes) {
                                Console.WriteLine(process);
                            }
                            break;

                        case Command.Kill:
                            if (input.Length == 1) continue;
                            cmd = new Command {
                                Text = Command.Kill,
                                Param = input[1]
                            };
                            bw.Write(JSON.Serialize(cmd));
                            responce = br.ReadString();
                            bool success = JSON.Deserialize<bool>(responce);
                            Console.WriteLine(success ? "Killed" : "Error");
                            break;

                        case Command.Run:
                            if (input.Length == 1) continue;
                            cmd = new Command {
                                Text = Command.Run,
                                Param = input[1]
                            };
                            bw.Write(JSON.Serialize(cmd));
                            responce = br.ReadString();
                            bool succes = JSON.Deserialize<bool>(responce);
                            Console.WriteLine(succes ? "Started" : "Error");
                            break;
                    }
                }
            }, null);


        }
    }

    class Program_UDP_Client {
        static void Main99(string[] args) {
            var client = new UdpClient();
            var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 45678);
            while (true) {
                var str = Console.ReadLine();
                var bytes = Encoding.Default.GetBytes(str);
                client.Send(bytes, bytes.Length, ep);
            }
        }
    }

    class Program_UDP_Multicast {
        static void Main123(string[] args) {
            var udpClient = new UdpClient();
            var ip = IPAddress.Parse("224.5.6.7");
            udpClient.JoinMulticastGroup(ip);
            var ep = new IPEndPoint(ip, 45678);

            while (true) {
                var str = Console.ReadLine();
                var bytes = Encoding.Default.GetBytes(str);
                udpClient.Send(bytes, bytes.Length, ep);
            }
        }
    }

    class Program_UDP_BroadCast {
        static void Main123123(string[] args) {
            Console.WriteLine(IPAddress.Broadcast);
            var udpListener = new UdpClient(45679);
            var listenerIp = IPAddress.Parse("255.255.255.255");

            var listenerEp = new IPEndPoint(listenerIp, 0);

            Task.Run(() => {
                while (true) {
                    var bytes = udpListener.Receive(ref listenerEp);
                    var str = Encoding.Default.GetString(bytes);
                    Console.WriteLine(str);
                }
            });

            var udpClient = new UdpClient();
            var ip = IPAddress.Parse("255.255.255.255");
            var ep = new IPEndPoint(ip, 45678);

            while (true) {
                var str = Console.ReadLine();
                var bytes = Encoding.Default.GetBytes(str);
                udpClient.Send(bytes, bytes.Length, ep);
            }
        }
    }
}

