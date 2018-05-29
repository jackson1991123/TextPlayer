using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System;

namespace TextTokenEngine.Core
{
    /// <summary>
    /// 富文本解析后的token结果
    /// </summary>
    public class Token
    {
        /// <summary>
        /// 由TokenType类中定义的类型
        /// </summary>
        public sbyte tokenType;

        /// <summary>
        /// 字符串内容，是否使用此字段请看 TokenType类的注解
        /// </summary>
        public string content;

        /// <summary>
        /// 其他参数，是否使用，怎么使用此字段请看 TokenType类的注解
        /// </summary>
        public object[] paramlist;

        public override string ToString()
        {
            if (tokenType == TokenType.word)
            {
                return content;
            }
            else
            {
                var builder = new System.Text.StringBuilder();
                builder.AppendFormat("[{0}: ", tokenType);
                if (!string.IsNullOrEmpty(content))
                {
                    builder.AppendFormat("content = {0} ", content);
                }
                foreach (var param in paramlist)
                {
                    builder.AppendFormat("{0}, ", param);
                }
                builder.Append("]");
                return builder.ToString();
            }
        }
    }

    /// <summary>
    /// 解析器
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// 反射-从TokenType中获得名称格式为 Convert_{typeName}的处理函数
        /// </summary>
        private delegate void TagParserHandler(ref Token result, string[] paramsList);

        private const char equalOperator = '=';
        private const char paramSpliter = ',';
        private const char endTail = '\r';
        private const string elementString = @"\s";
        private static Regex regex = new Regex(@"\{.*?\}");
        private static Dictionary<string, sbyte> string2TypeReflectionMap = new Dictionary<string, sbyte>();
        private static Dictionary<string, TagParserHandler> tokenParserHandlerReflectionMap = new Dictionary<string, TagParserHandler>();

        private static System.Text.StringBuilder tempBuilder = new System.Text.StringBuilder();
        private static string[] tempParamStrings = new string[128];

        static Parser()
        {
            //反射构建编译词典
            Type convertHandlerType = typeof(TagParserHandler);
            Type type = typeof(TokenType);
            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields)
            {
                sbyte value = (sbyte)(field.GetValue(null)) ;
                string key = field.Name;
                string handlerName = string.Concat("Convert_", key);
                MethodInfo method = type.GetMethod(handlerName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                TagParserHandler targetHandler = method == null ? null : Delegate.CreateDelegate(convertHandlerType, method) as TagParserHandler;
                tokenParserHandlerReflectionMap.Add(field.Name, targetHandler);
                string2TypeReflectionMap.Add(field.Name, (sbyte)(field.GetValue(null)));
            }
        }

        /// <summary>
        /// 执行解析
        /// </summary>
        /// <param name="sourceText">富文本</param>
        /// <param name="tokens">结果</param>
        public static void Execute(string sourceText, ref List<Token> tokens)
        {
            Match match = regex.Match(sourceText);

            int position = 0;
            while (match.Success)
            {
                string preText = sourceText.Substring(position, match.Index - position);
                string tagText = match.Value;

                if (preText != "")
                {
                    //构造一个word类型的token
                    Token token = new Token();
                    tempParamStrings[0] = preText;
                    tokenParserHandlerReflectionMap["word"](ref token, tempParamStrings);
                    tokens.Add(token);
                }

                //解析这个tag
                //构造一个tag类型的token
                tempBuilder.Length = 0;
                string tagTextEx = Regex.Replace(tagText, elementString, string.Empty) ;
                tagTextEx = tagTextEx.Substring(1, tagTextEx.Length - 2);

                tempBuilder.Append(tagTextEx);
                tempBuilder.Append(endTail);
                tagTextEx = tempBuilder.ToString();
                tempBuilder.Length = 0;

                string tagName = string.Empty;
                int paramIndex = 0;

                bool meetEqual = false;
                for (int i = 0; i < tagTextEx.Length; i++)
                {
                    char tagChar = tagTextEx[i];
                    if (!meetEqual)
                    {
                        //到了末尾也没有看到等于号，说明无参数，只有tag
                        if (tagChar == endTail)
                        {
                            tagName = tempBuilder.ToString();
                            tempBuilder.Length = 0;
                        }
                        //看到等于号了，之前的是tag
                        else if (tagChar == equalOperator)
                        {
                            meetEqual = true;
                            tagName = tempBuilder.ToString();
                            tempBuilder.Length = 0;
                        }
                        else
                        {
                            tempBuilder.Append(tagChar);
                        }
                    }
                    else
                    {
                        //最后一个了，收束;遇见分隔符，也得收束
                        if (tagChar == endTail || tagChar == ',')
                        {
                            tempParamStrings[paramIndex++] = tempBuilder.ToString();
                            tempBuilder.Length = 0;
                        }
                        else 
                        {
                            tempBuilder.Append(tagChar);
                        }
                    }
                }

                if (tokenParserHandlerReflectionMap.ContainsKey(tagName))
                {
                    Token token = new Token();
                    tokenParserHandlerReflectionMap[tagName].Invoke(ref token, tempParamStrings);
                    tokens.Add(token);
                }

                position = match.Index + tagText.Length;
                match = match.NextMatch();
            }

            if (position < sourceText.Length)
            {
                string postText = sourceText.Substring(position, sourceText.Length - position);
                if (postText.Length > 0)
                {
                    Token token = new Token();
                    tempParamStrings[0] = postText;
                    tokenParserHandlerReflectionMap["word"](ref token, tempParamStrings);
                    tokens.Add(token);
                }
            }
        }
    }

}


