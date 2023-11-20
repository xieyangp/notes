## 一、Dbup：一个数据迁移工具；
## 二、知识点
### 静态脚本提供者： 
WithScript, WithScripts  
#### 工作方式：
1.静态脚本是指直接将脚本文件作为参数传递给DbUp的方法。  
2.这些脚本文件可以是SQL脚本文件，也可以是其他类型的脚本文件。  
3.DbUp会按照指定的顺序执行这些脚本文件，以完成数据库升级的操作。  

### 嵌入式脚本提供者：
WithScriptsEmbeddedInAssembly, WithScriptsEmbeddedInAssemblies
#### 工作方式：
1.嵌入式脚本是指将脚本文件嵌入到程序集中，然后通过程序集来提供脚本。  
2.DbUp会从指定的程序集中提取嵌入式脚本，并按照指定的顺序执行这些脚本。  
### 文件系统脚本提供者：
WithScriptsFromFileSystem
#### 工作方式：
1.文件系统脚本是指将脚本文件存储在文件系统中，然后通过文件系统路径来提供脚本。  
2.DbUp会从指定的文件夹中读取脚本文件，并按照指定的顺序执行这些脚本。  
## 三、使用流程
### 1.引用DbUp的包，分别是dbup-core，dbup-mysql，这里我使用mysql所以引用mysql，如果用其他数据库语言则引用对应的dbup支持包。
### 2.创建一个类，里面配置迁移的一些规定等，如迁移日志、迁移脚步的读取方式等：
```C#
public class DbUpRunner
{
    private readonly string _connectionString;//数据库连接字符串

    public DbUpRunner(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Run()
    {
        EnsureDatabase.For.MySqlDatabase(_connectionString);//确保数据库是否存在，如果不存在则创建新数据库
        
        string outPutDirectory = Path.GetFullPath("../PractiseForJohnny.Core/DbUp");//在程序集中查找dbup

        var upgradeEngine = DeployChanges.To.MySqlDatabase(_connectionString)
            .WithScriptsAndCodeEmbeddedInAssembly(typeof(DbUpRunner).Assembly, s => s.EndsWith(".cs"))
            .WithScriptsFromFileSystem(outPutDirectory, new FileSystemScriptOptions{ IncludeSubDirectories = true, Filter = s => s.EndsWith(".sql") })
            .WithTransaction()//用于在执行脚本时启用事务。它确保在执行脚本期间，如果发生错误，将回滚所有已执行的更改
            .LogToAutodetectedLog()//用于自动检测并配置日志记录器。它根据可用的日志记录库自动选择适当的日志记录器。
            .LogToConsole()//将日志输出到控制台
            .Build();//构建升级引擎。

        var result = upgradeEngine.PerformUpgrade();//PerformUpgrade:用于执行数据库升级。它将执行所有未执行的脚本，并将执行结果返回给result变量。

        if (!result.Successful) throw result.Error;//判断升级是否成功
            
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Success!");
        Console.ResetColor();
    }
}
```
### 3、创建脚本，有两种脚步，一种是sql脚本，一种是代码脚本：
```C#
//sql脚本，创建一个文件后缀名为.sql
create table if not exists foods
(
    Id int not null primary key auto_increment,
    name varchar(50) not null,
    color varchar(10) not null
)charset=utf8mb4;

//代码脚本，创建一个类继承IScript，实现接口后，在ProvideScript的写sql
public class Scripts0002_initial_tables : IScript
{
        public string ProvideScript(Func<IDbCommand> dbCommandFactory)
        {
            return @"create table if not exists Foods
                        ( 
                            Id int not null primary key AUTO_INCREMENT, 
                            name varchar(50) not null,
                            color varchar(100) not null
                        ) charset=utf8mb4";
        }
}
//
```
### 4、在program的main中创建一个dbup类，将我们之前从配置文件中读取的数据库字符串通过构造方法赋值，再执行run方法：
```C#
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
        
        new DbUpRunner(new ConnectionString(configuration).Value).Run();
        
        CreateHostBuilder(args).Build().Run();
    }

```
## 四、迁移之后，可以在数据库工具看到一个额外的表（schemaversions），可以理解为迁移记录表，里面记录了你迁移脚步名和时间，但你在次迁移的时候它会通过这个表过滤已经执行的脚本，避免重复执行：
![数据记录表](https://github.com/xieyangp/notes/blob/main/image/DbUp/%E8%BF%81%E7%A7%BB.png)
## 五、学习网址
[DbUp官网网址](https://dbup.readthedocs.io/en/latest/)
