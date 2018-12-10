using Magicodes.WeChat.MiniProgram;

namespace Magicodes.MiniProgram.Startup
{
    public class MiniProgramConfig : IMiniProgramConfig
    {
        public string MiniProgramAppId { get; set; }
        public string MiniProgramAppSecret { get; set; }
    }
}