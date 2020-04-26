namespace Monaco.Tools.Areas.Emdi.Pages
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Services;
    using Tools.Models;

    public class ParseModel : PageModel
    {
        private readonly ILogParser _parser;

        public DateTime ParseDate { get; private set; }

        public IEnumerable<LogModel> Logs { get; private set; }

        public ParseModel(ILogParser parser)
        {
            _parser = parser;
        }

        public void OnGet()
        {
            ParseDate = DateTime.UtcNow;
            Logs = _parser.Parse("").Result;
        }
    }
}
