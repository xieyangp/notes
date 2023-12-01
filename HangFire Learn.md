## hangfire框架的作用
### 工作方式：将所有细节（如类型、方法名称、参数等）都被序列化并放入存储中，没有数据保存在进程的内存中。
```
客户端：负责创建后台作业并将其保存到存储中。后台作业是应该在当前执行上下文之外执行的工作单元，例如在后台线程、其他进程中，甚至在不同的服务器上，这一切都可以通过 Hangfire 实现，即使没有额外的配置。

服务器：通过查询存储来处理后台作业。粗略地说，它是一组后台线程，用于侦听存储中是否有新的后台作业，并通过反序列化类型、方法和参数来执行它们。
```
### hangfire执行状态：
```
Enqueued（已入队）： 任务已被加入队列，等待执行。这是任务的初始状态。

Scheduled（已调度）： 任务已被调度，但尚未执行。这可能是因为任务设置了延迟执行或者设置了具体的执行时间。

Awaiting（等待执行）： 任务正在等待执行，可能是由于父任务正在执行，而当前任务是子任务，需要等待父任务完成。

Processing（处理中）： 任务正在执行。这表示任务当前正在被某个 Worker 处理。

Failed（执行失败）： 任务执行过程中发生了异常或失败，被标记为失败。

Succeeded（执行成功）： 任务成功完成，没有发生异常。

Deleted（已删除）： 任务被删除，可能是由于某些原因，任务在执行之前被手动删除或过期自动删除。仅限Succeeded内置Deleted状态，但不是Failed最终状态,默认情况下将在 24 小时后自动过期。
```
### hangfire任务排队机制：
```
当使用 Hangfire 进行任务排队时，任务由专用的工作线程池处理。每个工作线程（Worker）执行以下流程：

1.获取下一份工作并对其他工人隐藏： Worker 从队列中获取下一个待执行的工作。这个工作可能是由你的应用程序中的某个地方排队的任务。在 Worker 获取到工作之后，它会将这个工作标记为“隐藏”，以避免其他 Worker 同时执行相同的工作。

2.执行作业及其所有扩展过滤器： Worker 开始执行获取到的工作。这个工作可能是你在应用程序中定义的一个后台任务。在执行工作之前，Hangfire 允许你定义扩展过滤器，这些过滤器可以在执行工作前后进行一些额外的处理。例如，你可以在执行工作之前检查某些条件，或者在工作执行完成后执行一些清理操作。

3.从队列中删除作业： 一旦工作执行完成，Worker 将该工作从队列中删除。这表示该工作已经成功执行，不再需要被其他 Worker 处理。

这个流程确保了任务的有序执行，避免了并发执行相同任务的问题。Hangfire 的工作线程池负责调度和执行任务，确保它们按照先进先出的顺序得到处理。这使得你可以方便地在后台执行一些耗时的任务，而不影响应用程序的性能和响应性。
```
### hangfire任务定期检查计划机制：Hangfire Server定期检查计划，将计划的作业排入其队列，从而允许工作人员执行它们。
```
计划：相当于一个定时任务。

Hangfire Server 定期检查： Hangfire Server 是一个独立的进程或服务，定期（通常每秒）检查计划信息。它从持久化存储中读取计划信息，查看是否有计划的作业需要被执行。这个检查过程是 Hangfire Server 在后台默默运行的。默认时间是15s，可以设置BackgroundJobServer构造函数中SchedulePollingInterval属性改变检查时间间隔。

将计划的作业添加到队列： 当 Hangfire Server 检测到有计划的作业需要执行时，它将这些作业添加到队列中。这意味着这些作业将被放入待执行的队列，等待工作线程池中的工人处理。

工作线程执行作业： 一旦计划的作业被添加到队列，工作线程池中的工人将开始处理这些作业。每个工人会从队列中取出一个作业，执行它，然后将结果报告回 Hangfire Server。
```
## 知识点：
### BackgroundJob.Enqueue：即发即忘作业仅执行一次，并且几乎在创建后立即执行。
```C#
 _backgroundJobClient.Enqueue<IFoodDataProvider>(i =>
            i.CreatedFoodAsync(_mapper.Map<Foods>(command.Food), CancellationToken.None));
/*
请注意，这不是委托，而是表达式树。Hangfire 不会立即调用该方法，而是序列化类型 ( System.Console)、方法名称（WriteLine以及所有参数类型以供稍后识别）以及所有给定参数，并将其放入存储中。
*/
```
### BackgroundJob.Schedule：延迟作业也仅在一定时间间隔后 执行一次，但不会立即执行。
```C#
_backgroundJobClient.Schedule<IFoodDataProvider>(
            i => i.UpdateFoodAsync(_mapper.Map<UpdateFoodDto>(command.Food), CancellationToken.None),
            TimeSpan.FromMinutes(1));
```
### RecurringJob.AddOrUpdate：执行重复任务，通过[Cron表达式](https://en.wikipedia.org/wiki/Cron#CRON_expression)指定执行时间，例如每年一月一号十二点执行一次：
```
RecurringJob.AddOrUpdate("powerfuljob", () => Console.Write("Powerful!"), "0 12 1 1 *");
```
### HangFire在数据库中创建的表的用途：
```C#
HangfireCounter: 用于存储计数器信息，这可能与Hangfire的某些统计信息相关。

HangfireDistributedLock: 用于分布式锁定，确保在多个应用程序实例中不会同时执行相同的任务。

HangfireHash: 用于存储哈希表数据，这可能与某些任务的关联数据有关。

HangfireJob: 存储调度的任务信息，包括任务的标识符、状态、类型等。

HangfireJobParameter: 用于存储与任务相关的参数信息。

HangfireJobQueue: 用于存储任务队列信息，包括队列名称和相关设置。

HangfireJobState: 存储任务的当前状态，例如Enqueued、Failed、Succeeded等。

HangfireList: 用于存储列表数据，这可能与某些任务的关联数据有关。

HangfireServer: 存储Hangfire服务器实例的信息，包括服务器标识符、最后一次活动时间等。

HangfireSet: 用于存储集合数据，这可能与某些任务的关联数据有关。

HangfireState: 存储任务的状态信息，与HangfireJobState可能有一些交叉，用于跟踪任务的状态历史。
```
### Cron:快速理解图
```
 ┌──────────── 分钟 (0–59)
 │ ┌──────────── 小时 (0–23)
 │ │ ┌──────────── 一个月中的某一天 (1–31)
 │ │ │ ┌────────────── 月 (1–12)
 │ │ │ │ ┌──────────── 星期几 (0–6)（星期日至星期六；
 │ │ │ │ │ 7 在某些系统上也是星期日）
 │ │ │ │ │
 │ │ │ │ │
 * * * * * <要执行的命令>
```
## 配置Hangfire
### 1.引用包
```
Hangfire.Core

Hangfire.MySqlStorage

Hangfire.AspNetCore
```
### 2.创建一个类，写Hangfire的配置信息:
```C#
public static class HangFireExtension
{
    public static void AddHangFireService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseStorage(
                new MySqlStorage(
                    configuration.GetConnectionString("Default"),
                    new MySqlStorageOptions
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(10),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        DashboardJobListLimit = 25000,
                        TransactionTimeout = TimeSpan.FromMilliseconds(1),
                        TablesPrefix = "Hangfire"
                    }
                )
            ));
        
        services.AddHangfireServer(options => options.WorkerCount = 1);
    }
}
```
### 3.在Startup中ConfigureServices添加Hangfire服务,在Configure启动Hangfire
```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddHangFireService(Configuration);
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseHangfireServer();//启动Hangfire服务
    app.UseHangfireDashboard();//启动Hangfire面板
}
```
