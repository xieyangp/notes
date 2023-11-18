MiddleWare使用步骤
====
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
2.添加规范类
```C#

```
