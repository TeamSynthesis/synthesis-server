using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.AppConfig;
using Octokit;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Mappings;

public interface IAnalyticsService
{
    Task<GlobalResponse<ReportDto>> GetProjectReports(Guid projectId);
}

public class AnalyticsService : IAnalyticsService
{
    private readonly RepositoryContext _repository;

    public AnalyticsService(RepositoryContext repository)
    {
        _repository = repository;
    }

    public async Task<GlobalResponse<ReportDto>> GetProjectReports(Guid projectId)
    {
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
        var today = DateTime.UtcNow;


        var projectReport = await _repository.Projects.Where(p => p.Id == projectId).Select(p => new ReportDto
        {
            TotalTasks = p.Tasks.Count() + p.Features.Select(f => f.Tasks).Count(),
            CompletedTasksCount = p.Tasks.Where(t => t.IsComplete == true).Count() + p.Features.Select(f => f.Tasks.Where(t => t.IsComplete == true)).Count(),
            OverdueTasks = p.Tasks.Where(t => t.DueDate <= DateTime.UtcNow).Count() + p.Features.Select(f => f.Tasks.Where(t => t.DueDate <= DateTime.UtcNow)).Count(),
            TeamProductivity =
            p.Tasks.Where(t => t.IsComplete == true).Count() + p.Features.Select(f => f.Tasks.Where(t => t.IsComplete == true)).Count() / (p.Tasks.Count() + p.Features.Select(f => f.Tasks).Count()),

            DailyTaskCompletions = new List<Dictionary<int, int>>()
            {
                new Dictionary<int, int>(){

                        { 0, new Random().Next(0, 3) },
                        { 1, new Random().Next(0, 4) },
                        { 2, new Random().Next(0, 5) },
                        { 3, new Random().Next(0, 6) },
                        { 4, new Random().Next(0, 7) },
                        { 5, new Random().Next(0, 5) },
                        { 6, new Random().Next(0, 2) }

                }
            }
        }).FirstOrDefaultAsync();
        if (projectReport == null)
        {
            return new GlobalResponse<ReportDto>(false, "getprojectreportsfailed", errors: [$"project with id: {projectId} not found"]);
        }

        return new GlobalResponse<ReportDto>(true, "get project reports success", projectReport);

    }

    private double CalculateWeeklyProductivity(List<TaskToDoModel> allTasks, DateTime startDate, DateTime endDate)
    {
        // Filter tasks for the specific week
        var weeklyTasks = allTasks.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate).ToList();

        // Count tasks completed on time
        var onTimeCompletions = weeklyTasks.Count(t => t.CreatedOn <= t.DueDate);

        // Calculate productivity percentage
        return (double)onTimeCompletions / weeklyTasks.Count * 100;
    }

}