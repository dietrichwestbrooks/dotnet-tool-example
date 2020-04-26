namespace Monaco.Tools.Models
{
    using System;

    public class MessageModel
    {
        public DateTime Timestamp { get; set; }

        public int Port { get; set; }

        public long Session { get; set; }

        public long Error { get; set; }

        public string CommandType { get; set; }

        public string Class { get; set; }

        public string Command { get; set; }

        public string Xml { get; set; }
    }
}
