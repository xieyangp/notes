依赖注入
====
## 一.  概念
####        1. 控制反转 ( Inversion Of Control IOC )是设计模式中的一种思想；它的目的就是把“创建对象和组装对象”操作的控制权从业务逻辑的代码中转移到框架中，这样的业务代码中只要说明“我需要某个类型的对象”，框架就会帮助我们创建这个对象。
    private readonly IUserDataProvider _userDataProvider;

    public UserService( IUserDataProvider userDataProvider)
    {
        this._userDataProvider = userDataProvider;
    }     
####      2. 控制反转这种思想实现有两种方式：
            a. 服务定位器 ( service locator )；
            b. 依赖注入 ( DI )；
####      3. 控制反转中两个重要的部分：
            a. "容器":负责提供对象的注册和获取功能的框架
            b. "服务":注册到容器中的对象
            总结：控制反转就是把"我创建对象",变成"我要对象"。
##    二、基本使用
####    依赖注入框架中注册服务之前要知道一个重要的概念叫作"生命周期"，简单理解就是获取服务的时候是在创建一个新对象还是用之前的对象。
####    生命周期：
        1. 瞬态 ( transient ):每次被请求的被请求的时候都会创建一个新的对象。优点：避免多段代码用于同一个对象而造成对象状态混乱；缺点：生成的对象多，容易浪费内存。（谨慎使用）
        2. 范围( scoped ):在给定的范围，多次请求共享同一个服务对象，服务每次被请求的时候都会返回同一个对象；在不同范围内，服务每次被请求的时候会返回不同的对象。范围可以框架定义，也可以自己定义；在ASP.NET Core中，服务范围默认是一次HTTP请求，也就是在同一次HTTP请求中，不同的注入会获得同一个对象。适用于在同一范围内共享同一个对象的情况。
        3. 单例 ( singleton ):全局共享一个服务对象。适用于服务无状态对象。
####        总结：服务对象无状态用单例（singleton），一个对象有状态且在框架环境中有范围控制用范围（scoped）；瞬态（transient）尽量在子范围中使用它们，而不要在跟范围中使用它们，控制不好容易造成内存泄漏；
####    线程与进程：https://blog.csdn.net/mu_wind/article/details/124616643?ops_request_misc=%257B%2522request%255Fid%2522%253A%2522169906756716800180649256%2522%252C%2522scm%2522%253A%252220140713.130102334..%2522%257D&request_id=169906756716800180649256&biz_id=0&utm_medium=distribute.pc_search_result.none-task-blog-2~all~top_positive~default-1-124616643-null-null.142^v96^pc_search_result_base9&utm_term=%E7%BA%BF%E7%A8%8B%E5%92%8C%E8%BF%9B%E7%A8%8B%E7%9A%84%E5%8C%BA%E5%88%AB&spm=1018.2226.3001.4187
####    配置Autofac
####    一、引用Autofac所需要的包：
        Autofac包:提供依赖注入的功能；
        Autofac.Extensions.DependencyInjection包：提供将默认依赖注入的容器替换为Autofac的功能。
####    二、注入服务：
####    1.单例注入

     public static void Main(string[] args)  
       {  
           var configuration = new ConfigurationBuilder()  
              .AddJsonFile("appsettings.json")  
              .AddEnvironmentVariables()  
              .Build();  
           CreateHostBuilder(args).Build().Run(); //如果没有改代码，启动立马就会停止
      }  
    public static IHostBuilder CreateHostBuilder(string[] args) =>  
          Host.CreateDefaultBuilder(args)  
          .UseServiceProviderFactory(new AutofacServiceProviderFactory())//注册一个AutofacServiceProviderFactory,这是Autofac容器的服务提供工厂，用于管理依赖注入
          .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });  
          .ConfigureContainer<ContainerBuilder>(builder =>  builder.RegisterType<HelloWordService>().As<IHelloWordService>()); //将服务注册到容器
####    2.多例注入
#####        a.写一个服务接口
![接口服务](https://github.com/xieyangp/notes/blob/main/image/autofac/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-12%20211653.png)
#####        b.需要注入的服务继承服务接口
![继承服务](https://github.com/xieyangp/notes/blob/main/image/autofac/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-12%20212840.png)
#####        c.写一个Module类继承module，Module是一个包含了注册组件和服务的逻辑的类。通过继承Autofac的Module类并重写其中的Load方法，可以定义组件的注册逻辑。在Load方法中，可以使用builder对象来注册各种服务、组件、接口和其他依赖项。
![module](https://github.com/xieyangp/notes/blob/main/image/autofac/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-12%20212009.png)
