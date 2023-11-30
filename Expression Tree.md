## 在 C# 中，表达树（Expression Tree）是一种用于在运行时表示代码的数据结构。表达树是一个树状结构，其中每个节点表示代码元素，如操作数、运算符、成员访问、方法调用等。这种树形结构允许在运行时动态创建、分析和修改代码。

## 表达树的主要概念：
### 1.Expression 类型： 表达树的节点类型是 Expression 类型。System.Linq.Expressions 命名空间提供了一组用于表示表达式的类，例如 BinaryExpression、UnaryExpression、MemberExpression 等。

### 2.Lambda 表达式： 表达树通常用于表示 Lambda 表达式，这是一种用于创建匿名函数的语法。Lambda 表达式可以被表示为 Expression<Func<T, TResult>>，其中 T 是输入参数的类型，TResult 是返回值的类型。
```c#
Expression<Func<int, int, int>> add = (a, b) => a + b;
```
### 3.表达树的节点： 表达树的每个节点表示代码的一个部分。例如，对于二元运算 a + b，它可以表示为一个 BinaryExpression 节点。
```c#
BinaryExpression binaryExpression = (BinaryExpression)add.Body;
```
### 4.构建表达树： 表达树可以通过手动构建或通过解析 Lambda 表达式创建。通过手动构建表达树，你可以创建一个动态的代码结构。
```C#
ParameterExpression a = Expression.Parameter(typeof(int), "a");
ParameterExpression b = Expression.Parameter(typeof(int), "b");
BinaryExpression addExpression = Expression.Add(a, b);
Expression<Func<int, int, int>> add = Expression.Lambda<Func<int, int, int>>(addExpression, a, b);
```
### 5.分析表达树： 通过分析表达树，可以了解代码的结构、类型和逻辑。这对于编写通用查询、动态代码生成等场景非常有用。
```C#
// 获取 Lambda 表达式的参数和主体
ParameterExpression parameter = (ParameterExpression)add.Parameters[0];
BinaryExpression body = (BinaryExpression)add.Body;
```
### 6.执行表达树： 表达树可以被编译为委托，然后在运行时执行。
```C#
Func<int, int, int> addDelegate = add.Compile();
int result = addDelegate(2, 3); // 结果为 5
```

## 以下是一个简单的示例，演示如何使用表达树表示和执行一个简单的加法操作：
```C#
using System;
using System.Linq.Expressions;

class Program
{
    static void Main()
    {
        // Lambda 表达式表示 a + b
        Expression<Func<int, int, int>> add = (a, b) => a + b;

        // 获取 Lambda 表达式的参数和主体
        ParameterExpression parameterA = (ParameterExpression)add.Parameters[0];
        ParameterExpression parameterB = (ParameterExpression)add.Parameters[1];
        BinaryExpression body = (BinaryExpression)add.Body;

        // 编译表达式为委托
        Func<int, int, int> addDelegate = add.Compile();

        // 执行委托
        int result = addDelegate(2, 3);

        Console.WriteLine($"Expression: {parameterA} + {parameterB}");
        Console.WriteLine($"Compiled Delegate Result: {result}");
    }
}
```
