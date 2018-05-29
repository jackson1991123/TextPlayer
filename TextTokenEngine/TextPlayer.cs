using System.Collections;
using System.Collections.Generic;
using TextTokenEngine.Core;

namespace TextTokenEngine
{
    /// <summary>
    /// 字符串播放器，可以用来播放一个有指令集的富文本
    /// 推荐自己继承这个类写solution，也可以先new再Add
    /// </summary>
    public abstract class TextPlayer
    {
        /// <summary>
        /// 句柄定义
        /// </summary>
        /// <param name="givenToken">由TokenType类中定义的类型</param>
        /// <returns></returns>
        public delegate IEnumerator PlayTokenHandler(Token givenToken);

        /// <summary>
        /// 富文本解析之后的tokens
        /// </summary>
        private List<Token> tokens = new List<Token>();

        /// <summary>
        /// 处理器字典
        /// key：由TokenType类中定义的类型
        /// value：返回值为IEnum的函数
        /// </summary>
        protected Dictionary<sbyte, PlayTokenHandler> handlers = new Dictionary<sbyte, PlayTokenHandler>();

        public TextPlayer(string richText)
        {
            Parser.Execute(richText, ref tokens);
        }

        /// <summary>
        /// 添加一个处理器句柄
        /// </summary>
        /// <param name="tokenType">由TokenType类中定义的类型</param>
        /// <param name="handler">处理句柄</param>
        public void Add(sbyte tokenType, PlayTokenHandler handler)
        {
            if (!handlers.ContainsKey(tokenType))
            {
                handlers.Add(tokenType, handler);
            }
        }

        /// <summary>
        /// 移除一个处理句柄
        /// </summary>
        /// <param name="tokenType">由TokenType类中定义的类型</param>
        public void Remove(sbyte tokenType)
        {
            if (handlers.ContainsKey(tokenType))
            {
                handlers.Remove(tokenType);
            }
        }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="finishCallBack">完毕回调</param>
        /// <returns></returns>
        public IEnumerator Play(System.Action finishCallBack)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (handlers.ContainsKey(token.tokenType))
                {
                    yield return handlers[token.tokenType].Invoke(token);
                }
            }

            if (finishCallBack != null)
            {
                finishCallBack.Invoke();
            }
        }
    }
}
