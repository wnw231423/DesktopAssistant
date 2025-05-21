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
    private string _aiResponse = "����AI��Ӧ��...";

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
        // ת��Ϊ��׼Markdown��ʽ
        return 
               rawText
                   .Replace("��", ":")      // �滻Ϊ����ð��
                   .Replace("��", "\n- ")  // ת��Ϊ�����б�
                   .Replace("\n", "\n\n"); // ȷ������ָ�
    }

}