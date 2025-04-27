using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Data.Models.AI;

namespace Core.AI
{
    public class AiService
    {
        private API _api;

        public AiService()
        {
            _api = AiService.GetApi();
        }
        
        // AI部分初始化工作, 将在程序启动时被调用
        public static void AiInit()
        {
            // TODO
            // 1. 你需要为用户创建一个本地配置文件, 用于存储API密钥和密钥
            // 2. 配置文件推荐放在用户个人目录的.config目录下
            throw new NotImplementedException();
        }
        
        // 向配置文件中写入给定的API密钥和密钥
        public static void WriteApiKey(string apiKey, string secretKey)
        {
            // TODO
            // 1. 用户在程序的设置界面填入API密钥和密钥后, 会调用此方法
            throw new NotImplementedException();
        }

        // 根据本地配置文件初始化API
        private static API GetApi()
        {
            // TODO:
            // 1. 你需要假设配置文件已经存在
            // 2. 你需要读取配置文件中的API密钥和密钥, 生成一个API对象
            // 3. 当用户使用AI功能时, 会调用此方法
            throw new NotImplementedException();
        }
        
        public async Task AiStringAsk()
        {
            //课表和todo
            string inputlist = "周一上午是数学课，下午则是英语听说训练；周二上午有物理实验课，下午则转为历史讲座；周三全天是编程与算法课程，晚上还有一场关于人工智能的研讨会；周四上午是化学实验，下午是文学赏析；周五上午是体育课的体能训练，下午则是自由讨论的哲学课。至于这周的todo清单，周一要完成数学作业并预习下周的物理内容，周二得复习历史讲座的重点并准备英语口语的练习材料，周三除了提交编程作业外，还要阅读一篇关于机器学习的论文，周四需完成化学实验报告并准备文学课的分享内容，周五则要整理体育课的笔记并规划周末的哲学阅读计划，别忘了每天晚上还要留出半小时进行自我反思和日程回顾哦。";
            //调用ai给出智能回答
            await _api.AiAssistent(inputlist);
        }
    }
}