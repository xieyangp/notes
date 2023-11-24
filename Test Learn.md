```C
public class TestUtilbase
{
    private ILifetimeScope _scope;

    protected TestUtilbase(ILifetimeScope scope = null)
    {
        _scope = scope;
    }

    protected void SetupScope(ILifetimeScope scope) => _scope = scope; //?

    protected void Run<T>(Action<T> action, Action<ContainerBuilder> extraRegistration = null)//无放回值
    {
        var dependency = extraRegistration != null
            ? _scope.BeginLifetimeScope(extraRegistration).Resolve<T>()
            : _scope.BeginLifetimeScope().Resolve<T>();
        action(dependency);
    }

    protected void Run<T, R>(Action<T, R> action, Action<ContainerBuilder> extraRegistration = null)//两个参数无返回值
    {
        var lifetime = extraRegistration != null
            ? _scope.BeginLifetimeScope(extraRegistration)
            : _scope.BeginLifetimeScope();

        var dependency = lifetime.Resolve<T>();
        var dependency2 = lifetime.Resolve<R>();
        action(dependency, dependency2);
    }

    protected void Run<T, R, L>(Action<T, R, L> action, Action<ContainerBuilder> extraRegistration = null)//三个无返回值
    {
        var lifetime = extraRegistration != null
            ? _scope.BeginLifetimeScope(extraRegistration)
            : _scope.BeginLifetimeScope();

        var dependency = lifetime.Resolve<T>();
        var dependency2 = lifetime.Resolve<R>();
        var dependency3 = lifetime.Resolve<L>();
        action(dependency, dependency2, dependency3);
    }
    
    protected async Task Run<T, R, L>(Func<T, R, L, Task> action, Action<ContainerBuilder> extraRegistration = null)//异步无返回
    {
        var lifetime = extraRegistration != null
            ? _scope.BeginLifetimeScope(extraRegistration)
            : _scope.BeginLifetimeScope();

        var dependency = lifetime.Resolve<T>();
        var dependency2 = lifetime.Resolve<R>();
        var dependency3 = lifetime.Resolve<L>();
        await action(dependency, dependency2, dependency3);
    }
    
    protected async Task Run<T>(Func<T, Task> action, Action<ContainerBuilder> extraRegistration = null)//异步一参数无返回值
    {
        var dependency = extraRegistration != null
            ? _scope.BeginLifetimeScope(extraRegistration).Resolve<T>()
            : _scope.BeginLifetimeScope().Resolve<T>();
        await action(dependency);
    }
    
    protected async Task RunWithUnitOfWork<T>(Func<T, Task> action, Action<ContainerBuilder> extraRegistration = null)//异步一参数无返回值保存
    {
        var scope = extraRegistration != null
            ? _scope.BeginLifetimeScope(extraRegistration)
            : _scope.BeginLifetimeScope();

        var dependency = scope.Resolve<T>();
        var unitOfWork = scope.Resolve<IUnitOfWork>();

        await action(dependency);
        await unitOfWork.SaveChangesAsync();
    }
    
    protected async Task<R> Run<T, R>(Func<T, Task<R>> action, Action<ContainerBuilder> extraRegistration = null)//异步一参数有返回值
    {
        var dependency = extraRegistration != null
            ? _scope.BeginLifetimeScope(extraRegistration).Resolve<T>()
            : _scope.BeginLifetimeScope().Resolve<T>();
        return await action(dependency);
    }
    
    protected async Task<R> RunWithUnitOfWork<T, R>(Func<T, Task<R>> action, Action<ContainerBuilder> extraRegistration = null)//一参数有返回值保存
    {
        var scope = extraRegistration != null
            ? _scope.BeginLifetimeScope(extraRegistration)
            : _scope.BeginLifetimeScope();

        var dependency = scope.Resolve<T>();
        var unitOfWork = scope.Resolve<IUnitOfWork>();

        var result = await action(dependency);
        await unitOfWork.SaveChangesAsync();

        return result;
    }
    
    protected async Task<R> RunWithUnitOfWork<T, U, R>(Func<T, U, Task<R>> action, Action<ContainerBuilder> extraRegistration = null)//异步两参数有放回值保存
    {
        var scope = extraRegistration != null
            ? _scope.BeginLifetimeScope(extraRegistration)
            : _scope.BeginLifetimeScope();

        var dependency = scope.Resolve<T>();
        var dependency1 = scope.Resolve<U>();
        var unitOfWork = scope.Resolve<IUnitOfWork>();

        var result = await action(dependency, dependency1);
        await unitOfWork.SaveChangesAsync();

        return result;
    }
    
    protected async Task RunWithUnitOfWork<T, U>(Func<T, U, Task> action, Action<ContainerBuilder> extraRegistration = null)//异步两参数无返回值保存
    {
        var scope = extraRegistration != null
            ? _scope.BeginLifetimeScope(extraRegistration)
            : _scope.BeginLifetimeScope();

        var dependency = scope.Resolve<T>();
        var dependency2 = scope.Resolve<U>();
        var unitOfWork = scope.Resolve<IUnitOfWork>();

        await action(dependency, dependency2);
        await unitOfWork.SaveChangesAsync();
    }
    
    protected R Run<T, R>(Func<T, R> action, Action<ContainerBuilder> extraRegistration = null)//一参数一返回值
    {
        var dependency = extraRegistration != null
            ? _scope.BeginLifetimeScope(extraRegistration).Resolve<T>()
            : _scope.BeginLifetimeScope().Resolve<T>();
        return action(dependency);
    }
    
    protected R Run<T, U, R>(Func<T, U, R> action, Action<ContainerBuilder> extraRegistration = null)//两参数一返回值
    {
        var lifetime = extraRegistration != null
            ? _scope.BeginLifetimeScope(extraRegistration)
            : _scope.BeginLifetimeScope();

        var dependency = lifetime.Resolve<T>();
        var dependency2 = lifetime.Resolve<U>();
        return action(dependency, dependency2);
    }
    
    protected Task Run<T, U>(Func<T, U, Task> action, Action<ContainerBuilder> extraRegistration = null)//异步两参数无返回值
    {
        var lifetime = extraRegistration != null
            ? _scope.BeginLifetimeScope(extraRegistration)
            : _scope.BeginLifetimeScope();
        var dependency = lifetime.Resolve<T>();
        var dependency2 = lifetime.Resolve<U>();
        return action(dependency, dependency2);
    }
}
```
## NSubstitute
###  Substitute.For<Interface>():创建替代品；最好用接口创建，用类创建容易出现一些问题，例如类中任何非虚拟代码都将被执行。
    替代多个接口：Substitute.For<ICommand, IDisposable>()；可以替代多个接口，但是只能实现一个。
    替代委托类型：Substiute.For<T>()。当替换委托类型时，您将无法让替换项实现其他接口或类
### Resturns：设置返回值
    方法设置：calculator.Add(1, 2).Returns(3);每次使用calculator.Add(1, 2)都有一个返回值为3
    属性设置：calculator.Mode.Returns("DEC");每次使用calculator.Mode都为"DEC"
### 参数匹配器：设置返回值 和 检查收到的调用 时可以使用参数匹配器   
    Arg.Any<T>():1.忽略参数，Arg.Any<int>()表示任何数字
    Arg.Is<T>();
   
    注意：arg 匹配器实际上是模糊匹配的；不直接传递给函数调用
    ReturnsForAnyArgs()
## 来电信息：
    Returns()和 的函数ReturnsForAnyArgs()的类型为Func<CallInfo,T>，其中T是调用返回的类型，并且CallInfo是提供对调用所用参数的访问的类型  
    T Arg<T>()T：获取传递给此调用的参数类型。  
    T ArgAt<T>(int position)：获取在指定的从零开始的位置传递给此调用的参数，并将其转换为类型T。
