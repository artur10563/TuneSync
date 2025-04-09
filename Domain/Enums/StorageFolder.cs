namespace Domain.Enums;

public enum StorageFolder
{
    [StoragePath("")]
    None,
    [StoragePath("images/")]
    Images
}
	
public class StoragePathAttribute : Attribute
{
    public string Path { get; }

    public StoragePathAttribute(string path)
    {
        Path = path;
    }
}

public static class EnumExtensions
{
    public static string GetPath(this StorageFolder folder)
    {
        var type = folder.GetType();
        var member = type.GetMember(folder.ToString()).FirstOrDefault();
        var attributes = member?.GetCustomAttributes(typeof(StoragePathAttribute), false);
			
        return attributes?.Length > 0 ? ((StoragePathAttribute)attributes[0]).Path : string.Empty;
    }
}