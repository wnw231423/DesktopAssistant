using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Todo;

public class TodoItem
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // 主键自增
    public int Id {get; set;}  // 主键
    
    //TODO: Add properties for TodoItem class.
}