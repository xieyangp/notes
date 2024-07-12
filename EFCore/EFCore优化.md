# EFCore 优化案例
### 一、查看EFCore转译的sql语句

#### 1.使用 Entity Framework Core

#####  1.1 使用 ToQueryString() 方法
```C#
/* Entity Framework Core 5.0 及以上版本提供了 ToQueryString() 方法，可以直接获取 LINQ 查询生成的 SQL 语句。*/
using (var context = new YourDbContext())
{
    var query = context.YourDbSet.Where(x => x.SomeProperty == "SomeValue");
    var sql = query.ToQueryString();
    Console.WriteLine(sql);
}
```
##### 1.2 使用日志记录 

##### 1.2.1 在 OnConfiguring 方法中配置日志记录：
```C#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseSqlServer("YourConnectionString")
        .LogTo(Console.WriteLine, LogLevel.Information);
}
```

##### 1.2.2 或者在 ASP.NET Core 应用中，通过配置 appsettings.json  
```C#
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### 2 使用 Entity Framework 6.x

#### 2.1 使用 ToString() 方法
```C#
using (var context = new YourDbContext())
{
    var query = context.YourDbSet.Where(x => x.SomeProperty == "SomeValue");
    var sql = query.ToString();
    Console.WriteLine(sql);
}
```

#### 2.2 使用数据库日志
```C#
/*通过设置 DbContext 的 Database.Log 属性来记录生成的 SQL 语句。*/
public class YourDbContext : DbContext
{
    public YourDbContext()
    {
        this.Database.Log = Console.WriteLine;
    }

    public DbSet<YourEntity> YourDbSet { get; set; }
}
```
