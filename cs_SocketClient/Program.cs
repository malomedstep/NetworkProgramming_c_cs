﻿using System;
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
        public static void Main2(string[] args) {
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


    class Program_TCP_Client {
        static void Main3(string[] args) {
            var client = new TcpClient();
            client.Connect("127.0.0.1", 45678);

            var stream = client.GetStream();
            var bw = new BinaryWriter(stream);
            var br = new BinaryReader(stream);
            while (true) {
                var str = Console.ReadLine();
                bw.Write(str);
                var answer = br.ReadString();
                Console.WriteLine($"Server: {answer}");
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

    class Program_TCP_Client__Proto {
        static void Main(string[] args) {
            var client = new TcpClient();
            client.Connect("127.0.0.1", 45678);


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
        }
    }
}

