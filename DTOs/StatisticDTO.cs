namespace backend.DTOs;

public partial class MonthlyAvgPointDTO
{
    public List<string> Time { get; set; } = [];

    public List<decimal> AvgPoint { get; set; } = [];

    public void AddMonth(int month, int year, decimal avgPoint = 0)
    {
        Time.Add($"{month}/{year}");
        AvgPoint.Add(avgPoint);
    }
}

public partial class PointDifficultyDTO
{
    public byte? Difficulty { get; set; }

    public List<PointDifficultyItemDTO> Types { get; set; } = [];

    public decimal Percentage { get; set; } = 0;

    public PointDifficultyDTO(byte difficulty)
    {
        Difficulty = difficulty;
    }
}

public partial class PointDifficultyItemDTO
{
    public byte? Difficulty { get; set; }

    public string? Type { get; set; }

    public int Count { get; set; } = 0;

    public int Total { get; set; } = 0;

    public decimal Percentage { get; set; } = 0;
}