using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Nirvana.Common.Entities;
using Nirvana.Common.Utils;
using Nirvana.Common.Utils.CodeTools;
using Serilog;

namespace Nirvana.Common;

public class NirvanaConfig {
    private static readonly List<IConfigValue> ConfigValues = [
        new ConfigValue<bool>(true) {
            Name = "hideAccount"
        }, // 隐藏账号
        new ConfigValue<bool>(true) {
            Name = "chatEnable"
        }, // 聊天功能
        new ConfigValue<int>(4096) {
            Name = "gameMemory"
        }, // 游戏内存
        new ConfigValue<string>("-XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Djdk.lang.Process.allowAmbiguousCommands=true -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True -Dlog4j2.formatMsgNoLookups=true") {
            Name = "jvmArgs"
        }, // 虚拟机参数
        new ConfigValue<string>(string.Empty) {
            Name = "gameArgs"
        }, // 游戏参数
        new ConfigValue<bool>(true) {
            Name = "autoLoginGame"
        }, // 主动登录游戏
        new ConfigValue<bool>(false) {
            Name = "autoLoginGame163Email"
        }, // 主动登录 163Email
        new ConfigValue<bool>(true) {
            Name = "autoLoginGameCookie"
        }, // 主动登录 Cookie
        new ConfigValue<bool>(true) {
            Name = "useJavaW"
        }, // 使用 javaw.exe
        new ConfigValue<bool>(true) {
            Name = "autoUpdatePlugin"
        }, // 自动更新插件
        new ConfigValue<bool>(false) {
            Name = "skipVersionCheck"
        }, // 跳过版本校验
        new ConfigValue<string> {
            Name = "account"
        }, // 涅槃账号
        new ConfigValue<string> {
            Name = "token"
        }, // 涅槃在线密钥
        new ConfigValue<JsonNode> {
            Name = "netease_device"
        }, // 163Email 设备 ID
        new ConfigValue<string> {
            Name = "netease_unique"
        } // 163Email 唯一标识
    ];

    // 初始化
    public static void Initialization()
    {
        lock (PathUtil.ConfigPath) {
            var entity = Tools.GetValueOrDefault<JsonObject>("nirvanaAccount.json").Item1;
            if (entity == null) {
                return;
            }

            foreach (var configValue in entity) {
                try {
                    SetValue(configValue.Key, configValue.Value, false);
                } catch (KeyNotFoundException) {
                    AddFromJsonNode(configValue.Key, configValue.Value);
                } catch (Exception e) {
                    Log.Error("初始化配置 {0} 失败 : {1}", configValue.Key, e.Message);
                }
            }
        }
    }

    private static void AddFromJsonNode(string name, JsonNode? node)
    {
        if (node == null) {
            return;
        }

        if (node.GetValueKind() == JsonValueKind.String) {
            ConfigValues.Add(new ConfigValue<string> {
                Name = name,
                Value = node.ToString()
            });
        } else if (node.GetValueKind() == JsonValueKind.Number) {
            ConfigValues.Add(new ConfigValue<double> {
                Name = name,
                Value = node.GetValue<double>()
            });
        } else if (node.GetValueKind() == JsonValueKind.True) {
            ConfigValues.Add(new ConfigValue<bool> {
                Name = name,
                Value = true
            });
        } else if (node.GetValueKind() == JsonValueKind.False) {
            ConfigValues.Add(new ConfigValue<bool> {
                Name = name,
                Value = false
            });
        } else if (node.GetValueKind() == JsonValueKind.Object) {
            ConfigValues.Add(new ConfigValue<JsonNode> {
                Name = name,
                Value = node
            });
        }
    }

    private static IConfigValue? GetConfig(string name)
    {
        return ConfigValues.FirstOrDefault(obj => obj.EqualsName(name));
    }

    private static IConfigValue GetConfigTo(string name)
    {
        var configValue = GetConfig(name);
        return configValue ?? throw new KeyNotFoundException();
    }

    public static T GetValue<T>(string name, Func<T>? defaultValue = null)
    {
        var config = GetConfig(name);
        if (config == null) {
            throw new KeyNotFoundException();
        }

        var context = config.GetValueTo();
        if (context == null && defaultValue != null) {
            var defaultVal = defaultValue.Invoke();
            SetValue(name, defaultVal);
            return defaultVal;
        }

        ArgumentNullException.ThrowIfNull(context);

        if (context is JsonNode jsonObject) {
            return jsonObject.Deserialize<T>() ?? throw new InvalidOperationException();
        }

        return (T)Convert.ChangeType(context, typeof(T));
    }

    public static void SetValue(string name, object? value, bool save = true)
    {
        GetConfigTo(name).SetFrom(value);
        if (save) {
            SaveConfig();
        }
    }

    private static void AddOrUpdate<T>(string name, string? defaultValue, Func<string, T> parser)
    {
        var existing = GetConfig(name);
        if (existing != null) {
            if (defaultValue != null) {
                existing.SetDefaultFrom(parser(defaultValue));
            }
        } else {
            var configValue = new ConfigValue<T> {
                Name = name
            };
            if (defaultValue != null) {
                configValue.SetDefault(parser(defaultValue));
            }

            ConfigValues.Add(configValue);
        }
    }

    public static void AddByTypeName(string name, string? defaultValue, string? typeName)
    {
        switch (typeName?.ToLower()) {
            case "string":
                AddOrUpdate(name, defaultValue, s => s);
                break;
            case "bool":
                AddOrUpdate(name, defaultValue, bool.Parse);
                break;
            case "int":
                AddOrUpdate(name, defaultValue, int.Parse);
                break;
            case "float":
                AddOrUpdate(name, defaultValue, float.Parse);
                break;
            case "double":
                AddOrUpdate(name, defaultValue, double.Parse);
                break;
            case "json":
                AddOrUpdate(name, defaultValue, s => JsonNode.Parse(s));
                break;
        }
    }

    private static string ToString(bool showDefault = true)
    {
        return JsonSerializer.Serialize(GetJsonObject(showDefault));
    }

    public static JsonObject GetJsonObject(bool showDefault = true)
    {
        var jsonObj = new JsonObject();
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var obj in ConfigValues) {
            if (showDefault || !obj.IsDefault()) {
                obj.ToAdd(jsonObj);
            }
        }

        return jsonObj;
    }

    // 保存账号
    private static void SaveConfig()
    {
        lock (PathUtil.ConfigPath) {
            File.WriteAllText(PathUtil.ConfigPath, ToString(false), Encoding.UTF8);
        }
    }

    // 退出登录
    public static void Logout()
    {
        SetValue("account", string.Empty);
        SetValue("token", string.Empty);
        SaveConfig();
    }

    // 登录检测
    public static void IsLogin()
    {
        var account = GetValue<string>("account");
        var token = GetValue<string>("token");
        if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(token)) {
            throw new ErrorCodeException(ErrorCode.LogInNot);
        }
    }

    public static void SetGameMemory(string? value)
    {
        if (string.IsNullOrEmpty(value)) {
            throw new ErrorCodeException(ErrorCode.MemoryError);
        }

        var gameMemory = int.Parse(value);
        if (gameMemory < 1024) {
            throw new ErrorCodeException(ErrorCode.MemoryError);
        }

        SetValue("gameMemory", gameMemory);
    }

    public static string GetLoginT()
    {
        var account = GetValue<string>("account");
        var token = GetValue<string>("token");
        return $"account={account}&online={token}";
    }
}