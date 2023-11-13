autofac知识点
====
#### 装配扫描：  
#####  RegisterAssemblyTypes(程序集)  
  RegisterAssemblyTypes(typeof(IService.IService).Assembly)：    
  这段代码 typeof(IService.IService).Assembly 的意思是获取 IService 接口所在的程序集。typeof(IService.IService) 返回 IService 接口的类型对象，然后通过 .Assembly 获取该类型所在的程序集。通常这样的操作用于在Autofac中使用 RegisterAssemblyTypes 方法来批量注册程序集中的类型。
#####  IsAssignableFrom(参数对象类型)
  typeof(IScopedService).IsAssignableFrom(type)：
  这段代码判断type类型是否与IScopedService一致，一致为true，否则为false
