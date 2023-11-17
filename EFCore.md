EF Core
====
## 一、引入基础包Microsoft.EntityFrameworkCore、根据你使用的数据库类型引用对应的包，这里我使用Mysql则引用Pomelo.EntityFrameworkCore.MySql。  

[!微软数据库提供程序网址](https://learn.microsoft.com/zh-cn/ef/core/providers/?tabs=dotnet-core-clia)

## 二、DbContext的


## Repository仓储模式的优缺点：
  优点：
  
  1、CRUD达到了高度复用（把一些公共的调用数据库的方法剥离出来，减少冗余的代码）。
  
  2、为业务开发（程序开发）提供统一的规范。大家编码都规范了，都按照标准的作业模式让仓库中放存放(编写代码)自己的东西了，用到的时候大家都可以互相借用（共用）。
  
  缺点：    
  
  1、多个Repository之间怎么保存在一个事务单元内的操作？    
##  UnitOfWork工作单元模式
  1、工作单元作用：跨多个请求的业务，统一管理事务，统一提交。  
  2、我们经常的代码都是分层的，有可能到处都在 new DbContext(options)，这是就要面对如何管理这些DbContext，在AspNetCore中 services.AddDbContext<>默认是用的Scope的作用域，也就是每次HttpRequest，比以前好了很多。但是事务这些管理还是很麻烦。  
  [!unitwork工作原理图]()  
  如上图 有一个Action需要调用很多Service 然后 Service之间又相互调用，在开启Action时 其实是想开启一个事务，但是某些内部代码有可能自己去开启了事务。相互之间调用管理起来非常麻烦。经常出现不可估计的问题。如果有一个集中管理的地方就好很多。比如在Action这里启动一个工作单元，后续所有的业务都使用同一个事务 和 DbContext，这才是我们的预期的。  
  3、如何使用工作单元
  
## UnifyResponseSpacification
```C#
//这段代码是一个实现了IPipeSpecification接口的泛型类UnifyResponseSpecification<TContext>。该类用于统一处理响应，包括在执行前、执行中、执行后以及出现异常时的处理。
public class UnifyResponseSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : IContext<IMessage>
    {
        //确定是否应该执行响应处理。在这个实现中，始终返回true，即始终执行响应处理。
        public bool ShouldExecute(TContext context, CancellationToken cancellationToken)
        {
            return true;
        }
        //在执行之前调用
        public Task BeforeExecute(TContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        //执行响应处理
        public Task Execute(TContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        //执行后调用,调用EnrichResponse方法来丰富响应信息，然后返回一个已完成的Task。
        public Task AfterExecute(TContext context, CancellationToken cancellationToken)
        {
            EnrichResponse(context, null);

            return Task.CompletedTask;
        }
        //出现异常时调用,同样调用EnrichResponse方法来丰富响应信息
        public Task OnException(Exception ex, TContext context)
        {
            EnrichResponse(context, ex);

            ExceptionDispatchInfo.Capture(ex).Throw();

            throw ex;
        }
        //EnrichResponse方法用于丰富响应信息，根据是否出现异常来设置响应的状态码和消息。如果未出现异常，则将状态码设置为OK，消息设置为OK；如果出现异常，则将状态码设置为InternalServerError，消息设置为异常的消息。
        private void EnrichResponse(TContext context, Exception ex)
        {
            if (!ShouldExecute(context, default) || context.Result is not CommonResponse) return;

            var response = (dynamic)context.Result;

            if (ex == null)
            {
                response.Code = HttpStatusCode.OK;
                response.Msg = nameof(HttpStatusCode.OK).ToLower();
            }
            else
            {
                response.Code = HttpStatusCode.InternalServerError;
                response.Msg = ex.Message;
            }
        }

```
