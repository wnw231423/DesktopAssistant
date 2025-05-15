using CommunityToolkit.Mvvm.ComponentModel;
using Core.AI;
using System.Threading.Tasks;

namespace GUI.ViewModels;

public partial class DailyReportWindowViewModel: ObservableObject
{
    private readonly AiService _aiService = new();

    [ObservableProperty]
    private string _aiResponse = "���ڻ�ȡAI����...";

    public async Task LoadAiResponse()
    {
        AiResponse = await _aiService.AiStringAsk();
    }
}