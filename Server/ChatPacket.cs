using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ChatPacket
    {
        public string Message { get; set; }
        public string Sender { get; set; }
        public DateTime Time { get; set; }

        public ChatPacket(string message, string sender = "Dave")
        {
            this.Message = message;
            this.Sender = sender;
            this.Time = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format($"[{Time}] {Sender} : {Message}");
        }

        public static byte[] CreatePacket(ChatPacket cp)
        {
            var json = JsonConvert.SerializeObject(cp);
            return Encoding.ASCII.GetBytes(json);
        }

        public static ChatPacket ParsePacket(byte[] data)
        {
            var json = Encoding.ASCII.GetString(data);
            return JsonConvert.DeserializeObject<ChatPacket>(json);
        }
    }
}
