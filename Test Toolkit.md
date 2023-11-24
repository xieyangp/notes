## xUnit.net：是一个用于编写和运行单元测试的开源测试框架。  
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
## Shouldly：一个断言库。

## NSubstitute
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
    Returns()和 的函数ReturnsForAnyArgs()的类型为Func<CallInfo,T>，其中T是调用返回的类型，并且CallInfo是提供对调用所用参数的访问的类型  
    T Arg<T>()T：获取传递给此调用的参数类型。  
    T ArgAt<T>(int position)：获取在指定的从零开始的位置传递给此调用的参数，并将其转换为类型T。
