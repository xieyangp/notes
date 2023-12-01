## hangfire框架的作用
### 工作方式：所有细节（如类型、方法名称、参数等）都被序列化并放入存储中，没有数据保存在进程的内存中。
```
客户端：负责创建后台作业并将其保存到存储中。后台作业是应该在当前执行上下文之外执行的工作单元，例如在后台线程、其他进程中，甚至在不同的服务器上——这一切都可以通过 Hangfire 实现，即使没有额外的配置。

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
### Hangfire框架是一个用于处理后台任务的开源库，它可以帮助开发人员在.NET应用程序中轻松地执行延迟任务、定时任务和重复任务。Hangfire框架的作用包括：

### 后台任务管理：Hangfire框架可以帮助开发人员管理和执行各种后台任务，包括处理队列、发送电子邮件、生成报告等。

### 定时任务调度：Hangfire框架可以轻松地调度定时任务，例如每天执行一次的数据备份任务或每小时执行一次的数据清理任务。

### 延迟任务处理：开发人员可以使用Hangfire框架来执行延迟任务，例如在用户注册后发送欢迎邮件或在订单创建后生成发货单。

### 可视化监控和管理：Hangfire框架提供了一个直观的仪表板，可以帮助开发人员监控和管理后台任务的执行情况，包括任务的执行状态、执行时间和执行历史记录等。

### 总之，Hangfire框架可以帮助开发人员简化后台任务的管理和执行，提高应用程序的性能和可靠性
## 知识点：HangFire几种常见的后台方法：
### BackgroundJob.Enqueue：即发即忘作业仅执行一次，并且几乎在创建后立即执行。
```C#
BackgroundJob.Enqueue(() => Console.WriteLine("Hello, world!"));

/*
请注意，这不是委托，而是表达式树。Hangfire 不会立即调用该方法，而是序列化类型 ( System.Console)、方法名称（WriteLine以及所有参数类型以供稍后识别）以及所有给定参数，并将其放入存储中。
*/
```
### BackgroundJob .Schedule：延迟作业也仅在一定时间间隔后 执行一次，但不会立即执行。
```C#
BackgroundJob .Schedule(
    () => Console .WriteLine( "延迟！" ),
     TimeSpan .FromDays(7));
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
