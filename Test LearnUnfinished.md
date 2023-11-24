# 一、单元测试（Unit Testing）：

目的：单元测试是针对软件中最小的可测试单元（通常是函数、方法或类）进行的测试。其目的是验证单个单元的功能是否正确，以确保其在隔离环境中的行为符合预期。

范围：单元测试的范围仅限于测试单个单元，通常通过模拟或替代依赖来隔离被测试单元。

执行方式：单元测试通常由开发人员编写，并在开发过程中频繁执行，以验证代码的正确性和稳定性。

# 二、集成测试（Integration Testing）：

目的：集成测试是验证多个组件或模块在一起协同工作的测试。其目的是确保各个组件之间的交互和集成正常，并检测潜在的集成问题。

范围：集成测试的范围比单元测试更广，涵盖多个组件、模块或服务之间的交互。它可以包括数据库、API、消息队列等外部依赖的测试。

执行方式：集成测试可以由开发人员或专门的测试团队编写和执行。它通常在开发阶段的后期或集成阶段进行，以确保各个组件的集成是正确的。

# 三、E2E测试（End-to-End Testing）

目的：E2E 测试是模拟真实用户场景，测试整个应用程序或系统的各个组件在集成状态下的行为。其目的是验证整个系统的功能、流程和交互是否按预期工作。

范围：E2E 测试覆盖整个应用程序或系统，包括用户界面、后端逻辑、数据库和外部服务的集成。它模拟用户从开始到结束的完整工作流程。

执行方式：E2E 测试通常由测试团队编写和执行，使用自动化测试框架来模拟用户操作，并验证系统的行为和结果。它可以在开发阶段的后期或系统部署前执行。

# 四、三者之间的联系与区别

## 4.1 联系：

1.这三种测试类型都是为了提高软件质量、发现问题和确保系统的正确性。

2.它们在不同层次和范围上相互补充，从最小的单元到整个系统，逐步验证系统的各个部分。

3.单元测试和集成测试可以由开发人员编写和执行，而 E2E 测试通常由测试团队负责。

4.单元测试和集成测试通常在开发过程中频繁执行，而 E2E 测试可能是一个更终验收的阶段。

5.所有这些测试类型都有助于提前发现和修复问题，减少后期的调试和修复成本。

## 4.2 区别

1.单元测试关注单个单元的功能和逻辑，而集成测试和 E2E 测试涉及多个组件或模块之间的协同工作。

2.单元测试是在隔离环境中执行的，而集成测试和 E2E 测试涉及到真实环境和外部依赖。

3.单元测试通常有更小的范围和更高的粒度，而集成测试和 E2E 测试的范围更广泛，更接近真实用户场景。

4.单元测试和集成测试通常由开发人员编写，而 E2E 测试可能需要测试团队的专门知识和技能。

5.在进行单元测试时，通过编写针对单个函数、方法或类的测试用例，这些测试用例覆盖了各种输入情况和预期输出。通过执行这些测试用例，可以验证代码的正确性，并捕获潜在的错误和异常行为。

#  四、AAA模式

在单元测试中，通常按照三个主要部分来组织测试方法：Arrange、Act 和 Assert，也被称为 AAA 模式。

Arrange（准备）：在这个部分，我们准备测试所需的对象实例、数据和环境。这包括创建类的实例、设置输入参数和初始化测试数据。

Act（操作）：在这个部分，我们执行要测试的操作或调用要测试的方法。这是对被测试代码进行实际操作的步骤。

Assert（断言）：在这个部分，我们验证操作的结果是否符合预期。我们使用断言方法来检查实际输出与期望输出之间的匹配性。



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
