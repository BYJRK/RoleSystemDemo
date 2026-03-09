using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using RoleSystemDemo.Models;

namespace RoleSystemDemo.Services;

/// <summary>
/// 数据持久化服务，使用 JSON 文件作为存储后端
/// 文件位于程序运行目录下的 Data/ 子目录中
/// </summary>
public static class DataService
{
    private static readonly string DataDir = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Data"
    );

    private static readonly string UsersFile = Path.Combine(DataDir, "users.json");
    private static readonly string PostsFile = Path.Combine(DataDir, "posts.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    #region Users

    public static List<UserModel> LoadUsers()
    {
        if (!File.Exists(UsersFile))
            return [];
        var json = File.ReadAllText(UsersFile);
        return JsonSerializer.Deserialize<List<UserModel>>(json, JsonOptions) ?? [];
    }

    public static void SaveUsers(IEnumerable<UserModel> users)
    {
        Directory.CreateDirectory(DataDir);
        var json = JsonSerializer.Serialize(users, JsonOptions);
        File.WriteAllText(UsersFile, json);
    }

    #endregion

    #region Posts

    public static List<BlogPost> LoadPosts()
    {
        if (!File.Exists(PostsFile))
            return [];
        var json = File.ReadAllText(PostsFile);
        return JsonSerializer.Deserialize<List<BlogPost>>(json, JsonOptions) ?? [];
    }

    public static void SavePosts(IEnumerable<BlogPost> posts)
    {
        Directory.CreateDirectory(DataDir);
        var json = JsonSerializer.Serialize(posts, JsonOptions);
        File.WriteAllText(PostsFile, json);
    }

    #endregion
}
