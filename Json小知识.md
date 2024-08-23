## Json
### 1.JsonSerializer.Deserialize 
```
要配置PropertyNameCaseInsensitive为true才能根据属性名进行映射，可以设置JsonPropertyName标明成员的映射关系，如果不配置JsonPropertyName，会根据成员名进行映射，不分大小写，但是如果有_等特殊符号不会忽略；
```
### 2.JsonConvert.DeserializeObject
```
可以设置JsonProperty标明成员的映射关系，如果不配置JsonProperty，会根据成员名进行映射，不分大小写，但是如果有_等特殊符号不会忽略
```
