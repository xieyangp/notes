autofac知识点
====
#### 装配扫描：  
#####  RegisterAssemblyTypes(程序集)  
  RegisterAssemblyTypes(typeof(IService.IService).Assembly)：    
  这段代码 typeof(IService.IService).Assembly 的意思是获取 IService 接口所在的程序集。typeof(IService.IService) 返回 IService 接口的类型对象，然后通过 .Assembly 获取该类型所在的程序集。通常这样的操作用于在Autofac中使用 RegisterAssemblyTypes 方法来批量注册程序集中的类型。
#####  IsAssignableFrom(参数对象类)
  typeof(a).IsAssignableFrom(c)：
  为true满足以下任一条件：

  c和当前实例代表相同的类型。
  
  c直接或间接从当前实例派生。c如果继承自当前实例，则直接从当前实例派生；c如果当前实例继承自一系列从当前实例继承的一个或多个类，则它是从当前实例间接派生的。
  
  当前实例是一个c实现的接口。
  
  c是泛型类型参数，当前实例代表 的约束之一c。
  
  c表示值类型，当前实例表示Nullable<c>(Nullable(Of c)在 Visual Basic 中)。

