## 一.AutoMapper概念：一个自动映射对象的开源库，它制定了一些规则，我们可以通过这些规则可以使对象相互映射。
## 二.AutoMapper的一些常用规则：
### 1.当源与目标成员一致时，
```C#
.CreateMap<Source, Destination>()//Source为源对象，Destination为目标对象，
```
### 2.当您想要将源值投影到与源结构不完全匹配的目标时，可以通过自定义成员映射进行映射；
```C#
.CreateMap<Source, Destination>()
	.ForMember(DestinationObject => DestinationObject.EventDate, SourceObject => SourceObject.MapFrom(SourceObject => SourceObject.Date.Date))
```
### 3.
