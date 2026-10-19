using Calendarium.Data;
using Calendarium.Models;
using Microsoft.EntityFrameworkCore;

namespace Calendarium.Services
{
    /// <summary>Records visitor request headers (deduplicated per hour), shown on the History page.</summary>
    public class RequestHeaderLogger(ApplicationDbContext context)
    {
        public async Task LogAsync(IHeaderDictionary headers, CancellationToken ct = default)
        {
            await LogHeaderAsync(new RequestHeaderField(ReqHeadFieldType.Accept, headers.Accept.ToString()), ct);
            // await LogHeaderAsync(new RequestHeaderField(ReqHeadFieldType.Encode, headers.AcceptEncoding.ToString()), ct);
            // await LogHeaderAsync(new RequestHeaderField(ReqHeadFieldType.Language, headers.AcceptLanguage.ToString()), ct);
            // await LogHeaderAsync(new RequestHeaderField(ReqHeadFieldType.Referer, headers.Referer.ToString()), ct);
            await LogHeaderAsync(new RequestHeaderField(ReqHeadFieldType.UsrAgent, headers.UserAgent.ToString()), ct);
        }

        private async Task LogHeaderAsync(RequestHeaderField reqHead, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(reqHead.Text)) return;

            var hourAgo = reqHead.Created.AddHours(-1);
            bool seenRecently = await context.RequestsHeaders.AnyAsync(old =>
                old.Field == reqHead.Field && old.Text == reqHead.Text && old.Created > hourAgo, ct);

            if (!seenRecently)
            {
                context.RequestsHeaders.Add(reqHead);
                await context.SaveChangesAsync(ct);
            }
        }
    }
}
