namespace backend.DTOs;

public partial class MonthlyAvgPointDTO
{
    public int Month { get; set; }

    public int Year { get; set; }

    public decimal? AvgPoint { get; set; }
}

public partial class PointDifficultyDTO
{
    public byte? Difficuly { get; set; }
    
    public decimal? AvgPoint { get; set; }
}