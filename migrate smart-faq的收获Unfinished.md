## 通过自定义特性(Attribute)使过滤条件更加灵活
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
