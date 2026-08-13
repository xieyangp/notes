# C# `string` 常用方法大全

> C# `string` 常用方法速查，适合日常开发、面试复习。

---

## 一、String 基础

### 1. string 是什么？

`string` 是 C# 中表示字符串的类型，本质上是：

```csharp
System.String
```

例如：

```csharp
string name = "Johnny";
```

等价于：

```csharp
System.String name = "Johnny";
```

### 2. string 是引用类型

```csharp
string name = "Johnny";
```

`string` 属于引用类型，但它具有一个非常重要的特性：

> **string 是不可变（Immutable）的。**

例如：

```csharp
string str = "Hello";

str += " World";
```

实际上不是修改原来的 `"Hello"`，而是创建了一个新的字符串：

```text
"Hello"
   ↓
"Hello World"
```

所以：

```csharp
string a = "Hello";
string b = a;

b = "World";

Console.WriteLine(a); // Hello
Console.WriteLine(b); // World
```

---

# 二、Length：获取字符串长度

```csharp
string str = "Hello";

int length = str.Length;

Console.WriteLine(length); // 5
```

### 注意

`Length` 是属性，不是方法：

```csharp
str.Length
```

而不是：

```csharp
str.Length()
```

---

# 三、判断字符串是否为空

## 1. string.IsNullOrEmpty()

判断字符串是否：

* `null`
* `""`

```csharp
string str = "";

bool result = string.IsNullOrEmpty(str);

Console.WriteLine(result); // true
```

等价于：

```csharp
str == null || str == ""
```

---

## 2. string.IsNullOrWhiteSpace()

判断字符串是否：

* `null`
* 空字符串 `""`
* 只包含空格
* 只包含制表符
* 只包含换行符等空白字符

```csharp
string str = "   ";

bool result = string.IsNullOrWhiteSpace(str);

Console.WriteLine(result); // true
```

### 推荐

业务开发中通常：

```csharp
string.IsNullOrWhiteSpace(str)
```

比：

```csharp
string.IsNullOrEmpty(str)
```

更严格。

---

# 四、Trim：去除首尾空白

## 1. Trim()

去除字符串**开头和结尾**的空白字符。

```csharp
string str = "  Hello World  ";

string result = str.Trim();

Console.WriteLine(result);
// Hello World
```

---

## 2. TrimStart()

只去除开头空白：

```csharp
string str = "  Hello  ";

string result = str.TrimStart();

Console.WriteLine(result);
// Hello  
```

---

## 3. TrimEnd()

只去除结尾空白：

```csharp
string str = "  Hello  ";

string result = str.TrimEnd();

Console.WriteLine(result);
//   Hello
```

---

## 4. 指定字符 Trim

```csharp
string str = "###Hello###";

string result = str.Trim('#');

Console.WriteLine(result);
// Hello
```

---

# 五、大小写转换

## 1. ToUpper()

转换成大写：

```csharp
string str = "hello";

string result = str.ToUpper();

Console.WriteLine(result);
// HELLO
```

---

## 2. ToLower()

转换成小写：

```csharp
string str = "HELLO";

string result = str.ToLower();

Console.WriteLine(result);
// hello
```

---

## 3. ToUpperInvariant()

使用固定区域性转换成大写：

```csharp
string result = str.ToUpperInvariant();
```

---

## 4. ToLowerInvariant()

使用固定区域性转换成小写：

```csharp
string result = str.ToLowerInvariant();
```

### 推荐

如果是程序内部进行不依赖语言环境的比较、转换：

```csharp
str.ToLowerInvariant();
str.ToUpperInvariant();
```

---

# 六、Contains：是否包含指定字符串

```csharp
string str = "Hello World";

bool result = str.Contains("World");

Console.WriteLine(result);
// true
```

例如：

```csharp
if (name.Contains("John"))
{
    Console.WriteLine("包含 John");
}
```

---

# 七、StartsWith：是否以指定字符串开头

```csharp
string str = "Hello World";

bool result = str.StartsWith("Hello");

Console.WriteLine(result);
// true
```

例如：

```csharp
if (fileName.StartsWith("out-"))
{
    // ...
}
```

---

# 八、EndsWith：是否以指定字符串结尾

