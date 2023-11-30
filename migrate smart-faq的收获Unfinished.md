# 通过自定义特性(Attribute)使过滤条件更加灵活
   例子：没用自定义特性之前，排序时过滤字段值需要与实体属性名相同，这样容易泄露数据库表，而且更改起来很麻烦，当更改了实体属性名后，前端也需要进行对应的调整；
       使用自定义特性之后，相当于为实体属性名添加了一个别名，过滤字段只要和别名一致也可以进行过滤，当更改实体名后，只要别名没改前端可以保持不变；
## 自定义特性使用流程：
### 1.创建一个类继承System.Attribute
```C#
public class SortColumnAttribute : System.Attribute
{
    public SortColumnAttribute(string sortKey, string sortValue, string[] sortInt = null)
    {
        SortKey = sortKey;
        SortValue = sortValue;
        SortInt = sortInt;
    }

    public string[] SortInt { get; set; }

    public string SortKey { get; set; }

    public string SortValue { get; set; }
}
```
### 2.为实体加上自定义特性
```C#
[Table("user_question")]
public class UserQuestion : IEntity
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [SortColumn("Id", "uid", new[] { "1", "2" })]
    public int Id { get; set; }
}
```
### 3.读取自定义特性值
```C#
 public string? FindSortPropertyNameBySortField(string sortField)//sortField 排序字段
 {
     return typeof(UserQuestion).GetProperties()//GetProperties():获取类的所有属性
         .Where(property => (property.GetCustomAttribute<SortColumnAttribute>()?.SortInt.Where(i => i.Equals(sortField))).Any())//.GetCustomAttribute<SortColumnAttribute>()：获取SortColumnAttribute特性的值
         .Select(property => property)
         .ToList().FirstOrDefault()?.Name;
 }
```
## 在使用特性的构造函数参数有一些限制，以下是一些常见的限制：
### 1.基本数据类型：构造函数参数的类型必须是基本数据类型，比如整数、浮点数、字符串、布尔等。
### 2.枚举类型：构造函数参数可以是枚举类型。
### 3.常量表达式：构造函数参数必须是常量表达式，也就是在编译时就能确定其值的表达式。
### 4.typeof 运算符： typeof 运算符可以用于获取类型。
### 5.数组：构造函数参数可以是数组，但数组元素必须是上述规定的基本数据类型、枚举类型、常量表达式、Type 类型或者其他满足这些条件的类型。
### 6.其他特性类型：构造函数参数可以是其他特性类型。
### 7.不能是动态类型 (dynamic)：特性的构造函数参数不能是动态类型，因为特性的信息在编译时就需要被解析
### 8.不能是泛型类型参数(T)和参数为泛型类型(List<T>等)：特性的构造函数参数不能是泛型类型参数。
- - -
- - -
# 常用属性路由特性
## 1.[Route]特性：用于在控制器或动作方法上指定路由模板。
## 2.[HttpGet], [HttpPost], [HttpPut], [HttpDelete], 等： 用于指定动作方法可以处理的 HTTP 请求类型。
```
使用 HttpGet 的场景：
   读取资源： 当你的操作是读取资源或执行幂等操作（不会修改服务器状态）时，适合使用 HttpGet。例如，获取用户信息、获取博客文章等。
   
   简单的查询： 对于简单的查询，特别是当参数是通过查询字符串传递时，使用 HttpGet 是合适的。例如，通过 URL 查询过滤用户列表。
   
   无副作用： 如果操作对服务器没有副作用，即它不会改变服务器状态，使用 HttpGet 是合适的

使用 HttpPost 的场景：
   创建资源： 当你的操作是创建新资源、提交表单或进行其他可能对服务器状态有影响的操作时，应该使用 HttpPost。例如，创建新用户、提交订单等。
   
   修改服务器状态： 当操作对服务器状态产生更复杂的影响，比如修改资源、执行一系列操作等，应该使用 HttpPost。
   
   传递敏感数据： 当操作需要传递敏感数据时，使用 HttpPost 更安全。因为 HttpGet 请求的参数会附加在 URL 中，而 HttpPost 请求的参数在请求体中，相对更安全。
   
   处理复杂的表单： 当你有一个复杂的表单，包含大量数据或文件上传等，使用 HttpPost 是更合适的选择。
```
## 3.[FromQuery], [FromRoute], [FromBody], [FromHeader] 等： 用于从请求的不同部分获取参数。
```
a、[FromQuery] 特性：作用：从查询字符串中获取参数值，通常用于 GET 请求。

b、[FromRoute] 特性：作用：从路由中获取参数值。

c、[FromBody] 特性：作用：从请求体中获取参数值，通常用于 POST 或 PUT 请求。

d、[FromHeader] 特性：作用：从 HTTP 头部中获取参数值。
```
## 4.[ApiController] 特性： 用于标识控制器是一个 Web API 控制器，它会对控制器行为的行为进行一些默认配置。
## 5.[AllowAnonymous] 特性： 允许匿名访问，即不需要身份验证。
## 6.[Produces] 特性： 用于指定动作方法的响应类型。
- - -
- - -
# partial 关键字
## 用于声明一个分部类（Partial Class）。分部类允许将一个类的定义分散到多个文件中，而这些文件会在编译时合并为一个单独的类。
