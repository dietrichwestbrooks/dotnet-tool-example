namespace Monaco.Tools.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Models;
    using Serialization;

    public class LogParser : ILogParser
    {
        private readonly ILogger<LogParser> _logger;

        public LogParser()
        {
            
        }

        public LogParser(ILogger<LogParser> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<LogModel>> Parse(string path)
        {
            var logs = new List<LogModel>();

            foreach (var file in Directory
                .EnumerateFiles(path, "*.log*",
                    SearchOption.TopDirectoryOnly)
                .Where(x => !x.Contains("Fatal"))
                .Select(x => new FileInfo(x))
                .OrderBy(x => x.LastWriteTime))
            {
                _logger?.LogDebug($"Parsing {file.FullName}...");

                var log = new LogModel();

                var content = File.ReadAllText(file.FullName);

                ParseStarts(log, content, logs.Any());

                ParseErrors(log, content);

                ParseMessages(log, content);

                logs.Add(log);

                // break;
            }

            // File.WriteAllText($"emdi-{reportDate:yyyymmddhhss}.json", Json.Stringify(_log));

            // var source = new StreamReader(GetResource<ReportGenerator>("templates.emdi.html")).ReadToEnd();

            return await Task.FromResult(logs);
        }

        private void ParseErrors(LogModel log, string content)
        {
            var re = new Regex(
                @"ERROR (?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2}) (?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2})\.(?<millisecond>\d{3}) \S+ - EMDI: (?<message>Error .*)");

            var match = re.Match(content);

            while (match.Success)
            {
                var year = int.Parse(match.Groups["year"].Value);
                var month = int.Parse(match.Groups["month"].Value);
                var day = int.Parse(match.Groups["day"].Value);
                var hour = int.Parse(match.Groups["hour"].Value);
                var minute = int.Parse(match.Groups["minute"].Value);
                var second = int.Parse(match.Groups["second"].Value);
                var millisecond = int.Parse(match.Groups["millisecond"].Value);
                var message = match.Groups["message"].Value;

                var timestamp = new DateTime(year, month, day, hour, minute, second, millisecond);

                // Console.WriteLine("Error: {0} {1}", timestamp, message);

                log.AddError(timestamp, message);

                match = match.NextMatch();
            }
        }

        private static void ParseMessages(LogModel log, string content)
        {
            var serializer = new MessageSerializer();

            var re = new Regex(
                @"DEBUG (?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2}) (?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2})\.(?<millisecond>\d{3}) EmdiHost - EMDI: Message (to|from) Content on port (?<port>\d{4}): (?<xml><md:mdMsg[\S\s]*?>[\s\S]*?<\/md:mdMsg>)");

            var match = re.Match(content);

            while (match.Success)
            {
                var year = int.Parse(match.Groups["year"].Value);
                var month = int.Parse(match.Groups["month"].Value);
                var day = int.Parse(match.Groups["day"].Value);
                var hour = int.Parse(match.Groups["hour"].Value);
                var minute = int.Parse(match.Groups["minute"].Value);
                var second = int.Parse(match.Groups["second"].Value);
                var millisecond = int.Parse(match.Groups["millisecond"].Value);
                var port = int.Parse(match.Groups["port"].Value);
                var xml = match.Groups["xml"].Value;

                var timestamp = new DateTime(year, month, day, hour, minute, second, millisecond);

                var message = serializer.Deserialize(xml);

                message.Timestamp = timestamp;
                message.Port = port;
                message.Xml = xml;

                // Console.WriteLine("{0} {1} {2} {3}", message.Timestamp, message.Port, message.Direction, message.Command);

                log.AddMessage(
                    message.Timestamp,
                    message.Port,
                    message.SessionId,
                    message.ErrorCode,
                    message.ClassName,
                    message.CommandType,
                    message.CommandName,
                    message.Xml);

                match = match.NextMatch();
            }
        }

        private void ParseStarts(LogModel log, string content, bool skipFirstStart)
        {
            var skippedFirst = false;

            var re = new Regex(
                @"Trace started - Version (?<version>\d{1,2}\.\d{1,2}\.\d{1,2}\.\d{1,2}), \.NET Framework (?<framework>\d.\d.\d+.\d+) (?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2}) (?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2})\.(?<millisecond>\d{3})");

            var match = re.Match(content);

            while (match.Success)
            {
                // Skip first Trace at the beginning of log file unless it is the first file processed 
                if (skipFirstStart && !skippedFirst)
                {
                    skippedFirst = true;
                    match = match.NextMatch();
                    continue;
                }

                var version = match.Groups["version"].Value;
                var framework = match.Groups["framework"].Value;
                var year = int.Parse(match.Groups["year"].Value);
                var month = int.Parse(match.Groups["month"].Value);
                var day = int.Parse(match.Groups["day"].Value);
                var hour = int.Parse(match.Groups["hour"].Value);
                var minute = int.Parse(match.Groups["minute"].Value);
                var second = int.Parse(match.Groups["second"].Value);
                var millisecond = int.Parse(match.Groups["millisecond"].Value);

                var timestamp = new DateTime(year, month, day, hour, minute, second, millisecond);

                // Console.WriteLine("Start {0}", timestamp);

                log.AddStart(timestamp, version, framework);

                match = match.NextMatch();
            }
        }
    }
}
