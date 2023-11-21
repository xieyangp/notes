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
### 5.处理空集合,映射集合属性时，如果源值为 null，AutoMapper 会将目标字段映射到空集合，而不是将目标值设置为 null。可以将AllowNullCollections属性设置为true将空值设置为null：
```C#
public class FoodMapping : Profile
{
    public FoodMapping()
    {
        AllowNullCollections = true;
        
        CreateMap<CreateFoodDto, Foods>().ReverseMap();
    }
}
```
### 6.集合中的多态元素类型,当源类型和目标类型都有子类，配置映射时用include将子类映射也配置一下且需要对子映射进行显式配置：
```C#
public class ParentSource
{
    public int Value1 { get; set; }
}

public class ChildSource : ParentSource
{
    public int Value2 { get; set; }
}

public class ParentDestination
{
    public int Value1 { get; set; }
}

public class ChildDestination : ParentDestination
{
    public int Value2 { get; set; }
}

var configuration = new MapperConfiguration(c=> {
    c.CreateMap<ParentSource, ParentDestination>()
	     .Include<ChildSource, ChildDestination>();
    c.CreateMap<ChildSource, ChildDestination>();
});
```
### 7.源成员映射到目标构造函数：
```C#
public class Source {
    public int Value { get; set; }
}
public class SourceDto {
    public SourceDto(int value) {
        _value = value;
    }
    private int _value;
    public int Value {
        get { return _value; }
    }
}
var configuration = new MapperConfiguration(cfg => cfg.CreateMap<Source, SourceDto>());

var configuration = new MapperConfiguration(cfg =>
  cfg.CreateMap<Source, SourceDto>()
    .ForCtorParam("valueParamSomeOtherName", opt => opt.MapFrom(src => src.Value))//目标构造函数参数名称不匹配时，通过ForCtorParam修改
);

var configuration = new MapperConfiguration(cfg => cfg.DisableConstructorMapping());//禁用构造函数映射

var configuration = new MapperConfiguration(cfg => cfg.ShouldUseConstructor = constructor => constructor.IsPublic);//配置目标对象考虑哪些构造函数，这里是只考虑公共构造函数
```
### 8.扁平化；当希望将一个复杂的对象映射到一个简单的对象时，通常将复杂对象扁平化：
```C#
//复杂的对象
public class Order
{
     private readonly IList<OrderLineItem> _orderLineItems = new List<OrderLineItem>();

     public Customer Customer { get; set; }

     public OrderLineItem[] GetOrderLineItems()
     {
         return _orderLineItems.ToArray();
     }

     public void AddOrderLineItem(Product product, int quantity)
     {
         _orderLineItems.Add(new OrderLineItem(product, quantity));
     }

     public decimal GetTotal()
     {
         return _orderLineItems.Sum(li => li.GetTotal());
     }
}

public class Product
{
    public decimal Price { get; set; }
    public string Name { get; set; }
}

public class OrderLineItem
{
    public OrderLineItem(Product product, int quantity)
    {
        Product = product;
        Quantity = quantity;
    }

    public Product Product { get; private set; }
    public int Quantity { get; private set;}

    public decimal GetTotal()
    {
        return Quantity*Product.Price; 
    }
}

public class Customer
{
    public string Name { get; set; }
}

public class OrderDto
{
     public string CustomerName { get; set; }
     public decimal Total { get; set; }
}
```
