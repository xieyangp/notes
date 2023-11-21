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
#### [模型驱动开发](https://github.com/xieyangp/notes/blob/main/Dbup.md) 

#### DatabaseFacade
```
    DatabaseFacade是Entity Framework Core中的一类，它提供了对数据库连接和交互的访问。它允许开发人员在应用程序中执行各种数据库操作，如执行原始SQL查询、执行存储过程、管理等事务。DatabaseFacade还提供了对数据库连接状态的管理和监控，以及对数据库架构和元数据的访问。

    DatabaseFacade，开发人员可以在应用程序中直接访问和操作数据库，而不必直接依赖于特定的数据库提供程序或连接器。这使得应用程序的数据库交互变得更加灵活和可扩展，同时也提供了更多多种控制和性能优化的可能性。
    
    总之，DatabaseFacade提供了对数据库连接和操作的统一访问接口，使得开发人员可以更方便地管理和执行数据库相关的任务。
```
## 四、学习网站：
[EF Core官网](https://learn.microsoft.com/zh-cn/ef/core/)   
[微软数据库提供程序网址](https://learn.microsoft.com/zh-cn/ef/core/providers/?tabs=dotnet-core-clia)
