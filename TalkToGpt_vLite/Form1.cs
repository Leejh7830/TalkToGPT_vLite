using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using System.Runtime.InteropServices;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using OpenAI;



namespace TalkToGpt_vLite
{
    public partial class Form1 : Form
    {
        IConfiguration configuration;
        string apiKey;

        public Form1()
        {
            InitializeComponent();
            apiKey = "여기 API키를 입력해주세요";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RequestGPT();
        }

        async Task RequestGPT()
        {
            var gpt3 = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = apiKey
            });

            WrapAPI.SetConsoleTextColor(ConTextColor.LIGHT_WHITE);
            Console.Write("You : ");
            string q = Console.ReadLine();

            var completionResult = await gpt3.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = q,
                Model = Models.TextDavinciV3, //모델명. GPT한테 물어보니 다빈치V3이 가장 좋은 버젼이라고 했음. 하지만.. GPT 3.5 tubo가 또 나왔다고 함
                Temperature = 0.5F,      //대답의 자유도(다양성 - Diversity)). 자유도가 낮으면 같은 대답, 높으면 좀 아무말?
                MaxTokens = 1000,      //이게 길수록 글자가 많아짐. 짧은 답장은 상관없으나 이게 100,200으로 짧으면 말을 짤라버림 (시간제약이 있거나 썸네일식으로 확인만 할때는 낮추면 좋을 듯. 추가로 토큰은 1개 단어라고 생각하면 편한데, 정확하게 1개 단어는 아닌 (1개 단어가 될수도 있고 긴단어는 2개 단어가 될수 있음. GPT 검색의 단위가된다고 함. 이 토큰 단위를 기준으로 트래픽이 매겨지고, (유료인경우) 과금 책정이 됨)
                N = 1   //경우의 수(대답의 수). N=3으로 하면 3번 다른 회신을 배열에 담아줌
            });

            WrapAPI.SetConsoleTextColor(ConTextColor.LIGHT_GREEN);

            if (completionResult.Successful)
            {
                foreach (var choice in completionResult.Choices)
                {
                    Console.WriteLine("\n" + "GPT : " + choice.Text.Replace("\n", ""));
                }
                Console.WriteLine();
            }
            else
            {
                if (completionResult.Error == null)
                {
                    throw new Exception("Unknown Error");
                }
                Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
            }
            RequestGPT();
        }
    }

    //아래는 글자 색 바꿔주는 부분
    public enum ConTextColor
    {
        LACK, BLUE, GREEN, JADE, RED,
        PURPLE, YELLOW, WHITE, GRAY, LIGHT_BLUE, LIGHT_GREEN,
        LIGHT_JADE, LIGHT_RED, LIGHT_PURPLE,
        LIGHT_YELLOW, LIGHT_WHITE
    };

    public static class WrapAPI
    {
        [DllImport("Kernel32.dll")]
        static extern int SetConsoleTextAttribute(IntPtr hConsoleOutput, short wAttributes);

        [DllImport("Kernel32.dll")]
        static extern IntPtr GetStdHandle(int nStdHandle);

        const int STD_OUTPUT_HANDLE = -11;
        public static void SetConsoleTextColor(ConTextColor color)
        {
            IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
            SetConsoleTextAttribute(handle, (short)color);
        }
    }
}
