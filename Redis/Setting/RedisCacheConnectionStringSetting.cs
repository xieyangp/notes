using Microsoft.Extensions.Configuration;

namespace AmusementPark.Core.Setting.Caching;

public class RedisCacheConnectionStringSetting : IConfiguartionSetting<string>
{
    public RedisCacheConnectionStringSetting(IConfiguration configuration)
    {
        Value = configuration.GetValue<string>("RedisCacheConnectionString");
    }
    
    public string Value { get; set; }
}
