# C# 基础面试速记

## 1. 值类型 vs 引用类型

### 一句话记忆

> **值类型存值，引用类型存地址。**

| 对比   | 值类型                            | 引用类型                                |
| ---- | ------------------------------ | ----------------------------------- |
| 常见类型 | `int`、`double`、`struct`、`enum` | `class`、`array`、`delegate`、`string` |
| 核心特点 | 复制的是**值**                      | 复制的是**引用**                          |
| 修改变量 | 通常互不影响                         | 可能影响同一个对象                           |
| 典型存储 | 通常涉及 Stack，但不能简单理解为“值类型一定在栈上”  | 对象通常在 Heap                          |

### 面试重点：值类型的“副本”

```csharp
int a = 10;
int b = a;

b = 20;

Console.WriteLine(a); // 10
Console.WriteLine(b); // 20
```

因为：

```text
a = 10
 ↓ 复制值
b = 10

修改 b
 ↓
b = 20

a 还是 10
```

---

## 2. 为什么 string 是引用类型，却表现得像不可变类型？

这是一个**高频面试题**。

### 核心答案

> **“变量改变”和“对象内部状态改变”是两回事。**

`string` 是引用类型，但 `string` 对象本身是**不可变的（Immutable）**。

```csharp
string s = "Hello";

s += " World";
```

看起来像是在修改 `s`，实际上是：

```text
原来的对象：
"Hello"
   ↓
创建新对象
"Hello World"
   ↓
s 指向新对象
```

所以：

```csharp
string a = "Hello";
string b = a;

b += " World";

Console.WriteLine(a); // Hello
Console.WriteLine(b); // Hello World
```

### 记忆

> **string 是引用类型，但对象不可变。修改 string，本质上是创建新对象。**

---

# 3. 装箱（Boxing）与拆箱（Unboxing）

### 一句话记忆

> **装箱：值 → 引用**
> **拆箱：引用 → 值**

```csharp
int x = 10;

// 装箱
object obj = x;

// 拆箱
int y = (int)obj;
```

过程：

```text
int x = 10
   ↓
装箱 Boxing
   ↓
Heap 中创建对象
   ↓
object obj

object obj
   ↓
拆箱 Unboxing
   ↓
int y
```

### 面试注意

拆箱时类型必须匹配：

```csharp
object obj = 10;

int x = (int)obj;      // ✅
long y = (long)obj;    // ❌
```

因为箱子里面装的是 `int`，不是 `long`。

### 记忆口诀

> **装箱进堆，拆箱取值。**

---

# 4. foreach vs for

## foreach

`foreach` 要求集合实现：

```csharp
IEnumerable
```

或者：

```csharp
IEnumerable<T>
```

例如：

```csharp
List<int> list = new List<int> { 1, 2, 3 };

foreach (var item in list)
{
    Console.WriteLine(item);
}
```

### 记忆

> **foreach：遍历集合，关注“拿数据”。**

---

## for

```csharp
for (int i = 0; i < list.Count; i++)
{
    list[i] = list[i] + 1;
}
```

### 记忆

> **for：通过下标访问，适合需要修改元素或控制遍历过程的场景。**

⚠️ 不要简单记成：

> `foreach = 不可变`、`for = 可变`

更准确的说法是：

> **foreach 本身不能通过迭代变量直接修改值类型元素；for 可以通过索引修改集合中的元素。**

---

# 5. string += 为什么循环中可能很慢？

这是另一个高频面试题。

```csharp
string result = "";

for (int i = 0; i < 10000; i++)
{
    result += i;
}
```

`string` 不可变，所以每次 `+=` 都可能创建新的字符串。

例如：

```text
第1次：
"" + "1"
 ↓
"1"

第2次：
"1" + "2"
 ↓
"12"

第3次：
"12" + "3"
 ↓
"123"
```

每一次都需要创建新的字符串，并复制之前的内容。

所以大量循环拼接可能产生：

```text
O(1)
O(2)
O(3)
...
O(n)
```

总成本接近：

> **O(n²)**

数据量大时性能会明显下降。

---

# 6. 什么时候使用 StringBuilder？

### 记忆口诀

> **少量用 `+`，循环用 `StringBuilder`，分隔符用 `Join`。**

| 场景        | 推荐              |
| --------- | --------------- |
| 少量字符串拼接   | `+`             |
| 普通几个变量拼接  | `$""`           |
| 循环大量拼接    | `StringBuilder` 避免产生 O(n²) 的性能问题|
| 有统一分隔符    | `string.Join()` |
| LINQ 集合拼接 | `string.Join()` |
| 复杂、有条件的拼接 | `StringBuilder` |

### 少量拼接

```csharp
string result = firstName + lastName;
```

或者：

```csharp
string result = $"{firstName} {lastName}";
```

这种情况下直接用即可，没必要为了性能到处使用 `StringBuilder`。

---

### 循环大量拼接

```csharp
var sb = new StringBuilder();

for (int i = 0; i < 10000; i++)
{
    sb.Append(i);
}

string result = sb.ToString();
```

核心思想：

```text
string +=

旧字符串
   ↓
创建新字符串
   ↓
复制旧内容
   ↓
添加新内容
   ↓
旧对象等待回收
```

而：

```text
StringBuilder

内部缓冲区
   ↓
不断 Append
   ↓
最后 ToString()
```

所以大量拼接更加高效。

---
