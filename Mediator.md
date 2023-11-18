Mediator
====
##  Mediator思想：通过封装对象之间的通信，从而减少通信对象之间的依赖性，从而减少耦合。

##  可以解决的问题：
        1.避免一组交互对象之间的紧密耦合。
        2.应该可以独立地改变一组对象之间的交互。
        3.一组对象定义良好但是需要复杂的方式进行通信，产生的依赖关系导致结构混乱。
        4.一个对象引用其它很多对象，并且直接通信，导致难以复用该对象。
        5.想定制一个分布在多个类中的行为，而不想依赖太多子类。
##  Mediator管道图：
![工作原理图](https://github.com/xieyangp/notes/blob/main/image/Mediator/mediator1.png)
##  管道作用：
        Mediator：发送命令至GlobalReceivePipe；
        GlobalReceivePipe: 每当消息在到达下一个管道和处理程序之前发送、发布或请求时，都会触发此管道；
        CommandReceivePipe: 接收Icommand接口的命令至处理程序(CommandHandler);我们用于发送增删改命令
        RequesteReceivePipe: 接收IRequest接口的命令至处理程序(RequestHandler);我们用于发送查询命令
        EventReceivePipe: 接收IEvent接口的命令至处理程序中(EventHandler);
        Handler: 处理程序，处理各种命令，以及发布消息；
        publishPipe: 接收IEvent接口的事件，我们通常用于处理后续事件或命令，没有则结束；  
##  Mediator配置步骤：
        一、引用Mediator.Net与Mediator.Net.Autofac包；Mediator.Net包作用：提供了一种中介者模式的实现；提供中介者模式封装的接口和方法、类，如消息传递、事件处理、命令调度；Mediator.Net.Autofac的作用:提供依赖注入；1.注册中介者和处理程序；2.中介者和处理程序的解析；3.中介者和处理程序的生命周期管理；
        二、在Module中注册Mediator：
```C#
 private void RegisterMediator(ContainerBuilder builder)
    {
        var mediatorBuidler = new MediatorBuilder();

        mediatorBuidler.RegisterHandlers(_assemblies);
0
        builder.RegisterMediator(mediatorBuidler);
    }
```
##  Mediator的使用步骤：
###  1.创建命令、请求，如果有响应则创建对应的响应
```C#
        //创建命令
        public class CreateFoodCommand : ICommand
        {
            public CreateFoodDto Food { get; set; }
        }
        //创建命令的响应
        public class CreateFoodResponse : IResponse
        {
            public string Result { get; set; }
        }

        //创建请求
        public class PongResponse : IResponse
        {
            public string Message { get; set; }
        }
```
###  2.发布命令：发布命令的方法有如下几种：
```C#
        await _mediator.SendAsync(new TestBaseCommand(Guid.NewGuid()));//无响应命令
        await _mediator.SendAsync<Command,Response>(command)//有响应命令；Command:ICommand类型的命令类型，Response：IRsponse类型的结果类型；command：ICommand类型的命令实体；
        await _mediator.RequestAsync<Request, Response>(new GetGuidRequest(_guid));//有响应请求；Request:IRequest类型的命令类型，Response：IRsponse类型的结果类型，Request:IRequest类型的命令实体；
        await _mediator.Publish(Event);//发布事件；Event：继续IEvent的事件实体；
```
###  3.创建Handler
```C#
        //创建命令Handler，CreateFoodCommand：命令类型；CreateFoodResponse：响应类型；
        public class CreateFoodCommandHandler : ICommandHandler<CreateFoodCommand, CreateFoodResponse>
        {
            private readonly IFoodService _foodService;
        
            public CreateFoodCommandHandler(IFoodService foodService)
            {
                _foodService = foodService;
            }
        
            public async Task<CreateFoodResponse> Handle(IReceiveContext<CreateFoodCommand> context, CancellationToken cancellationToken)
            {
                var @event = await _foodService.CreateFoodAsync(context.Message, cancellationToken).ConfigureAwait(false);
                //发布事件
                await context.PublishAsync(@event, cancellationToken).ConfigureAwait(false);
        
                return new CreateFoodResponse
                {
                    Result = @event.Result
                };
            }
        }
```
###  4.创建事件
```C#
        public class CreateFoodEvent : IEvent
        {
            public string Result { get; set; }
        }

        public class CreateFoodEventHandler : IEventHandler<CreateFoodEvent>
        {
            public Task Handle(IReceiveContext<CreateFoodEvent> context, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
```

##  添加中间件
1.添加中间件的静态类
```C#
  public static class UnitOfWorkMiddleWare
  {
      //在管道配置器中添加一个工作单元的规范
      public static void UseUnitOfWork<TContext>(this IPipeConfigurator<TContext> configurator,
          IUnitOfWork unitOfWork = null) where TContext : IContext<IMessage>
      {
          //检查是否提供了工作单元实例以及是否配置了依赖范围。如果都没有提供工作单元实例并且依赖范围也没有配置，则会抛出一个DependencyScopeNotConfiguredException异常。
          if (unitOfWork == null && configurator.DependencyScope == null)
          {
              throw new DependencyScopeNotConfiguredException(
                  $"{nameof(unitOfWork)} is not provided and IDependencyScope is not configured, Please ensure {nameof(unitOfWork)} is registered properly if you are using IoC container, otherwise please pass {nameof(unitOfWork)} as parameter");
          }

          //如果没有提供工作单元实例，则方法会尝试从依赖范围中解析出IUnitOfWork实例。如果依赖范围中没有配置IUnitOfWork实例，则会抛出异常。
          unitOfWork ??= configurator.DependencyScope.Resolve<IUnitOfWork>();

          //管道配置器中的规范中，以便在管道执行过程中使用工作单元
          configurator.AddPipeSpecification(new UnifyOfWorkSpacification<TContext>(unitOfWork));
      }
  }
```
2.添加规范类，在管道中加入中间件时，中间件也要满足管道的一个规范；
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
3.注册中间件
```C#
  private void RegisterMediator(ContainerBuilder builder)
    {
        var mediatorBuidler = new MediatorBuilder();

        mediatorBuidler.RegisterHandlers(_assemblies);

        //注册中间件
        mediatorBuidler.ConfigureGlobalReceivePipe(x =>
        {
            x.UseUnitOfWork();
            x.UseUnifyResponse();
        });

        builder.RegisterMediator(mediatorBuidler);
    }
```
        
Mediator.Net.Unity 这个包提供了与Unity依赖注入容器的集成，允许您在使用Mediator.Net中介者库时，将中介者和处理程序与Unity容器进行集成。这样可以利用Unity容器的功能来管理中介者和处理程序的生命周期和依赖注入。

Mediator.Net.StructureMap 这个包提供了与StructureMap依赖注入容器的集成，允许您在使用Mediator.Net中介者库时，将中介者和处理程序与StructureMap容器进行集成。这样可以利用StructureMap容器的功能来管理中介者和处理程序的生命周期和依赖注入。

Mediator.Net.Uni 这个包提供了一种与不同依赖注入容器集成的通用方法，允许您在使用Mediator.Net中介者库时，将中介者和处理程序与不同的依赖注入容器进行集成。这样可以根据具体的情况选择合适的依赖注入容器，并将其与Mediator.Net集成起来。

总的来说，这些扩展包为Mediator.Net中介者库提供了更多的灵活性和可扩展性，使其可以与不同的依赖注入容器进行集成，以满足不同项目的需求。


ConfigureAwait(false)：是告诉程序在异步操作完成后是否需要返回到原先的上下文中继续执行。在这种情况下，传入false表示不需要返回到原先的上下文中，可以在任何上下文中继续执行后续操作。这对于避免死锁和提高性能非常有用。
