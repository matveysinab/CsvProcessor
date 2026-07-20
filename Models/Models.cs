using Microsoft.EntityFrameworkCore;

namespace CsvProcessor.Models;

public class ValueRecord
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double ExecutionTime { get; set; }
    public double Value { get; set; }
}
public class ResultRecord
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public double DeltaTimeSeconds { get; set; }
    public DateTime MinDate { get; set; }
    public double AvgExecutionTime { get; set; }
    public double AvgValue { get; set; }
    public double MedianValue { get; set; }
    public double MaxValue { get; set; }
    public double MinValue { get; set; }
}
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ValueRecord> Values { get; set; }
    public DbSet<ResultRecord> Results { get; set; }
}