```csharp
string str = "Hello.txt";

bool result = str.EndsWith(".txt");

Console.WriteLine(result);
// true
```

常用于判断文件扩展名：

```csharp
if (fileName.EndsWith(".mp3"))
{
    // 音频文件
}
```

---

# 九、IndexOf：查找字符串位置

查找指定字符串第一次出现的位置。

```csharp
string str = "Hello World";

int index = str.IndexOf("World");

Console.WriteLine(index);
// 6
```

字符串下标从 `0` 开始：

```text
H e l l o   W o r l d
0 1 2 3 4 5 6 7 8 9 10
```

如果找不到：

```csharp
int index = str.IndexOf("ABC");

Console.WriteLine(index);
// -1
```

---

# 十、LastIndexOf：查找最后一次出现的位置

```csharp
string str = "Hello World World";

int index = str.LastIndexOf("World");

Console.WriteLine(index);
```

常用于查找最后一个 `/`：

```csharp
string path = "C:/Users/Johnny/test.txt";

int index = path.LastIndexOf("/");
```

---

# 十一、Substring：截取字符串

## 1. Substring(startIndex)

从指定位置截取到结尾：

```csharp
string str = "Hello World";

string result = str.Substring(6);

Console.WriteLine(result);
// World
```

---

## 2. Substring(startIndex, length)

从指定位置开始，截取指定长度：

```csharp
string str = "Hello World";

string result = str.Substring(0, 5);

Console.WriteLine(result);
// Hello
```

### 注意

```csharp
Substring(0, 5)
```

表示：

```text
从 0 开始
取 5 个字符
```

不是结束位置为 `5`。

---

# 十二、Remove：删除字符串

## 1. Remove(startIndex)

从指定位置删除到结尾：

```csharp
string str = "Hello World";

string result = str.Remove(5);

Console.WriteLine(result);
// Hello
```

---

## 2. Remove(startIndex, count)

从指定位置删除指定数量：

```csharp
string str = "Hello World";

string result = str.Remove(5, 1);

Console.WriteLine(result);
// HelloWorld
```

---

# 十三、Insert：插入字符串

```csharp
string str = "Hello World";

string result = str.Insert(5, ",");

Console.WriteLine(result);
// Hello, World
```

例如：

```csharp
string phone = "13812345678";

string result = phone.Insert(3, "-");

Console.WriteLine(result);
// 138-12345678
```

---

# 十四、Replace：替换字符串

## 1. 替换字符串

```csharp
string str = "Hello World";

string result = str.Replace("World", "Johnny");

Console.WriteLine(result);
// Hello Johnny
```

---

## 2. 替换字符

```csharp
string str = "Hello";

string result = str.Replace('l', 'x');

Console.WriteLine(result);
// Hexxo
```

---

## 注意

`Replace()` 不会修改原字符串。

```csharp
string str = "Hello";

str.Replace("Hello", "World");

Console.WriteLine(str);
// Hello
```

正确：

```csharp
str = str.Replace("Hello", "World");
```

因为：

> **string 是不可变的。**

---

# 十五、Split：分割字符串

这是开发中非常常用的方法。

```csharp
string str = "A,B,C";

string[] result = str.Split(',');

foreach (string item in result)
{
    Console.WriteLine(item);
}
```

结果：

```text
A
B
C
```

---

## 使用多个分隔符

```csharp
string str = "A,B;C|D";

string[] result = str.Split(',', ';', '|');
```

得到：

```text
A
B
C
D
```

---

## 按空格分割

```csharp
string str = "Hello World CSharp";

string[] result = str.Split(' ');
```

---

# 十六、Join：拼接字符串数组

`Join()` 和 `Split()` 经常一起使用。

```csharp
string[] names =
{
    "Tom",
    "Jerry",
    "Jack"
};

string result = string.Join(",", names);

Console.WriteLine(result);
// Tom,Jerry,Jack
```

### Split + Join

```csharp
string str = "A,B,C";

string[] arr = str.Split(',');

string result = string.Join("-", arr);

Console.WriteLine(result);
// A-B-C
```

---

# 十七、Concat：拼接字符串

```csharp
string result = string.Concat("Hello", " ", "World");

Console.WriteLine(result);
// Hello World
```

多个字符串：

