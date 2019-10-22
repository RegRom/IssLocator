using Hangfire;
using IssLocator.Controllers;

namespace IssLocator.Hangfire
{
    public class RecurringJobsScheduler
    {
        public static void ScheduleRecurringJobs()
        {
            RecurringJob.RemoveIfExists("AddCheckPointJob");
            RecurringJob.AddOrUpdate<IssLocationController>("AddCheckPointJob", job => job.AddLocationToDb(), Cron.MinuteInterval(1));
        }
    }
}
