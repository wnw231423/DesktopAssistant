using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace Data.Models.AI
{
    public class API
    {
        //在本地配置文件中存储API密钥和密钥, 然后读取它. 不要直接硬编码到代码里
        private string _api_key;
        private string _secret_key;
        private const string TOKEN_URL = "https://aip.baidubce.com/oauth/2.0/token";
        private const string API_URL = "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/completions";
        public string inputlist;

        public API(string api_key, string secret_key)
        {
            _api_key = api_key;
            _secret_key = secret_key;
        }

        public async Task<string> AiAssistent(string inputlist)
        {
            var accessToken = await GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                //Console.WriteLine("Failed to get access token");
                return "获取密钥失败";
            }
            return await ChatLoop(accessToken, inputlist);
        }

        public async Task<string> GetAccessToken()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{TOKEN_URL}?grant_type=client_credentials&client_id={_api_key}&client_secret={_secret_key}");
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            return json["access_token"]?.ToString();
        }

        public async Task<string> ChatLoop(string accessToken,string inputlist)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            //Console.WriteLine("文心一言对话系统（输入'exit'退出）");
            Console.WriteLine("祝您度过充实的一天！");
            //预先设定要根据输入提供今日提醒
            var input1 = "今天是几号";
            var response1 = await SendChatRequest(client, accessToken, input1);
            var answer1 = ParseResponse(response1);
            //Console.WriteLine($"{answer1}");

            var input2 = "今天武汉市洪山区的天气怎么样,只说出实时气温和最高和最低气温和天气情况，分行回答，后边不要附加其他东西";
            var response2 = await SendChatRequest(client, accessToken, input2);
            var answer2 = ParseResponse(response2);
            //Console.WriteLine($"{answer2}");

            var preinput = "然后请根据下面的内容简要概括出今天所需要干的事情，分点输出上午和下午和晚上要干的每个任务，如果有具体时间则一并输出任务的时间，不需要输出今天的日期，只输出任务。";
            var inputtodo = answer1 + preinput + inputlist;
            var responsetodo = await SendChatRequest(client, accessToken, inputtodo);
            var answertodo = ParseResponse(responsetodo);
            //Console.WriteLine("今天的任务是："+"\n"+$"{answertodo}");

            /*while (true)
            {
                Console.Write("你：");
                var input = Console.ReadLine();
                if (input.ToLower() == "exit")
                {
                    break;
                }

                var response = await SendChatRequest(client, accessToken, input);
                var answer = ParseResponse(response);

                Console.WriteLine($"AI：{answer}");
            }*/
            return answer1 +"\n" + "\n" + answer2 + "\n" + "\n" + "今天的任务是：\n"  + answertodo+ "\n" + "\n" + "祝您度过元气满满的一天！";
        }

        public async Task<string> SendChatRequest(HttpClient client, string accessToken, string message)
        {
            var requestBody = new
            {
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = message
                    }
                },
                // 根据需要选择模型（例如ernie-4.0、ernie-3.5等）
                model = "ernie-4.5"
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"{API_URL}?access_token={accessToken}";
            var response = await client.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }

        public string ParseResponse(string response)
        {
            try
            {
                var json = JObject.Parse(response);
                if (json["result"] != null)
                {
                    return json["result"].ToString();
                }
                if (json["error_code"] != null)
                {
                    return $"Error {json["error_code"]}: {json["error_msg"]}";
                }
                return "无法解析响应";
            }
            catch (Exception ex)
            {
                return $"解析响应时出错：{ex.Message}";
            }
        }
    }
}
