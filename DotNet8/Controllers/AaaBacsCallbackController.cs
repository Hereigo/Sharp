using System.Text.Json;
using Calendarium.Data;
using Calendarium.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calendarium.Controllers;

/// <summary>
/// Test sink for callbacks: stores every received payload as a <see cref="Note"/>.
/// </summary>
public class AaaBacsCallbackController : ControllerBase
{
    private static readonly JsonSerializerOptions _payloadJsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ApplicationDbContext _context;

    public AaaBacsCallbackController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromBody] JsonElement content)
    {
        string json = content.GetRawText();

        await SaveAsNoteAsync(json);

        try
        {
            JsonSerializer.Deserialize<BacsCallbackPayload>(json, _payloadJsonOptions);
        }
        catch (JsonException)
        {
            await SaveAsNoteAsync("CANNOT PARSE: " + json);
        }

        return Ok();
    }

    private async Task SaveAsNoteAsync(string message)
    {
        _context.Notes.Add(new Note
        {
            SortNum = 0,
            Text = $"{DateTime.Now:MM.dd HH:mm} - {message}"
        });
        await _context.SaveChangesAsync();
    }
}
