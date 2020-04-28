using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;

namespace sc_Smtp {
    class Program {
        static void Smtp() {
            var client = new SmtpClient("smtp.gmail.com", 587);
            var message = new MailMessage {
                Body = "<p>Shalom, we provide you Azure subscription for free for new 100 years!</p> <p>Congrats!</p><p>Regards,<br>Bill Gates</p>",
                Subject = "Free Azure Subscription"
            };
            client.EnableSsl = true;
            message.To.Add(new MailAddress("oleksandr.malomed@outlook.com"));
            message.From = new MailAddress("johndoestep42@gmail.com", "Microsoft");
            client.Credentials = new NetworkCredential(
                "johndoestep42@gmail.com",
                "Password1234!"
            );
            client.Send(message);

        }

        static void Main(string[] args) {
            var imapClient = new ImapClient();
            imapClient.Connect("imap.gmail.com", 993, true);
            imapClient.Authenticate("johndoestep42@gmail.com", "Password1234!");
            var inbox = imapClient.GetFolder("Inbox");
            inbox.Open(FolderAccess.ReadWrite);
            inbox.SetFlags(new List<int> { 0, 1 }, MessageFlags.Deleted, true);
            inbox.Expunge();

            


            // SMTP
            // POP3
            // IMAP
            // SmtpClient
            // mailkit
        }
    }
}
