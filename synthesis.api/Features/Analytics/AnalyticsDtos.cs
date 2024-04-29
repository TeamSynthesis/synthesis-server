public record ReportDto
{
    public int TotalTasks { get; set; }
    public int CompletedTasksCount { get; set; }
    public int OverdueTasks { get; set; }
    public double TeamProductivity { get; set; }
    public List<Dictionary<int, int>>? DailyTaskCompletions { get; set; }
}
