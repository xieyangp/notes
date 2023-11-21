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
### 3.当源类型中有复杂类型的时候，需要为复杂类型也进行映射配置。配置时候注意：配置类型的顺序并不重要，调用 Map 不需要指定任何内部类型映射，只需指定用于传入源值的类型映射：
```C#
public class OuterSource
{
	public int Value { get; set; }
	public InnerSource Inner { get; set; }//复杂类型
}

public class InnerSource
{
	public int OtherValue { get; set; }
}

public class OuterDest
{
	public int Value { get; set; }
	public InnerDest Inner { get; set; }//复杂类型
}

public class InnerDest
{
	public int OtherValue { get; set; }
}

var config = new MapperConfiguration(cfg => {
    cfg.CreateMap<OuterSource, OuterDest>();
    cfg.CreateMap<InnerSource, InnerDest>();
});
```
### 4.AutoMapper 仅需要配置元素类型，而不需要配置任何可能使用的数组或列表类型。
```C#
public class Source
{
	public int Value { get; set; }
}

public class Destination
{
	public int Value { get; set; }
}

var configuration = new MapperConfiguration(cfg => cfg.CreateMap<Source, Destination>());

var sources = new[]
	{
		new Source { Value = 5 },
		new Source { Value = 6 },
		new Source { Value = 7 }
	};

IEnumerable<Destination> ienumerableDest = mapper.Map<Source[], IEnumerable<Destination>>(sources);
ICollection<Destination> icollectionDest = mapper.Map<Source[], ICollection<Destination>>(sources);
IList<Destination> ilistDest = mapper.Map<Source[], IList<Destination>>(sources);
List<Destination> listDest = mapper.Map<Source[], List<Destination>>(sources);
Destination[] arrayDest = mapper.Map<Source[], Destination[]>(sources);
```
