EF Core
====
## 一、EF Core一些概念：
```
    对象关系映射(ORM):ORM框架可以将数据库表中的数据映射为对象，使开发人员能够使用面向对象的编程语言（如Java、C#等）来操作数据库，而不需要直接编写SQL语句。
ORM框架通常提供了一种简单的方式来执行数据库查询、插入、更新和删除操作，同时隐藏了底层数据库操作的细节。

    实体(Entity)：在EF Core中，实体代表了数据库中的表或集合的模型。每个实体对应数据库中的一条记录。

    上下文（Context）：上下文是应用程序与数据库之间的桥梁，它包含了实体的集合以及用于配置和执行对数据库的操作的方法。

    映射（Mapping）：映射定义了实体类和数据库表之间的映射关系，包括字段名、数据类型、关联关系等。

    查询（Query）：EF Core提供了丰富的查询功能，可以使用LINQ或原生SQL语句来进行数据查询。

    变更跟踪（Change Tracking）：EF Core能够跟踪实体对象的变更，并将这些变更同步到数据库中。

    数据迁移（Data Migration）：EF Core支持数据迁移功能，可以通过代码来管理数据库结构的变更，而不需要手动编写SQL脚本。
```

## 二、EF Core 配置

###   1、引入基础包Microsoft.EntityFrameworkCore、根据你使用的数据库类型引用对应的包，这里我使用Mysql则引用Pomelo.EntityFrameworkCore.MySql。  
        
###   2、创建一个Context类，继承DbContext，重写OnConfiguring(配置数据库)和OnModelCreating(配置实体)方法；
```C#
public class PratiseForJohnnyDbContext : DbContext, IUnitOfWork
{
    //通过依赖注入数据库链接
    private readonly ConnectionString _connectionString;

    public PratiseForJohnnyDbContext(ConnectionString connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(_connectionString.Value,new MySqlServerVersion(new Version(8,2,0)));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        typeof(PratiseForJohnnyDbContext).GetTypeInfo().Assembly.GetTypes()
            .Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsClass).ToList()
            .ForEach(x =>
            {
                if (modelBuilder.Model.FindEntityType(x) == null)
                    modelBuilder.Model.AddEntityType(x);
            });
    }
}

//配置文件配置依赖注入，
//一、创建一个配置接口；
public interface IConfiguartionSetting
{
}

public interface IConfiguartionSetting<TValue> : IConfiguartionSetting
{
    TValue Value { get; set; }
}

//二、创建一个类实现接口；
 public class ConnectionString : IConfiguartionSetting<string>
{
    public string Value { get; set; }
            
    public ConnectionString(IConfiguration configuration)
    {
       Value = configuration.GetConnectionString("Default");
    }
}

//三、在Module注册
private void RegisterSettings(ContainerBuilder builder)
{
    var settingTypes = typeof(PratiseForJohnnyDbContext).Assembly.GetTypes()
    .Where(t => t.IsClass && typeof(IConfiguartionSetting).IsAssignableFrom(t))
    .ToArray();
        
     builder.RegisterTypes(settingTypes).AsSelf().SingleInstance();
}
```
###   3、创建实体、并且在context注册实体
```C#
//创建实体
[Table("Foods")]
public class Foods : IEntity
{
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Color { get; set; }
}

//注册实体
 protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        typeof(PratiseForJohnnyDbContext).GetTypeInfo().Assembly.GetTypes()
            .Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsClass).ToList()
            .ForEach(x =>
            {
                if (modelBuilder.Model.FindEntityType(x) == null)
                    modelBuilder.Model.AddEntityType(x);
            });
    }
```
###   4、根据项目是数据驱动开发还是模型驱动开发，选择读取数据库或数据迁移
#### 数据驱动
```C#
```
####模型驱动开发
```C#
```

### Repository仓储模式的优缺点：
  优点：
  
  1、CRUD达到了高度复用（把一些公共的调用数据库的方法剥离出来，减少冗余的代码）。
  
  2、为业务开发（程序开发）提供统一的规范。大家编码都规范了，都按照标准的作业模式让仓库中放存放(编写代码)自己的东西了，用到的时候大家都可以互相借用（共用）。
  
  缺点：    
  
  1、多个Repository之间怎么保存在一个事务单元内的操作？    
##  UnitOfWork工作单元模式
  1、工作单元作用：跨多个请求的业务，统一管理事务，统一提交。  
  2、我们经常的代码都是分层的，有可能到处都在 new DbContext(options)，这是就要面对如何管理这些DbContext，在AspNetCore中 services.AddDbContext<>默认是用的Scope的作用域，也就是每次HttpRequest，比以前好了很多。但是事务这些管理还是很麻烦。  
  ![unitwork工作原理图](https://github.com/xieyangp/notes/blob/main/image/EFCore/unitwork.png)  
  如上图 有一个Action需要调用很多Service 然后 Service之间又相互调用，在开启Action时 其实是想开启一个事务，但是某些内部代码有可能自己去开启了事务。相互之间调用管理起来非常麻烦。经常出现不可估计的问题。如果有一个集中管理的地方就好很多。比如在Action这里启动一个工作单元，后续所有的业务都使用同一个事务 和 DbContext，这才是我们的预期的。  
  3、如何使用工作单元
  

## DatabaseFacade
```
    DatabaseFacade是Entity Framework Core中的一类，它提供了对数据库连接和交互的访问。它允许开发人员在应用程序中执行各种数据库操作，如执行原始SQL查询、执行存储过程、管理等事务。DatabaseFacade还提供了对数据库连接状态的管理和监控，以及对数据库架构和元数据的访问。

    DatabaseFacade，开发人员可以在应用程序中直接访问和操作数据库，而不必直接依赖于特定的数据库提供程序或连接器。这使得应用程序的数据库交互变得更加灵活和可扩展，同时也提供了更多多种控制和性能优化的可能性。
    
    总之，DatabaseFacade提供了对数据库连接和操作的统一访问接口，使得开发人员可以更方便地管理和执行数据库相关的任务。
```
[EF Core官网](https://learn.microsoft.com/zh-cn/ef/core/)
[微软数据库提供程序网址](https://learn.microsoft.com/zh-cn/ef/core/providers/?tabs=dotnet-core-clia)