```csharp
string result = string.Concat("A", "B", "C");

// ABC
```

---

# 十八、字符串拼接

## 1. `+`

```csharp
string name = "Johnny";

string result = "Hello " + name;
```

---

## 2. string.Concat()

```csharp
string result = string.Concat("Hello ", name);
```

---

## 3. string.Join()

```csharp
string result = string.Join(",", "A", "B", "C");

// A,B,C
```

---

## 4. StringBuilder

大量拼接字符串时推荐：

```csharp
StringBuilder sb = new StringBuilder();

sb.Append("Hello");
sb.Append(" ");
sb.Append("World");

string result = sb.ToString();
```

---

# 十九、PadLeft：左侧补字符

```csharp
string str = "123";

string result = str.PadLeft(5, '0');

Console.WriteLine(result);
// 00123
```

常用于格式化数字：

```csharp
string number = "12";

string result = number.PadLeft(5, '0');

// 00012
```

---

# 二十、PadRight：右侧补字符

```csharp
string str = "123";

string result = str.PadRight(5, '0');

Console.WriteLine(result);
// 12300
```

---

# 二十一、Compare：字符串比较

```csharp
int result = string.Compare("A", "B");
```

返回值：

```text
< 0    A < B
= 0    A == B
> 0    A > B
```

例如：

```csharp
int result = string.Compare("abc", "abc");

Console.WriteLine(result);
// 0
```

---

# 二十二、Equals：判断字符串是否相等

## 1. string.Equals()

```csharp
string a = "Hello";
string b = "Hello";

bool result = string.Equals(a, b);

Console.WriteLine(result);
// true
```

---

## 2. 实例 Equals()

```csharp
string a = "Hello";

bool result = a.Equals("Hello");
```

---

# 二十三、==：判断字符串是否相等

```csharp
string a = "Hello";
string b = "Hello";

Console.WriteLine(a == b);
// true
```

对于 `string`：

```csharp
a == b
```

通常表示**字符串内容比较**，而不是简单的引用地址比较。

---

# 二十四、忽略大小写比较

例如：

```csharp
string a = "Hello";
string b = "hello";
```

如果直接：

```csharp
a == b
```

结果：

```text
false
```

可以使用：

```csharp
string.Equals(
    a,
    b,
    StringComparison.OrdinalIgnoreCase
);
```

结果：

```text
true
```

### 推荐写法

```csharp
if (string.Equals(
    input,
    "admin",
    StringComparison.OrdinalIgnoreCase))
{
    // ...
}
```

---

# 二十五、StringComparison

常见：

```csharp
StringComparison.Ordinal
```

区分大小写。

```csharp
StringComparison.OrdinalIgnoreCase
```

忽略大小写。

例如：

```csharp
str.Equals(
    "hello",
    StringComparison.OrdinalIgnoreCase
);
```

### 面试重点

> 程序内部进行字符串比较，通常优先考虑 `Ordinal` / `OrdinalIgnoreCase`。

---

# 二十六、ToCharArray：转换成字符数组

```csharp
string str = "Hello";

char[] chars = str.ToCharArray();
```

结果：

```text
H
e
l
l
o
```

可以：

```csharp
foreach (char c in chars)
{
    Console.WriteLine(c);
}
```

---

# 二十七、Contains + StringComparison

现代 .NET 中可以指定比较规则：

```csharp
string str = "Hello World";

bool result = str.Contains(
    "hello",
    StringComparison.OrdinalIgnoreCase
);
```

结果：

```text
true
```

类似的还有：

```csharp
StartsWith()
EndsWith()
Contains()
```

都可以在适用的重载中指定比较方式。

---

# 二十八、字符访问

可以通过下标访问字符串中的字符：

```csharp
string str = "Hello";

char c = str[0];

Console.WriteLine(c);
// H
```

例如：

```csharp
Console.WriteLine(str[1]);
// e
```

字符串下标：

```text
H e l l o
0 1 2 3 4
```

---

# 二十九、遍历字符串

```csharp
string str = "Hello";

for (int i = 0; i < str.Length; i++)
{
    Console.WriteLine(str[i]);
}
```

或者：

```csharp
foreach (char c in str)
{
    Console.WriteLine(c);
}
```

---

# 三十、Format：字符串格式化

