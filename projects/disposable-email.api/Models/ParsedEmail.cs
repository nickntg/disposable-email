using System;
using System.Collections.Generic;

namespace disposable_email.api.Models
{
    public sealed class ParsedEmail
    {
        public string Subject { get; init; }
        public string From { get; init; }
        public List<string> To { get; init; } = new();
        public List<string> Cc { get; init; } = new();
        public DateTimeOffset? Date { get; init; }
        public List<string> Received { get; init; } = new();
        public string ContentType { get; init; }
        public string Charset { get; init; }
        public string TransferEncoding { get; init; }
        public string TextBody { get; init; }
        public string HtmlBody { get; init; }
        public Dictionary<string, List<string>> Headers { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    }
}