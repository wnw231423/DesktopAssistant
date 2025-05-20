using CommunityToolkit.Mvvm.ComponentModel;
using Core.AI;
using System.Threading.Tasks;
using Markdown.Avalonia;

namespace GUI.ViewModels;

public partial class DailyReportWindowViewModel: ObservableObject
{
    private readonly AiService _aiService = new();

    [ObservableProperty]
    private string _aiResponse = "����AI��Ӧ��...";

    public async Task LoadAiResponse()
    {
        var rawResponse = await _aiService.AiStringAsk();
        AiResponse = FormatAsMarkdown(rawResponse);
    }

    private static string FormatAsMarkdown(string rawText)
    {
        // ת��Ϊ��׼Markdown��ʽ
        return $"# ÿ�ձ���\n\n" +
               rawText
                   .Replace("��", ":")      // �滻Ϊ����ð��
                   .Replace("��", "\n- ")  // ת��Ϊ�����б�
                   .Replace("\n", "\n\n"); // ȷ������ָ�
    }

}