```csharp
string name = "Johnny";
int age = 18;

string result = string.Format(
    "Name: {0}, Age: {1}",
    name,
    age
);

Console.WriteLine(result);
```

结果：

```text
Name: Johnny, Age: 18
```

---

# 三十一、字符串插值 `$`

现在更推荐使用字符串插值：

```csharp
string name = "Johnny";
int age = 18;

string result = $"Name: {name}, Age: {age}";
```

比：

```csharp
string.Format()
```

更加直观。

---

# 三十二、格式化数字

```csharp
double price = 1234.5678;

string result = $"{price:F2}";

Console.WriteLine(result);
// 1234.57
```

常见格式：

```text
F2     保留 2 位小数
N2     千分位 + 2 位小数
C2     货币格式
P2     百分比
```

例如：

```csharp
double price = 1234.5678;

Console.WriteLine($"{price:F2}");
// 1234.57

Console.WriteLine($"{price:N2}");
// 1,234.57
```

---

# 三十三、数字转字符串

```csharp
int number = 123;

string str = number.ToString();
```

格式化：

```csharp
string str = number.ToString("D5");

// 00123
```

---

# 三十四、字符串转数字

虽然这是 `string` 常用操作，但实际使用的是数字类型的方法。

## int.Parse()

```csharp
string str = "123";

int number = int.Parse(str);
```

如果字符串不是数字：

```csharp
int number = int.Parse("abc");
```

会抛出异常。

---

## int.TryParse()

更安全：

```csharp
string str = "123";

if (int.TryParse(str, out int number))
{
    Console.WriteLine(number);
}
```

转换失败不会抛异常：

```csharp
string str = "abc";

if (int.TryParse(str, out int number))
{
    // 转换成功
}
else
{
    // 转换失败
}
```

### 面试重点

> 用户输入、接口参数等不确定数据，通常优先考虑 `TryParse()`。

---

# 三十五、null 条件访问

不要直接：

```csharp
string str = null;

int length = str.Length;
```

会出现：

```text
NullReferenceException
```

可以：

```csharp
int? length = str?.Length;
```

或者：

```csharp
int length = str?.Length ?? 0;
```

意思：

```text
str 不为空 → 获取 Length
str 为空 → 返回 0
```

---

# 三十六、空字符串与 null

需要区分：

```csharp
string a = null;
string b = "";
string c = " ";
```

三者不同：

```text
null    没有字符串对象
""      空字符串
" "     一个空格
```

判断：

```csharp
string.IsNullOrEmpty(a);
```

判断：

```text
null
""
```

判断：

```csharp
string.IsNullOrWhiteSpace(c);
```

还可以判断：

```text
" "
"\t"
"\n"
```

---

# 三十七、常用方法速查表

| 方法                     | 作用             | 示例                               |
| ---------------------- | -------------- | -------------------------------- |
| `Length`               | 获取长度           | `str.Length`                     |
| `IsNullOrEmpty()`      | 判断 null / 空字符串 | `string.IsNullOrEmpty(str)`      |
| `IsNullOrWhiteSpace()` | 判断 null / 空白   | `string.IsNullOrWhiteSpace(str)` |
| `Trim()`               | 去除首尾空白         | `str.Trim()`                     |
| `TrimStart()`          | 去除开头空白         | `str.TrimStart()`                |
| `TrimEnd()`            | 去除结尾空白         | `str.TrimEnd()`                  |
| `ToUpper()`            | 转大写            | `str.ToUpper()`                  |
| `ToLower()`            | 转小写            | `str.ToLower()`                  |
| `Contains()`           | 是否包含           | `str.Contains("a")`              |
| `StartsWith()`         | 是否以某内容开头       | `str.StartsWith("A")`            |
| `EndsWith()`           | 是否以某内容结尾       | `str.EndsWith(".txt")`           |
| `IndexOf()`            | 查找第一次出现的位置     | `str.IndexOf("a")`               |
| `LastIndexOf()`        | 查找最后一次出现的位置    | `str.LastIndexOf("a")`           |
| `Substring()`          | 截取字符串          | `str.Substring(0, 3)`            |
| `Remove()`             | 删除字符串          | `str.Remove(0, 2)`               |
| `Insert()`             | 插入字符串          | `str.Insert(0, "A")`             |
| `Replace()`            | 替换字符串          | `str.Replace("A", "B")`          |
| `Split()`              | 分割字符串          | `str.Split(',')`                 |
| `Join()`               | 拼接数组           | `string.Join(",", arr)`          |
| `Concat()`             | 拼接字符串          | `string.Concat(a, b)`            |
| `PadLeft()`            | 左侧补字符          | `str.PadLeft(5, '0')`            |
| `PadRight()`           | 右侧补字符          | `str.PadRight(5, '0')`           |
| `Equals()`             | 判断内容是否相等       | `str.Equals("A")`                |
| `Compare()`            | 比较字符串          | `string.Compare(a, b)`           |
| `ToCharArray()`        | 转字符数组          | `str.ToCharArray()`              |
| `ToString()`           | 转字符串           | `number.ToString()`              |

