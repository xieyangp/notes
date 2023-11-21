# UnitOfWork工作单元模式
##  一、工作单元作用：跨多个请求的业务，统一管理事务，统一提交。  
##  二、我们经常的代码都是分层的，有可能到处都在 new DbContext(options)，这是就要面对如何管理这些DbContext，在AspNetCore中 services.AddDbContext<>默认是用的Scope的作用域，也就是每次HttpRequest，比以前好了很多。但是事务这些管理还是很麻烦。  
  ![unitwork工作原理图](https://github.com/xieyangp/notes/blob/main/image/EFCore/unitwork.png)  
### 如上图 有一个Action需要调用很多Service 然后 Service之间又相互调用，在开启Action时 其实是想开启一个事务，但是某些内部代码有可能自己去开启了事务。相互之间调用管理起来非常麻烦。经常出现不可估计的问题。如果有一个集中管理的地方就好很多。比如在Action这里启动一个工### 作单元，后续所有的业务都使用同一个事务 和 DbContext，这才是我们的预期的。  
##  三、如何使用工作单元
### 1.添加一个静态类：
```C#
public static class UnitOfWorkMiddleWare
{
    public static void UseUnitOfWork<TContext>(this IPipeConfigurator<TContext> configurator,
        IUnitOfWork unitOfWork = null) where TContext : IContext<IMessage>
    {
        if (unitOfWork == null && configurator.DependencyScope == null)
        {
            throw new DependencyScopeNotConfiguredException(
                $"{nameof(unitOfWork)} is not provided and IDependencyScope is not configured, Please ensure {nameof(unitOfWork)} is registered properly if you are using IoC                           container, otherwise please pass {nameof(unitOfWork)} as parameter");
        }

        unitOfWork ??= configurator.DependencyScope.Resolve<IUnitOfWork>();

        configurator.AddPipeSpecification(new UnifyOfWorkSpacification<TContext>(unitOfWork));
    }
}
```
### 2.添加一个规范类继承IPipeSpecification<TContext>:
```C#
public class UnifyOfWorkSpacification<TContext> : IPipeSpecification<TContext>
    where TContext : IContext<IMessage>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnifyOfWorkSpacification(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public bool ShouldExecute(TContext context, CancellationToken cancellationToken)
    {
        return true;
    }

    public Task BeforeExecute(TContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task Execute(TContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task AfterExecute(TContext context, CancellationToken cancellationToken)
    {
        if (_unitOfWork.ShouldSaveChanges)//
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _unitOfWork.ShouldSaveChanges = false;
        }
    }

    public Task OnException(Exception ex, TContext context)
    {
        ExceptionDispatchInfo.Capture(ex).Throw();
        throw ex;
    }
}
```
### 3.创建一个IUnitOfWork接口：
```C#
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    bool ShouldSaveChanges { get; set; }
}
```
### 4.DbContext继承IUnitOfWork接口，实现ShouldSaveChanges成员
```C#
public class PratiseForJohnnyDbContext : DbContext, IUnitOfWork
{
    public bool ShouldSaveChanges { get; set; }
}
```
