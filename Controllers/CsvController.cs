using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using CsvProcessor.Models;

namespace CsvProcessor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CsvController : ControllerBase
{
    private readonly AppDbContext _db;

    public CsvController(AppDbContext db)
    {
        _db = db;
    }
    [HttpPost("upload")]
    public async Task<IActionResult> UploadCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "Файл не выбран" });
        using var reader = new StreamReader(file.OpenReadStream());
        var content = await reader.ReadToEndAsync();
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 1 || lines.Length > 10000)
            return BadRequest(new { error = $"Строк должно быть от 1 до 10000, сейчас {lines.Length}" });

        var parsedData = new List<(DateTime Date, double Time, double Value)>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var parts = line.Split(';');
            if (parts.Length != 3)
                return BadRequest(new { error = $"Строка {i + 1}: должно быть 3 колонки" });

            if (!DateTime.TryParseExact(parts[0].Trim(), "yyyy-MM-ddTHH-mm-ss.ffffZ",
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date))
                return BadRequest(new { error = $"Строка {i + 1}: неверный формат даты" });
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            if (date > DateTime.UtcNow)
                return BadRequest(new { error = $"Строка {i + 1}: дата в будущем" });

            if (date < new DateTime(2000, 1, 1))
                return BadRequest(new { error = $"Строка {i + 1}: дата раньше 2000 года" });

            if (!double.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var time))
                return BadRequest(new { error = $"Строка {i + 1}: неверный формат времени" });

            if (time < 0)
                return BadRequest(new { error = $"Строка {i + 1}: время не может быть отрицательным" });

            if (!double.TryParse(parts[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                return BadRequest(new { error = $"Строка {i + 1}: неверный формат значения" });

            if (value < 0)
                return BadRequest(new { error = $"Строка {i + 1}: значение не может быть отрицательным" });

            parsedData.Add((date, time, value));
        }

        var oldValues = await _db.Values.Where(v => v.FileName == file.FileName).ToListAsync();
        if (oldValues.Any())
            _db.Values.RemoveRange(oldValues);

        var oldResult = await _db.Results.FirstOrDefaultAsync(r => r.FileName == file.FileName);
        if (oldResult != null)
            _db.Results.Remove(oldResult);

        var newValues = parsedData.Select(d => new ValueRecord
        {
            FileName = file.FileName,
            Date = d.Date,
            ExecutionTime = d.Time,
            Value = d.Value
        });

        await _db.Values.AddRangeAsync(newValues);
        var minDate = parsedData.Min(d => d.Date);
        var maxDate = parsedData.Max(d => d.Date);
        var deltaTime = (maxDate - minDate).TotalSeconds;
        var avgExecutionTime = parsedData.Average(d => d.Time);
        var avgValue = parsedData.Average(d => d.Value);
        var maxValue = parsedData.Max(d => d.Value);
        var minValue = parsedData.Min(d => d.Value);

        var sortedValues = parsedData.Select(d => d.Value).OrderBy(v => v).ToList();
        double median;
        if (sortedValues.Count % 2 == 1)
            median = sortedValues[sortedValues.Count / 2];
        else
            median = (sortedValues[sortedValues.Count / 2 - 1] + sortedValues[sortedValues.Count / 2]) / 2;

        var result = new ResultRecord
        {
            FileName = file.FileName,
            DeltaTimeSeconds = deltaTime,
            MinDate = minDate,
            AvgExecutionTime = avgExecutionTime,
            AvgValue = avgValue,
            MedianValue = median,
            MaxValue = maxValue,
            MinValue = minValue
        };

        await _db.Results.AddAsync(result);
        await _db.SaveChangesAsync();

        return Ok(new { message = $"Файл {file.FileName} загружен", records = parsedData.Count });
    }
    [HttpGet("results")]
    public async Task<IActionResult> GetResults(
        [FromQuery] string? fileName = null,
        [FromQuery] DateTime? minDate = null,
        [FromQuery] DateTime? maxDate = null,
        [FromQuery] double? minAvgValue = null,
        [FromQuery] double? maxAvgValue = null,
        [FromQuery] double? minAvgExecutionTime = null,
        [FromQuery] double? maxAvgExecutionTime = null)
    {
        var query = _db.Results.AsQueryable();

        if (!string.IsNullOrEmpty(fileName))
            query = query.Where(r => r.FileName.Contains(fileName));

        if (minDate.HasValue)
            query = query.Where(r => r.MinDate >= minDate.Value);

        if (maxDate.HasValue)
            query = query.Where(r => r.MinDate <= maxDate.Value);

        if (minAvgValue.HasValue)
            query = query.Where(r => r.AvgValue >= minAvgValue.Value);

        if (maxAvgValue.HasValue)
            query = query.Where(r => r.AvgValue <= maxAvgValue.Value);

        if (minAvgExecutionTime.HasValue)
            query = query.Where(r => r.AvgExecutionTime >= minAvgExecutionTime.Value);

        if (maxAvgExecutionTime.HasValue)
            query = query.Where(r => r.AvgExecutionTime <= maxAvgExecutionTime.Value);

        var results = await query.ToListAsync();
        return Ok(results);
    }

    [HttpGet("values/{fileName}/last")]
    public async Task<IActionResult> GetLastValues(string fileName)
    {
        var values = await _db.Values
            .Where(v => v.FileName == fileName)
            .OrderByDescending(v => v.Date)
            .Take(10)
            .ToListAsync();

        if (!values.Any())
            return NotFound(new { error = $"Файл {fileName} не найден" });

        return Ok(values);
    }
}