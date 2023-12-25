#指令开发需求学习
1.通过接口成员优化if
```
//原逻辑
if(intent = "指令a")
{
}

if(intent = "指令b")
{
}

if(intent = "指令c")
{
}

//优化后
//1.新增一个选择指令类
public interface IIntentSwitcher
{
    IIntentHandler IntentHandler(Intent intent);
}

public class IntentSwitcher : IIntentSwitcher
{
    private readonly IEnumerable<IIntentHandler> _intentHandlers;

    public AskFaqSpecialIntentSwitcher(IEnumerable<IIntentHandler> intentHandlers)
    {
        _intentHandlers = intentHandlers;
    }

    public IIntentHandler IntentHandler(Intent intent)
    {
        return _intentHandlers.FirstOrDefault(x => x.Intent == intent);
    }
}

//2.新增指令处理类
public interface IIntentHandler
{
    Intent Intent { get; }

    Task<List<ResultDto>> HandleIntentAsync(IntentDto intent, CancellationToken cancellationToken);
}

public class AIntentHandler : IIntentHandler
{
    public async Task<List<ResultDto>> HandleIntentAsync(IntentDto intent, CancellationToken cancellationToken)
    {
      //逻辑处理
    }
   Intent Intent  => Intent.A;
}

public class BIntentHandler : IIntentHandler
{
    public async Task<List<ResultDto>> HandleIntentAsync(IntentDto intent, CancellationToken cancellationToken)
    {
      //逻辑处理
    }
   Intent Intent  => Intent.B;
}

public class CIntentHandler : IIntentHandler
{
    public async Task<List<ResultDto>> HandleIntentAsync(IntentDto intent, CancellationToken cancellationToken)
    {
      //逻辑处理
    }
   Intent Intent  => Intent.C;
}

//调用
publi Class Test
{
   private readonly IIntentSwitcher _intentSwitcher;

   public Test(IIntentSwitcher intentSwitcher)
   {
     _intentSwitcher = intentSwitcher;
   }

   public Task Fun()
   {
      await _intentSwitcher.IntentHandler(intent).HandleIntentAsync(intentDto, cancellationToken);
   }
}
```
