using CommunityToolkit.Mvvm.ComponentModel;
using Core.AI;
using System.Threading.Tasks;
using Markdown.Avalonia;
using Core.Todo;
using Avalonia.Controls;
using Core.Table;

namespace GUI.ViewModels;

public partial class DailyReportWindowViewModel: ObservableObject
{
    private readonly AiService _aiService = new();
    private readonly TodoService _todoService;
    private readonly TableService _tableService;

    [ObservableProperty]
    private string _aiResponse = "加载AI响应中...";

    public DailyReportWindowViewModel(TableService tableService, TodoService todoService)
    {
        _todoService = todoService;
        _tableService = tableService;
    }

    public async Task LoadAiResponse()
    { 
        var rawResponse = await _aiService.AiStringAsk(_tableService.GetCoursesString(), _todoService.GetTodoItemString());
        AiResponse = FormatAsMarkdown(rawResponse);
    }

    private static string FormatAsMarkdown(string rawText)
    {
        // 转换为标准Markdown格式
        return 
               rawText
                   .Replace("：", ":")      // 替换为常规冒号
                   .Replace("；", "\n- ")  // 转换为无序列表
                   .Replace("\n", "\n\n"); // 确保段落分隔
    }

}