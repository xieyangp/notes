## xUnit.net：是一个用于编写和运行单元测试的开源测试框架。  
[xUnit官网](https://xunit.net/docs/getting-started/netfx/jetbrains-rider)
## xUnit.net 支持两种不同主要类型的单元测试：事实和理论。
```
事实是永远正确的测试。他们测试不变条件。[Fact]

理论是仅适用于一组特定数据的测试。[Theory]
```
## 使用xUnit.net编写一个简单测试：
```C#
public class Tests
{
   [Fact]
   public void PassingTest()
   {
       Assert.Equal(4, Calculator.Add(2, 2));
   }

   [Fact]
   public void FailingTest()
   {
       Assert.Equal(5, Calculator.Add(2, 2));
   }

   [Theory]
   [InlineData(2, 2, 4)]
   [InlineData(3, 3, 6)]
   [InlineData(2, 2, 5)]
   public void MyTheory(int x, int y, int sum)
   {
       Assert.Equal(sum, Calculator.Add(x, y));
   }
}
```
## Shouldly：一个强大的断言库。断言的好处：判断代码的结果是否与预期相同，快速定位错误的地方；
[Shouldly官网](https://xunit.net/docs/getting-started/netfx/jetbrains-rider)   
## 下面是常用的一些方法
### ShouldBe：应该是,如下A应该是B，如果不是则报错
```C#
A.ShouldBe(B)
```
### ShouldBeNull：应该为空，如下A应该为空
```C#
A.ShouldBeNull()
```
### ShouldBeTrue、ShouldBeFalse：应该为真，应该为假，如下A应该为真，B应该为假
```C#
A.ShouldBeTrue()

B.ShouldBeFalse()
```
### ShouldBeOfType：应该属于某类型，如下A应该为B类型
```C#
A.ShouldBeOfType(B)
```
### Shouldly提供了许多的方法，一般每种判断类型有两种，例如ShouldBy，ShouldNotBe,所以在记忆的时候记住'可以'即可；  

### 通常我们在操作(Assert)之后对进行断言，有时候也会在准备(Arrang)之后进行一次断言来判断准备是否准确;
```C#
await RunWithUnitOfWork<IRepository>(async repository =>
   await repository.InsertAsync<Foods>(new Foods { Id = 11, Name = "cake", Color = "red" }).ConfigureAwait(false));

var food = new UpdateFoodDto { Id = 11, Name = "mike", Color = "white" };

var beforeUpdateFood = await Run<IRepository, Foods>(async repository =>
   await repository.GetByIdAsync<Foods>(11).ConfigureAwait(false));

beforeUpdateFood.Id.ShouldBe(11);//判断测试数据是否成功插入
beforeUpdateFood.Name.ShouldBe("cake");
beforeUpdateFood.Color.ShouldBe("red");

await _foodsUtil.UpdateFoodAsync(food);

var afterUpdateFood = await Run<IRepository, Foods>(async repository =>
   await repository.GetByIdAsync<Foods>(food.Id).ConfigureAwait(false));

afterUpdateFood.Id.ShouldBe(11);//判断测试数据是否被更新
afterUpdateFood.Name.ShouldBe("mike");
afterUpdateFood.Color.ShouldBe("white");
```  
## NSubstitute：一个替代库  
[NSubstituteg官网](https://nsubstitute.github.io/help/return-for-args/)   
### 下面是常用的一些方法
###  Substitute.For<Interface>():创建替代品；最好用接口创建，用类创建容易出现一些问题，例如类中任何非虚拟代码都将被执行。
    替代多个接口：Substitute.For<ICommand, IDisposable>()；可以替代多个接口，但是只能实现一个。
    替代委托类型：Substiute.For<T>()。当替换委托类型时，您将无法让替换项实现其他接口或类
### Resturns：设置返回值
    方法设置：calculator.Add(1, 2).Returns(3);每次使用calculator.Add(1, 2)都有一个返回值为3
    属性设置：calculator.Mode.Returns("DEC");每次使用calculator.Mode都为"DEC"
### 参数匹配器：设置返回值 和 检查收到的调用 时可以使用参数匹配器   
    Arg.Any<T>():1.忽略参数，Arg.Any<int>()表示任何数字
    Arg.Is<T>();
   
    注意：arg 匹配器实际上是模糊匹配的；不直接传递给函数调用
    ReturnsForAnyArgs()
## 来电信息：
    Returns()和 的函数ReturnsForAnyArgs()的类型为Func<CallInfo,T>，其中T是调用返回的类型，并且CallInfo是提供对调用所用参数的访问的  类型  
    T Arg<T>()T：获取传递给此调用的参数类型。  
    T ArgAt<T>(int position)：获取在指定的从零开始的位置传递给此调用的参数，并将其转换为类型T。
## 一个例子，mock思路大致是这样，但是一般不会这么测试
```C#
 [Fact]
 public async Task CanUpdateService()
 {
     //创建需要的参数
     var food = new UpdateFoodDto
     {
         Id = 1,
         Name = "cake",
         Color = "red"
     };
     
     var command = new UpdateFoodCommand
     {
         Food = food
     };

     var foods =new Foods
     {   Id = 1,
         Name = "cake",
         Color = "red"
     };
     
     var cancellationToken = new CancellationToken();
     
     //创建方法内引用的其他方法，并设置其返回值
     var mapper = Substitute.For<IMapper>();
     mapper.Map<UpdateFoodDto>(foods).Returns(food);
     
     var foodDataProvider = Substitute.For<IFoodDataProvider>();
     foodDataProvider.UpdateFoodAsync(food, cancellationToken).Returns(Task.FromResult(foods));
   
     //创建要测试的方法所在的类，并将其中需要的引用注册进去
     var foodService = new FoodService(mapper,foodDataProvider);
    
     //测试方法
     var result = await foodService.UpdateFoodAsync(command, cancellationToken);

     //检验是否正确
     result.ShouldBeOfType<FoodUpdatedEvent>();
     result.Result.ShouldBeOfType<UpdateFoodDto>();
     result.Result.Id = 1;
     result.Result.Name = "cake";
     result.Result.Color = "red";
    
     var a = await foodDataProvider.Received().UpdateFoodAsync(food, cancellationToken);
 }
```
