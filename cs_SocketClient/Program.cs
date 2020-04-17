using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace cs_SocketClient {
    class Program {
        static void Main(string[] args) {
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
}

// RDP