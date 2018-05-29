using System.Collections;
using System.Collections.Generic;
using TextTokenEngine.Core;

namespace TextTokenEngine
{
    /// <summary>
    /// 类型定义类：
    /// const sbyte作为 tokentype，减少enum引起的GC
    /// 每个tokentype必须配套一个解析器函数，格式为：
    /// private static void Convert_{typeName}(ref Token result, string[] paramsList)
    /// 在函数中，要根据解析结果组装result返回值，这里要进行paramlist的类型转化
    /// </summary>
    public partial class TokenType
    {
        /// <summary>
        /// 正规的字符串 
        /// 填充 content
        /// paramlist = null
        /// </summary>
        public const sbyte word = -1;

        /// <summary>
        /// 播放音频
        /// 填充 content，音频名称
        /// 填充 paramlist[0] float，音量
        /// </summary>
        public const sbyte audiostart = -2;

        /// <summary>
        /// 停止播放音频
        /// 填充 content，音频名称
        /// </summary>
        public const sbyte audiostop = -3;

        /// <summary>
        /// 等待 x 秒，格式 {wait=0.5}
        /// 填充 paramlist[0] float，等待的秒数
        /// </summary>
        public const sbyte wait = -4;

        /// <summary>
        /// 屏幕闪白 x 秒，格式 {flash=0.5}
        /// 填充 paramlist[0] float，等待的秒数
        /// </summary>
        public const sbyte flash = -5;

        private static void Convert_word(ref Token result, string[] paramsList)
        {
            result.tokenType = word;
            result.content = paramsList[0];
        }

        private static void Convert_audiostart(ref Token result, string[] paramsList)
        {
            result.tokenType = audiostart;
            result.content = paramsList[0];
            result.paramlist = new object[1] { System.Convert.ToSingle(paramsList[1]) };
        }

        private static void Convert_audiostop(ref Token result, string[] paramsList)
        {
            result.tokenType = audiostop;
            result.content = paramsList[0];
        }

        private static void Convert_wait(ref Token result, string[] paramsList)
        {
            result.tokenType = wait;
            result.paramlist = new object[1] { System.Convert.ToSingle(paramsList[0]) };
        }

        private static void Convert_flash(ref Token result, string[] paramsList)
        {
            result.tokenType = flash;
            result.paramlist = new object[1] { System.Convert.ToSingle(paramsList[0]) };
        }
    }
}