---

# 三十八、面试重点：最需要记住的 15 个

如果是为了 **C# / .NET 面试**，优先记下面这些：

```text
1. Length
2. IsNullOrEmpty
3. IsNullOrWhiteSpace
4. Trim
5. ToUpper / ToLower
6. Contains
7. StartsWith
8. EndsWith
9. IndexOf
10. LastIndexOf
11. Substring
12. Replace
13. Split
14. Join
15. Equals
```

---

# 三十九、实际开发高频组合

## 1. 判断字符串是否为空

```csharp
if (string.IsNullOrWhiteSpace(name))
{
    return;
}
```

---

## 2. 去除用户输入的空格

```csharp
name = name.Trim();
```

---

## 3. 判断文件扩展名

```csharp
if (fileName.EndsWith(
    ".mp3",
    StringComparison.OrdinalIgnoreCase))
{
    // ...
}
```

---

## 4. 判断前缀

```csharp
if (fileName.StartsWith("out-"))
{
    // ...
}
```

---

## 5. 判断是否包含关键字

```csharp
if (text.Contains("error"))
{
    // ...
}
```

---

## 6. 截取字符串

```csharp
string result = str.Substring(0, 5);
```

---

## 7. 替换字符串

```csharp
str = str.Replace("old", "new");
```

---

## 8. 分割字符串

```csharp
string[] items = str.Split(',');
```

---

## 9. 拼接字符串

```csharp
string result = string.Join(",", items);
```

---

## 10. 忽略大小写比较

```csharp
if (string.Equals(
    a,
    b,
    StringComparison.OrdinalIgnoreCase))
{
    // 相等
}
```

---

# 四十、一个完整示例

```csharp
string input = "  Hello, Johnny, CSharp  ";

// 1. 去除首尾空格
input = input.Trim();

// 2. 判断是否为空
if (string.IsNullOrWhiteSpace(input))
{
    return;
}

// 3. 判断是否包含
if (input.Contains("Johnny"))
{
    Console.WriteLine("包含 Johnny");
}

// 4. 判断开头
if (input.StartsWith("Hello"))
{
    Console.WriteLine("以 Hello 开头");
}

// 5. 查找位置
int index = input.IndexOf("Johnny");

// 6. 替换
input = input.Replace("Johnny", "Tom");

// 7. 分割
string[] items = input.Split(',');

// 8. 遍历
foreach (string item in items)
{
    Console.WriteLine(item.Trim());
}

// 9. 重新拼接
string result = string.Join("-", items);

// 10. 转大写
result = result.ToUpper();

Console.WriteLine(result);
```

---

# 四十一、面试一句话总结

### string 最重要的几个特点

```text
string
  ↓
引用类型
  ↓
不可变
  ↓
修改字符串实际上会创建新的字符串
```

### 高频方法记忆

```text
判断：
IsNullOrEmpty
IsNullOrWhiteSpace

查找：
Contains
StartsWith
EndsWith
IndexOf
LastIndexOf

修改：
Trim
Replace
Insert
Remove

截取：
Substring

分割/拼接：
Split
Join
Concat

大小写：
ToUpper
ToLower

比较：
Equals
Compare

长度：
Length
```

### 最重要的一句话

> **String 是不可变的引用类型，`Replace()`、`Substring()`、`Trim()` 等操作不会修改原字符串，而是返回新的字符串。**
