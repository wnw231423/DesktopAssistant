using CommunityToolkit.Mvvm.ComponentModel;
using Core.AI;
using System.Threading.Tasks;
using Markdown.Avalonia;

namespace GUI.ViewModels;

public partial class DailyReportWindowViewModel: ObservableObject
{
    private readonly AiService _aiService = new();

    [ObservableProperty]
    private string _aiResponse = "加载AI响应中...";

    public async Task LoadAiResponse()
    {
        var rawResponse = await _aiService.AiStringAsk();
        AiResponse = FormatAsMarkdown(rawResponse);
    }

    private static string FormatAsMarkdown(string rawText)
    {
        // 转换为标准Markdown格式
        return $"# 每日报告\n\n" +
               rawText
                   .Replace("：", ":")      // 替换为常规冒号
                   .Replace("；", "\n- ")  // 转换为无序列表
                   .Replace("\n", "\n\n"); // 确保段落分隔
    }

}