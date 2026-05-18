namespace BlogApi.Models;

/// <summary>
/// JWT 配置类，对应 appsettings.json 中 Jwt 节点
/// </summary>
public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpireMinutes { get; set; } = 60;
}
