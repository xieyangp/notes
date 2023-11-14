Mediator
====
##  Mediator思想：通过封装对象之间的通信，从而减少通信对象之间的依赖性，从而减少耦合。

##  可以解决的问题：
        1.避免一组交互对象之间的紧密耦合。
        2.应该可以独立地改变一组对象之间的交互。
        3.一组对象定义良好但是需要复杂的方式进行通信，产生的依赖关系导致结构混乱。
        4.一个对象引用其它很多对象，并且直接通信，导致难以复用该对象。
        5.想定制一个分布在多个类中的行为，而不想依赖太多子类。
##  Mediator原理图：
![工作原理图]([https://github.com/xieyangp/notes/blob/main/image/Mediator/Mediator.png](https://github.com/xieyangp/notes/blob/main/image/Mediator/mediator1.png))
##  图解：
        Mediator：定义一个接口，用于其它Colleague进行通信。
        Colleague：同事接口，与其它同事通信时依靠Mediator进行通信。
        ConcreteMediator：具体中介者，了解并维护各个Colleague，协调它们的通信行为。

Mediator.Net.Unity 这个包提供了与Unity依赖注入容器的集成，允许您在使用Mediator.Net中介者库时，将中介者和处理程序与Unity容器进行集成。这样可以利用Unity容器的功能来管理中介者和处理程序的生命周期和依赖注入。

Mediator.Net.StructureMap 这个包提供了与StructureMap依赖注入容器的集成，允许您在使用Mediator.Net中介者库时，将中介者和处理程序与StructureMap容器进行集成。这样可以利用StructureMap容器的功能来管理中介者和处理程序的生命周期和依赖注入。

Mediator.Net.Uni 这个包提供了一种与不同依赖注入容器集成的通用方法，允许您在使用Mediator.Net中介者库时，将中介者和处理程序与不同的依赖注入容器进行集成。这样可以根据具体的情况选择合适的依赖注入容器，并将其与Mediator.Net集成起来。

总的来说，这些扩展包为Mediator.Net中介者库提供了更多的灵活性和可扩展性，使其可以与不同的依赖注入容器进行集成，以满足不同项目的需求。


ConfigureAwait(false)：是告诉程序在异步操作完成后是否需要返回到原先的上下文中继续执行。在这种情况下，传入false表示不需要返回到原先的上下文中，可以在任何上下文中继续执行后续操作。这对于避免死锁和提高性能非常有用。
