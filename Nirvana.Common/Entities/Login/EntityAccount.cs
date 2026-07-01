using System;
using System.Text.Json.Serialization;

namespace Nirvana.Common.Entities.Login;

public class EntityAccount : EntityUserInfo {
    // 基础信息
    [JsonPropertyName("name")]
    [JsonInclude]
    public string? Name { get; set; }

    [JsonPropertyName("account")]
    [JsonInclude]
    public string? Account { get; set; }

    [JsonPropertyName("type")]
    [JsonInclude]
    public string? Type { get; set; }

    [JsonPropertyName("password")]
    [JsonInclude]
    public string? Password { get; init; }

    // 识别信息
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /**
     * 根据 基础信息 判断 是否 是 同一个账号
     * @param other 另一个账号
     * @return 是否 是 同一个账号
     */
    public bool Equals(EntityAccount other)
    {
        // cookie 用 值 判断
        if ("cookie".Equals(Type) && "cookie".Equals(other.Type))
        {
            return string.Equals(Password, other.Password);
        }

        // 账号 密码 登录类型 一致 则 认为 是 同一个账号
        return string.Equals(Account, other.Account) && string.Equals(Type, other.Type) && string.Equals(Password, other.Password);
    }


    public new string ToString()
    {
        return Type == "cookie" ? $"Type: {Type}, Password: {Password}" : $"Account: {Account}, Type: {Type}, Password: {Password}";
    }

    // public void Update(EntityAccount account)
    // {
    //     Token = account.Token;
    //     UserId = account.UserId;
    // }

    public bool IsConfig()
    {
        // 主动登录游戏
        if (!NirvanaConfig.GetValue<bool>("autoLoginGame")) {
            return false;
        }

        // 主动登录 163Email
        if ("163Email".Equals(Type, StringComparison.OrdinalIgnoreCase)) {
            return NirvanaConfig.GetValue<bool>("autoLoginGame163Email");
        }

        // 主动登录 Cookie
        return "cookie".Equals(Type) && NirvanaConfig.GetValue<bool>("autoLoginGameCookie");
    }
}