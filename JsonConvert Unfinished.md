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
