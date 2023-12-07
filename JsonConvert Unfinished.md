JsonConvert 类是 Newtonsoft.Json 库中的一个核心类，用于处理 JSON 数据的序列化（对象转为 JSON 字符串）和反序列化（JSON 字符串转为对象）操作。以下是一些常用的 JsonConvert 类的方法及其作用：

SerializeObject 方法：

作用： 将对象序列化为 JSON 字符串。
示例： string json = JsonConvert.SerializeObject(myObject);
DeserializeObject 方法：

作用： 将 JSON 字符串反序列化为对象。
示例： MyObject myObject = JsonConvert.DeserializeObject<MyObject>(json);
SerializeXmlNode 方法：

作用： 将 XML 节点序列化为 JSON 字符串。
示例： string json = JsonConvert.SerializeXmlNode(xmlNode);
DeserializeXmlNode 方法：

作用： 将 JSON 字符串反序列化为 XML 节点。
示例： XmlNode xmlNode = JsonConvert.DeserializeXmlNode(json);
SerializeXNode 方法：

作用： 将 LINQ to XML 对象序列化为 JSON 字符串。
示例： string json = JsonConvert.SerializeXNode(xElement);
DeserializeXNode 方法：

作用： 将 JSON 字符串反序列化为 LINQ to XML 对象。
示例： XElement xElement = JsonConvert.DeserializeXNode(json);
PopulateObject 方法：

作用： 用 JSON 数据填充现有对象。
示例： JsonConvert.PopulateObject(json, myObject);
AnonymousTypeObject 方法：

作用： 将匿名类型序列化为 JSON 字符串。
示例： var jsonData = JsonConvert.AnonymousTypeObject(new { Name = "John", Age = 25 });
GetHashCode 方法：

作用： 获取 JSON 字符串的哈希码。
示例： int hashCode = JsonConvert.GetHashCode(json);  

```C#
 var bufferedContent = await httpResponseMessage.Content.ReadAsStreamAsync();
        var response = new TencentMeetingResponseBaseDto();
        using (var reader = new StreamReader(bufferedContent, Encoding.UTF8))
        {
            var responseBody = await reader.ReadToEndAsync();
            response = JsonConvert.DeserializeObject<TencentMeetingResponseBaseDto>(responseBody);
        }

await httpResponseMessage.Content.ReadAsStreamAsync()： 从 HttpResponseMessage 对象中获取响应内容的流，并异步读取为一个 Stream 对象。这个流是一个可缓存的内存流，称为 bufferedContent。  

var response = new TencentMeetingResponseBaseDto();： 创建一个新的 TencentMeetingResponseBaseDto 对象，用于存储反序列化后的响应数据。  

using (var reader = new StreamReader(bufferedContent, Encoding.UTF8))： 使用 StreamReader 类以指定的编码（UTF-8）从 bufferedContent 流中创建一个读取器。这个读取器 reader 将被用于逐行读取响应内容。

var responseBody = await reader.ReadToEndAsync();： 使用 ReadToEndAsync 方法异步读取整个响应内容，并将其存储在 responseBody 变量中。

response = JsonConvert.DeserializeObject<TencentMeetingResponseBaseDto>(responseBody);： 使用 JsonConvert 类的 DeserializeObject 方法，将 responseBody 中的 JSON 字符串反序列化为 TencentMeetingResponseBaseDto 对象。这个对象现在包含了从 API 返回的结构化数据。
```
