using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Data.Models.AI;
using System.IO;
using System.Text.Json;
using Core.Table;
using Core.Todo;

namespace Core.AI
{
    public class AiService
    {
        private class ApiConfig
        {
            public string ApiKey { get; set; }
            public string SecretKey { get; set; }
        }

        private static readonly string ConfigDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config",
            "MyAIApp"
        );

        private static readonly string ConfigFilePath = Path.Combine(ConfigDirectory, "ai_config.json");

        private API _api;

        public AiService()
        {
            _api = AiService.GetApi();
        }

        // AI部分初始化工作, 将在程序启动时被调用
        public static void AiInit()
        {
            //为用户创建一个本地配置文件, 用于存储API密钥和密钥
            //配置文件放在用户个人目录的.config目录下
            if (!Directory.Exists(ConfigDirectory))
            {
                Directory.CreateDirectory(ConfigDirectory);
            }
        }

        // 向配置文件中写入给定的API密钥和密钥
        public static void WriteApiKey(string apiKey, string secretKey)
        {
            //用户在程序的设置界面填入API密钥和密钥后, 会调用此方法
            var config = new ApiConfig
            {
                ApiKey = apiKey,
                SecretKey = secretKey
            };

            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(config, jsonOptions);
            File.WriteAllText(ConfigFilePath, json);
        }

        // 根据本地配置文件初始化API
        public static API GetApi()
        {
            //假设配置文件已经存在
            //读取配置文件中的API密钥和密钥, 生成一个API对象
            //当用户使用AI功能时, 会调用此方法
            if (!File.Exists(ConfigFilePath))
            {
                throw new FileNotFoundException("API配置文件不存在");
            }

            string json = File.ReadAllText(ConfigFilePath);
            var config = JsonSerializer.Deserialize<ApiConfig>(json);
            
            return new API(config.ApiKey, config.SecretKey);
        }

        public async Task<string> AiStringAsk(string input_table,string input_todo)
        {
            string inputlist = input_table + input_todo;
            //调用ai给出智能回答
             return await _api.AiAssistent(inputlist);

        }
    }
}