namespace Data.Models.Table;

public class ResourceInfo
{   // 课程资源信息类, 用于GUI显示课程资源
    public string ResourceName { get; set; } // 资源名称
    public string ResourcePath { get; set; } // 资源路径
    public string ResourceType { get; set; } // 资源类型, 笔记? PPT?
    public DateTime LastModified { get; set; } // 最后修改时间
}