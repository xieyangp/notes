## hangfire框架的作用
## Hangfire框架是一个用于处理后台任务的开源库，它可以帮助开发人员在.NET应用程序中轻松地执行延迟任务、定时任务和重复任务。Hangfire框架的作用包括：

## 后台任务管理：Hangfire框架可以帮助开发人员管理和执行各种后台任务，包括处理队列、发送电子邮件、生成报告等。

## 定时任务调度：Hangfire框架可以轻松地调度定时任务，例如每天执行一次的数据备份任务或每小时执行一次的数据清理任务。

## 延迟任务处理：开发人员可以使用Hangfire框架来执行延迟任务，例如在用户注册后发送欢迎邮件或在订单创建后生成发货单。

## 可视化监控和管理：Hangfire框架提供了一个直观的仪表板，可以帮助开发人员监控和管理后台任务的执行情况，包括任务的执行状态、执行时间和执行历史记录等。

## 总之，Hangfire框架可以帮助开发人员简化后台任务的管理和执行，提高应用程序的性能和可靠性
## 知识点：HangFire几种常见的后台方法：
### BackgroundJob.Enqueue：即发即忘作业仅执行一次，并且几乎在创建后立即执行。
```C#
  BackgroundJob.Enqueue(() => Console.WriteLine("Hello, world!"));
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
