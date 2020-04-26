namespace Monaco.Tools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    public class LogModel
    {
        private readonly List<MessageModel> _messages = new List<MessageModel>();
        private readonly List<LogStartModel> _starts = new List<LogStartModel>();
        private readonly List<LogErrorModel> _errors = new List<LogErrorModel>();

        public string Path { get; set; }

        public IReadOnlyCollection<LogStartModel> Starts => _starts;

        public IReadOnlyCollection<LogErrorModel> Errors => _errors;

        public IReadOnlyCollection<MessageModel> Messages => _messages;

        public LogModel AddStart(DateTime timestamp, string version, string framework)
        {
            _starts.Add(new LogStartModel
            {
                Timestamp = timestamp,
                Version = version,
                Framework = framework
            });

            return this;
        }

        public LogModel AddError(DateTime timestamp, string message)
        {
            _errors.Add(new LogErrorModel
            {
                Timestamp = timestamp,
                Message = message
            });

            return this;
        }

        public LogModel AddMessage(DateTime timestamp, int port, long sessionId, long errorCode, string className,
            string commandType, string commandName, string xml)
        {
            _messages.Add(new MessageModel
            {
                Timestamp = timestamp,
                Port = port,
                Session = sessionId,
                Error = errorCode,
                Class = className,
                CommandType = commandType,
                Command = commandName,
                Xml = WebUtility.HtmlEncode(xml),
            });

            return this;
        }
    }
}
