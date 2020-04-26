// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

namespace Aristocrat.G2S.Emdi.Protocol.v21ext1b1
{
    using System;
    using System.Globalization;

    public partial class mdMsg
    {
        public DateTime Timestamp { get; set; }

        public int Port { get; set; }

        public string Xml { get; set; }

        public long SessionId => Class.sessionId;

        public long ErrorCode => Class.errorCode;

        public string CommandType => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Class.cmdType.ToString());

        public dynamic Class => Item;

        public string ClassName => Class.GetType().Name;

        public dynamic Command => Class.Item;

        public string CommandName => Command?.GetType().Name;

        public bool IsHeartbeat => Item is mdComms fg && fg.Item is heartbeat;

        public bool IsCommsOnline => Item is mdComms fg && fg.Item is commsOnLine;

        public override string ToString()
        {
            return $"{Timestamp} {CommandType} {SessionId} {ErrorCode} {Port} {Command}";
        }
    }
}
