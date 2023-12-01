# 一、创建一个后台方法在控制器中
```C#
[HttpPost]
[Route("recurring/update/food")]
public async Task<IActionResult> RecurringUpdateFoodAsync([FromBody] RecurringUpdateFoodCommand command)
{
    await _mediator.SendAsync(command).ConfigureAwait(false);
    
    return Ok();
}
```
# 二、创建一个Command命令
```C#
public class RecurringUpdateFoodCommand : ICommand
{
    public UpdateFoodDto Food { get; set; }
}
```
# 三、创建一个CommandHandler处理程序
```C#
public class RecurringUpdateFoodCommandHandler : ICommandHandler<RecurringUpdateFoodCommand>
{
    private readonly IFoodService _foodService;

    public RecurringUpdateFoodCommandHandler(IFoodService foodService)
    {
        _foodService = foodService;
    }

    public async Task Handle(IReceiveContext<RecurringUpdateFoodCommand> context, CancellationToken cancellationToken)
    {
        await _foodService.RecurringUpdateFood(context.Message, cancellationToken);
    }
}
```
# 四、创建一个创建后台方法
```C#
public Task RecurringUpdateFood(RecurringUpdateFoodCommand command, CancellationToken cancellationToken)
{
    RecurringJob.AddOrUpdate<IFoodDataProvider>("updateFoodRecurringJob" + command.Food.Id, //"updateFoodRecurringJob" + command.Food.Id:是这个后台id，如果id相同会覆盖
        i => i.UpdateFoodAsync(_mapper.Map<UpdateFoodDto>(command.Food), CancellationToken.None),
        _updateFoodJobCron.Value, new RecurringJobOptions() //_updateFoodJobCron：是将Corn配置在配置文件中，通过类的构造函数读取，再通过依赖注入进来
        {
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time")
        });
    
    return Task.CompletedTask;
}
```
# 五、Corn配置到配置文件
```C#
public class UpdateFoodJobCron : IConfiguartionSetting<string>
{
    public string Value { get; set; }

    public UpdateFoodJobCron(IConfiguration configuration)
    {
        Value = configuration["UpdateFoodJobCron:Cron"];
    }
}
```
