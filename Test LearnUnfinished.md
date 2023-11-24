一、测试
    测试的目的保证软件的质量和功能。测试的分类有很多种，下面主要是按开发阶段的测试方法；  
二、知识点  
    1.单元测试：对软件中最小可测试单元进行检查和验证。一般情况下最小可测试单元如一个类，一个函数，一个方法等。   
    2.集成测试：对整个模块功能的正确性、单元模块之间接口的正确性、单个模块的缺陷对整个模块功能的影响、模块之间功能的冲突、全局数据结构的测试，   
    3.系统测试：对系统的功能、界面、兼容性、安全性、性能、可靠性、易用性、容错性；   
    4.E2E测试：将应用程序与其依赖的系统一起进行测试，确保在使用网络后，前后端程序(包括上下游系统)能顺畅交互，从而保证业务上实现闭环，确保满足客户的使用需求，可以帮助发现与系统相关的问题。     
    系统测试与E2E测试的区别：     
    ![系统测试与E2E测试的区别](https://github.com/xieyangp/notes/blob/main/image/Test/Test.png)    
三、使用流程
1.在单元测试中，通过使用AAA模式进行单元测试编写：
```
AAA模式即Arrange，Act，Assert：

Arrange（准备）：在这个部分，我们准备测试所需的对象实例、数据和环境。这包括创建类的实例、设置输入参数和初始化测试数据。

Act（操作）：在这个部分，我们执行要测试的操作或调用要测试的方法。这是对被测试代码进行实际操作的步骤。

Assert（断言）：在这个部分，我们验证操作的结果是否符合预期。我们使用断言方法来检查实际输出与期望输出之间的匹配性。
```
2.测试配置
a.建立一个测试基础类，即TestBase：
```C#
//partial关键字：可在命名空间中定义该类、结构或接口的其他部分。 所有部分都必须使用 partial 关键字。在编译时，各个部分都必须可用来形成最终的类型。
public partial class TestBase : TestUtilbase, IAsyncLifetime, IDisposable
{
    private readonly string _testTopic;
    private readonly string _databaseName;

    private static readonly ConcurrentDictionary<string, IContainer> Containers = new();

    private static readonly ConcurrentDictionary<string, bool> shouldRunDbUpDatabases = new();

    protected ILifetimeScope CurrentScope { get; }

    protected IConfiguration CurrentConfiguration => CurrentScope.Resolve<IConfiguration>();
    
    protected TestBase(string testTopic, string databaseName) 
    {
        _testTopic = testTopic;
        _databaseName = databaseName;

        var root = Containers.GetValueOrDefault(testTopic);

        if (root == null)
        {
            var containerBuilder = new ContainerBuilder();
            var configuration = Registerconfiguration(containerBuilder);
            RegisterBaseContainer(containerBuilder, configuration);
            root = containerBuilder.Build();
            Containers[testTopic] = root;
        }

        CurrentScope = root.BeginLifetimeScope();

        RunDbUpIfRequired();
        SetupScope(CurrentScope);
    }
}
```
b.再创建一个TestBase.Initial.cs类，是TestBase的部分
```C#
public partial class TestBase
{
    private readonly List<string> _tableRecordsDeletionExcludeList = new()
    {
        "schemaversions"
    };

    private void RunDbUpIfRequired()
    {
        if (!shouldRunDbUpDatabases.GetValueOrDefault(_databaseName, true)) return;

        new DbUpRunner(new ConnectionString(CurrentConfiguration).Value).Run();

        shouldRunDbUpDatabases[_databaseName] = false;
    }

    private void RegisterBaseContainer(ContainerBuilder containerBuilder, IConfiguration configuration)
    {
        containerBuilder.RegisterModule(
            new PractiseForJohnnyModule(configuration, typeof(PractiseForJohnnyModule).Assembly));

        containerBuilder.RegisterInstance(Substitute.For<IMemoryCache>()).AsImplementedInterfaces();
    }

    private IConfigurationRoot Registerconfiguration(ContainerBuilder containerBuilder)
    {
        var targetJson = $"appsettings{_testTopic}.json";
        File.Copy("appsettings.json", targetJson, true);
        dynamic jsonObj = JsonConvert.DeserializeObject(File.ReadAllText(targetJson));
        jsonObj["ConnectionStrings"]["Default"] =
            jsonObj["ConnectionStrings"]["Default"].ToString()
                .Replace("Database=smart_faq", $"Database={_databaseName}");
        File.WriteAllText(targetJson, JsonConvert.SerializeObject(jsonObj));
        var configuration = new ConfigurationBuilder().AddJsonFile(targetJson).Build();
        containerBuilder.RegisterInstance(configuration).AsImplementedInterfaces();
        return configuration;
    }

    public async Task InitializeAsync()
    {
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private void ClearDatabaseRecord()
    {
        try
        {
            var connection = new MySqlConnection(new ConnectionString(CurrentConfiguration).Value);

            var deleteStatements = new List<string>();

            connection.Open();

            using var reader = new MySqlCommand(
                    $"SELECT table_name FROM INFORMATION_SCHEMA.tables WHERE table_schema = '{_databaseName}';",
                    connection)
                .ExecuteReader();

            deleteStatements.Add($"SET SQL_SAFE_UPDATES = 0");
            while (reader.Read())
            {
                var table = reader.GetString(0);

                if (!_tableRecordsDeletionExcludeList.Contains(table))
                {
                    deleteStatements.Add($"DELETE FROM `{table}`");
                }
            }

            deleteStatements.Add($"SET SQL_SAFE_UPDATES = 1");

            reader.Close();

            var strDeleteStatements = string.Join(";", deleteStatements) + ";";

            new MySqlCommand(strDeleteStatements, connection).ExecuteNonQuery();

            connection.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cleaning up data, {_testTopic}, {ex}");
        }
    }
```
c.创建一个TestUtilbase类：
```C#
public class TestUtilbase
{
    private ILifetimeScope _scope;

    protected TestUtilbase(ILifetimeScope scope = null)
    {
        _scope = scope;
    }

    protected void SetupScope(ILifetimeScope scope) => _scope = scope; 

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
d.创建一个TestUtil类，继承TestUtilbase
```C#
public class TestUtil : TestUtilbase
{
    protected TestUtil(ILifetimeScope scope)
    {
        SetupScope(scope);
    }

    protected string ReadJsonFileFromResource(string resourceName)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

        if (stream == null) return string.Empty;

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
```
四、学习网址  

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
