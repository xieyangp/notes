# UnitOfWork工作单元模式
##  1、工作单元作用：跨多个请求的业务，统一管理事务，统一提交。  
##  2、我们经常的代码都是分层的，有可能到处都在 new DbContext(options)，这是就要面对如何管理这些DbContext，在AspNetCore中 services.AddDbContext<>默认是用的Scope的作用域，也就是每次HttpRequest，比以前好了很多。但是事务这些管理还是很麻烦。  
  ![unitwork工作原理图](https://github.com/xieyangp/notes/blob/main/image/EFCore/unitwork.png)  
### 如上图 有一个Action需要调用很多Service 然后 Service之间又相互调用，在开启Action时 其实是想开启一个事务，但是某些内部代码有可能自己去开启了事务。相互之间调用管理起来非常麻烦。经常出现不可估计的问题。如果有一个集中管理的地方就好很多。比如在Action这里启动一个工### 作单元，后续所有的业务都使用同一个事务 和 DbContext，这才是我们的预期的。  
##  3、如何使用工作单元
