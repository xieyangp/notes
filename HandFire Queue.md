# 1. 什么是队列？
队列是 Hangfire 中用于存储和执行作业的一种机制。作业按照先进先出（FIFO）的顺序执行。你可以根据需要创建多个队列，每个队列都有自己的作业集合。
# 2. 队列的作用
组织作业: 队列允许你将作业按照一定的规则组织，以便更好地管理它们的执行顺序。
调度和优先级: Hangfire 允许你将作业放入指定的队列，并可以为每个队列设置不同的调度和优先级。
并发控制: 可以通过队列来控制作业的并发执行，确保同一队列中的作业不会同时执行。
# 3. 创建队列
在 Hangfire 中，队列是动态创建的。当你将一个作业放入一个队列时，如果该队列尚不存在，Hangfire 会自动创建它。
```C#
BackgroundJob.Enqueue(() => Console.WriteLine("This job is in the 'default' queue."));

BackgroundJob.Enqueue(() => Console.WriteLine("This job is in the 'critical' queue."), "critical");
```
上面的例子中，第一个作业将进入默认队列，而第二个作业将进入名为 "critical" 的队列。如果 "critical" 队列不存在，Hangfire 会在需要时自动创建它。
# 4. 指定队列
你可以在 Enqueue 方法中为作业指定队列，也可以在使用 BackgroundJob.Enqueue 方法时传递队列参数。
```C#
BackgroundJob.Enqueue(() => Console.WriteLine("This job is in the 'critical' queue."), "critical");
```
# 5. 调度和优先级
Hangfire 允许你为队列设置调度和优先级，以便更精确地控制作业的执行。这通常在启动 Hangfire 时进行配置。
```C#
// 设置 'critical' 队列的调度和优先级
RecurringJob.AddOrUpdate("critical-job", () => Console.WriteLine("Critical job"), Cron.Minutely, new RecurringJobOptions
{
    QueueName = "critical",
    Priority = JobPriority.High
});
```
# 6.使用 AddOrUpdate 创建队列
AddOrUpdate 方法用于设置重复执行的任务，并且可以通过 RecurringJobOptions 指定队列。
```C#
RecurringJob.AddOrUpdate("job-id", () => Console.WriteLine("Recurring job"), Cron.Daily, new RecurringJobOptions
{
    QueueName = "my-queue"
});
```
在上面的例子中，"job-id" 是作业的唯一标识符，Cron.Daily 指定作业每天执行一次，new RecurringJobOptions 允许你设置队列的名称为 "my-queue"。
# 7.使用 Schedule 创建队列
Schedule 方法用于在未来的某个时间点执行一次性任务，并且也可以通过 JobOptions 指定队列。
```C#
var jobId = BackgroundJob.Schedule(() => Console.WriteLine("Scheduled job"), TimeSpan.FromMinutes(30), new JobOptions
{
    QueueName = "my-queue"
});
```
在上面的例子中，TimeSpan.FromMinutes(30) 表示 30 分钟后执行一次性作业，new JobOptions 允许你设置队列的名称为 "my-queue"。
  
在 Hangfire 中，使用 AddOrUpdate 或 Schedule 方法创建队列与创建普通的后台作业有些不同。这两种方法通常用于设置重复执行的任务，而非直接将作业放入队列。
   
注意事项  
1.当使用 AddOrUpdate 或 Schedule 方法时，Hangfire 会自动创建一个内部的队列，用于存储和执行定时作业。这个队列不同于通过 BackgroundJob.Enqueue 方法手动创建的队列。  
   
2.在使用 AddOrUpdate 或 Schedule 方法时，指定的队列名称不同于手动放入队列的方法。这个队列名称仅仅用于定时作业。  
  
总体来说，使用 AddOrUpdate 或 Schedule 方法时，不需要手动创建队列。这些方法会为定时作业创建一个内部队列，并按照你的设置进行执行。而手动放入队列的方法适用于普通的后台作业。  
