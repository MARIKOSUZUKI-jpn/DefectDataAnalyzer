namespace DefectApp.Models;

public class Defect
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? DefectType { get; set; }
    public string? CauseAnalysis { get; set; }
    public DateTime CreatedAt { get; set; }
}