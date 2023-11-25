## 一、测试
    测试的目的保证软件的质量和功能。测试的分类有很多种，下面主要是按开发阶段的测试方法；  
## 二、知识点  
    1.单元测试：对软件中最小可测试单元进行检查和验证。一般情况下最小可测试单元如一个类，一个函数，一个方法等。   
    2.集成测试：对整个模块功能的正确性、单元模块之间接口的正确性、单个模块的缺陷对整个模块功能的影响、模块之间功能的冲突、全局数据结构的测试，   
    3.系统测试：对系统的功能、界面、兼容性、安全性、性能、可靠性、易用性、容错性；   
    4.E2E测试：将应用程序与其依赖的系统一起进行测试，确保在使用网络后，前后端程序(包括上下游系统)能顺畅交互，从而保证业务上实现闭环，确保满足客户的使用需求，可以帮助发现与系统相关的问题。     
### 系统测试与E2E测试的区别：     
![系统测试与E2E测试的区别](https://github.com/xieyangp/notes/blob/main/image/Test/Test.png)    
## 三、使用流程
### 1.在单元测试中，通过使用AAA模式进行单元测试编写：
```
AAA模式即Arrange，Act，Assert：

Arrange（准备）：在这个部分，我们准备测试所需的对象实例、数据和环境。这包括创建类的实例、设置输入参数和初始化测试数据。

Act（操作）：在这个部分，我们执行要测试的操作或调用要测试的方法。这是对被测试代码进行实际操作的步骤。

Assert（断言）：在这个部分，我们验证操作的结果是否符合预期。我们使用断言方法来检查实际输出与期望输出之间的匹配性。
```
一个单元测试的例子
```C#
// 定义一个接口
public interface ICalculator
{
    int Add(int a, int b);
}

// 要测试的类
public class MyCalculator
{
    private ICalculator _calculator;

    public MyCalculator(ICalculator calculator)
    {
        _calculator = calculator;
    }

    public int AddTwoNumbers(int x, int y)
    {
        // 调用ICalculator接口的方法
        return _calculator.Add(x, y);
    }
}

// 单元测试
[Test]
public void TestAddTwoNumbers()
{
    // 创建一个模拟对象代替ICalculator接口的实现
    var calculator = Substitute.For<ICalculator>();
    
    // 设置模拟对象的方法的行为
    calculator.Add(1, 2).Returns(3);
    
    // 创建要测试的类的实例，并传入模拟对象
    var myCalculator = new MyCalculator(calculator);
    
    // 调用要测试的方法
    int result = myCalculator.AddTwoNumbers(1, 2);
    
    // 验证调用方法的行为和结果
   result.ShouldBe(3);
    
    // 验证被调用的方法是否被调用了指定的次数
    calculator.Received().Add(1, 2);
}

```
### 2.测试配置   
#### a.建立一个测试基础类，用来设置生命周期和初始化测试环境，以及添加测试主题和数据库名，即TestBase：
```C#
//partial关键字：可在命名空间中定义该类、结构或接口的其他部分。 所有部分都必须使用 partial 关键字。在编译时，各个部分都必须可用来形成最终的类型。
public partial class TestBase : TestUtilbase, IAsyncLifetime, IDisposable
{
    private readonly string _testTopic;//测试主题
    private readonly string _databaseName;//数据库名

    private static readonly ConcurrentDictionary<string, IContainer> Containers = new();//储存容器

    private static readonly ConcurrentDictionary<string, bool> shouldRunDbUpDatabases = new();//是否需要运行Dbup类

    protected ILifetimeScope CurrentScope { get; }//生命周期范围

    protected IConfiguration CurrentConfiguration => CurrentScope.Resolve<IConfiguration>();//当前配置
    
    protected TestBase(string testTopic, string databaseName) 
    {
        _testTopic = testTopic;
        _databaseName = databaseName;
        
        var root = Containers.GetValueOrDefault(testTopic);//获取测试容器

        if (root == null)//当前容器不存在就新建一个容器
        {
            var containerBuilder = new ContainerBuilder();
            var configuration = Registerconfiguration(containerBuilder);
            RegisterBaseContainer(containerBuilder, configuration);
            root = containerBuilder.Build();
            Containers[testTopic] = root;
        }

        CurrentScope = root.BeginLifetimeScope();//创建一个新的生命周期范围

        RunDbUpIfRequired();//如果需要运行DbUp数据库则运行它
        SetupScope(CurrentScope);//设置生命周期
    }
}
```
#### b.再创建一个TestBase.Initial.cs类，主要用于运行数据库迁移、注册容器和配置，以及清理数据库记录，也是TestBase的部分
```C#
public partial class TestBase
{
    //用来存储需要排除的表名
    private readonly List<string> _tableRecordsDeletionExcludeList = new()
    {
        "schemaversions"
    };

    //运行DbUp数据库迁移
    private void RunDbUpIfRequired()
    {
        //检查字典中是否存在指定数据库名称
        if (!shouldRunDbUpDatabases.GetValueOrDefault(_databaseName, true)) return;

        //存在，执行数据库迁移
        new DbUpRunner(new ConnectionString(CurrentConfiguration).Value).Run();

        //将字典中的数据库名设置为false，表示已经迁移过
        shouldRunDbUpDatabases[_databaseName] = false;
    }

    //在容器注册Module和IMemoryCache。
    private void RegisterBaseContainer(ContainerBuilder containerBuilder, IConfiguration configuration)
    {
        containerBuilder.RegisterModule(
            new PractiseForJohnnyModule(configuration, typeof(PractiseForJohnnyModule).Assembly));

        containerBuilder.RegisterInstance(Substitute.For<IMemoryCache>()).AsImplementedInterfaces();
    }

    //注册配置
    private IConfigurationRoot Registerconfiguration(ContainerBuilder containerBuilder)
    {
        var targetJson = $"appsettings{_testTopic}.json";
        File.Copy("appsettings.json", targetJson, true);//将appsettings.json复制到targetJson
        dynamic jsonObj = JsonConvert.DeserializeObject(File.ReadAllText(targetJson));//JsonConvert.DeserializeObject方法将targetJson文件的内容解析为动态对象jsonObj，
        jsonObj["ConnectionStrings"]["Default"] =
            jsonObj["ConnectionStrings"]["Default"].ToString()
                .Replace("Database=smart_faq", $"Database={_databaseName}");//修改其中的连接字符串，将数据库名称替换为_databaseName。
        File.WriteAllText(targetJson, JsonConvert.SerializeObject(jsonObj));//然后将修改后的jsonObj重新写入到targetJson文件中
        var configuration = new ConfigurationBuilder().AddJsonFile(targetJson).Build();//使用ConfigurationBuilder将其添加为配置
        containerBuilder.RegisterInstance(configuration).AsImplementedInterfaces();//将configuration实例注册为containerBuilder的实现接口
        return configuration;//返回configuration对象
    }

    public async Task InitializeAsync()
    {
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    //清除数据库中的记录
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
                .ExecuteReader();//将_databaseName中的表名全部读取出来

            deleteStatements.Add($"SET SQL_SAFE_UPDATES = 0");//创建一个空的deleteStatements列表，用于存储删除记录的SQL语句。并且添加了"SET SQL_SAFE_UPDATES = 0"语句，这是为了禁用安全更新模式。

            //通过循环读取DataReader对象中的数据，获取每个表的名称，并检查是否在排除列表中。如果不在排除列表中，则向deleteStatements列表中添加"DELETE FROM {table}"语句，用于删除该表中的所有记录。
            while (reader.Read())
            {
                var table = reader.GetString(0);

                if (!_tableRecordsDeletionExcludeList.Contains(table))
                {
                    deleteStatements.Add($"DELETE FROM `{table}`");
                }
            }

            deleteStatements.Add($"SET SQL_SAFE_UPDATES = 1");//启用安全更新模式。

            reader.Close();

            var strDeleteStatements = string.Join(";", deleteStatements) + ";";//将deleteStatements列表中的SQL语句通过string.Join方法连接成一个完整的SQL语句，并在末尾添加分号。

            new MySqlCommand(strDeleteStatements, connection).ExecuteNonQuery();

            connection.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cleaning up data, {_testTopic}, {ex}");
        }
    }
```
#### c.创建一个TestUtilbase类，这个类提供了一些方法来管理生命周期范围，并在生命周期范围内执行某些操作：
```C#
public class TestUtilbase
{
    private ILifetimeScope _scope;

    protected TestUtilbase(ILifetimeScope scope = null)
    {
        _scope = scope;
    }

    protected void SetupScope(ILifetimeScope scope) => _scope = scope; //子类通过这个方法将scope赋值给_scope

    protected void Run<T>(Action<T> action, Action<ContainerBuilder> extraRegistration = null)//无放回值
    {
        //容器存在注册额外组件，再解析出T类型，不存在直接解析T类型
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
#### d.创建一个TestUtil类，创建生命周期，继承TestUtilbase
```C#
public class TestUtil : TestUtilbase
{
    protected TestUtil(ILifetimeScope scope)
    {
        SetupScope(scope);
    }

    //从程序集获取resourceName的流，并读取返回字符串
    protected string ReadJsonFileFromResource(string resourceName)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

        if (stream == null) return string.Empty;

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
```
### 3.开始编写集成测试：
#### a.创建一个以Fixture结尾的测试基础类,继承FoodFixtureBase
```c#
[Collection("Food Tests")]
public class FoodFixtureBase : TestBase
{
    protected FoodFixtureBase() : base("_food_", "Test")//_food_：测试标题，Test：数据库名；
    {
    }
}
```
#### b.创建一个测试类,继承Fixture基础类，这里面写测试方法测试方法：
```C#
public partial class FoodFixture : FoodFixtureBase
{
    private readonly FoodsUtil _foodsUtil;

    public FoodFixture()
    {
        _foodsUtil = new FoodsUtil(CurrentScope);
    }

    [Fact]
    public async Task CanCreateFood()
    {
        var food = new CreateFoodDto { Name = "mike", Color = "white" };

        await Run<IRepository>(async repository =>
        {
            var beforeCreateFood = await repository.CountAsync<Foods>(x => true).ConfigureAwait(false);

            beforeCreateFood.ShouldBe(0);

            await _foodsUtil.CreateFoodAsync(food);

            var afterUpdateFood = await repository.FirstOrDefaultAsync<Foods>(i => i.Name.Equals("mike")).ConfigureAwait(false);
            
            afterUpdateFood?.Color.ShouldBe("white");
            afterUpdateFood?.Name.ShouldBe("mike");
        });
    }

    [Fact]
    public async Task CanUpdateFood()
    {
        await RunWithUnitOfWork<IRepository>(async repository =>
            await repository.InsertAsync<Foods>(new Foods { Id = 11, Name = "cake", Color = "red" }).ConfigureAwait(false));

        var food = new UpdateFoodDto { Id = 11, Name = "mike", Color = "white" };

        var beforeUpdateFood = await Run<IRepository, Foods>(async repository =>
            await repository.GetByIdAsync<Foods>(11).ConfigureAwait(false));

        beforeUpdateFood.Id.ShouldBe(11);
        beforeUpdateFood.Name.ShouldBe("cake");
        beforeUpdateFood.Color.ShouldBe("red");

        await _foodsUtil.UpdateFoodAsync(food);

        var afterUpdateFood = await Run<IRepository, Foods>(async repository =>
            await repository.GetByIdAsync<Foods>(food.Id).ConfigureAwait(false));

        afterUpdateFood.Id.ShouldBe(11);
        afterUpdateFood.Name.ShouldBe("mike");
        afterUpdateFood.Color.ShouldBe("white");
    }

    [Fact]
    public async Task CanDeleteFood()
    {
        await RunWithUnitOfWork<IRepository>(async repository =>
            await repository.InsertAsync<Foods>(new Foods { Id = 11, Name = "cake", Color = "red" }).ConfigureAwait(false));
        
        var food = new DeleteFoodDto() { Id = 11 };

        var beforeDeleteFood = await Run<IRepository, Foods>(async respository =>
            await respository.GetByIdAsync<Foods>(food.Id));

        beforeDeleteFood.Id.ShouldBe(11);

        await _foodsUtil.DeleteFoodAsync(food);

        var afterDeleteFood = await Run<IRepository, Foods>(async respository =>
            await respository.GetByIdAsync<Foods>(food.Id));

        afterDeleteFood.ShouldBeNull();
    }

    [Fact]
    public async Task CanGetFood()
    {
        await RunWithUnitOfWork<IRepository>(async repository =>
            await repository.InsertAsync<Foods>(new Foods { Id = 11, Name = "cake", Color = "red" }).ConfigureAwait(false));

        var food = new GetFoodDto { Id = 11 };

        var getFood = await _foodsUtil.GetFoodAsync(food);

        getFood.Result.Id.ShouldBe(11);
        getFood.Result.Name.ShouldBe("cake");
        getFood.Result.Color.ShouldBe("red");
    }
}
```
## 四、学习网址  
[文章](https://blog.csdn.net/weixin_56331124/article/details/132740470)  
[文章](https://blog.csdn.net/fengershishe/article/details/133692240)  
[文章](https://foreval.cn/archives/ruan-jian-ce-shi-de-si-da-fa-bao-)  
