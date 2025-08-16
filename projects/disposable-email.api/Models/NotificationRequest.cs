using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MimeKit;

namespace disposable_email.api.Models
{
    public class NotificationRequest
    {
        [JsonPropertyName("envelope")]
        public Envelope Envelope { get; set; }
        [JsonPropertyName("headers")]
        public Headers Headers { get; set; }
        [JsonPropertyName("raw_b64")]
        public string RawBase64 { get; set; }

        public async Task<ParsedEmail> Decode()
        {
            byte[] raw = Convert.FromBase64String(RawBase64);

            using (var ms = new MemoryStream(raw, writable: false))
            {
                var message = await MimeMessage.LoadAsync(ms);

                var dto = new ParsedEmail
                {
                    Subject = message.Subject,
                    From = message.From?.ToString(),
                    To = message.To?.Select(a => a.ToString()).ToList() ?? new(),
                    Cc = message.Cc?.Select(a => a.ToString()).ToList() ?? new(),
                    Date = message.Date != DateTimeOffset.MinValue ? message.Date : null,

                    ContentType = message.Body?.ContentType?.MimeType,
                    Charset = message.Body?.ContentType?.Charset,
                    TransferEncoding = (message.Body as MimePart)?.ContentTransferEncoding.ToString(),

                    TextBody = message.TextBody,
                    HtmlBody = message.HtmlBody,

                    Received = message.Headers
                        .Where(h => h.Field.Equals("Received", StringComparison.OrdinalIgnoreCase))
                        .Select(h => h.Value)
                        .ToList(),

                    Headers = message.Headers
                        .GroupBy(h => h.Field, StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(h => h.Value).ToList(),
                            StringComparer.OrdinalIgnoreCase
                        )
                };

                return dto;
            }
        }
    }

    public class Envelope
    {
        [JsonPropertyName("mail_from")]
        public string MailFrom { get; set; }
        [JsonPropertyName("rcpt_to")]
        public string[] MailTo { get; set; }
    }

    public class Headers
    {
        [JsonPropertyName("received")]
        public string[] Received { get; set; }
        [JsonPropertyName("mime-version")]
        public string[] MimeVersion { get; set; }
        [JsonPropertyName("from")]
        public string[] From { get; set; }
        [JsonPropertyName("to")]
        public string[] To { get; set; }
        [JsonPropertyName("cc")]
        public string[] Cc { get; set; }
        [JsonPropertyName("date")]
        public string[] Date { get; set; }
        [JsonPropertyName("subject")]
        public string[] Subject { get; set; }
        [JsonPropertyName("content-type")]
        public string[] ContentType { get; set; }
        [JsonPropertyName("content-transfer-encoding")]
        public string[] ContentTransferEncoding { get; set; }
    }
}
