/*一个自定义的Token类型的例子，这是一个扩展点*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TextTokenEngine
{
    using TextTokenEngine.Core;

    public partial class TokenType
    {
        /// <summary>
        /// 摇晃摄像机，格式 {camerashake=1}
        /// 填充 paramlist[0] int，摇晃的类型（1为剧烈摇晃，2为轻轻摇晃）
        /// </summary>
        public const sbyte camerashake = 1;

        private static void Convert_camerashake(ref Token result, string[] paramsList)
        {
            result.tokenType = camerashake;
            result.paramlist = new object[1] { System.Convert.ToInt32(paramsList[0]) }; ;//注意这里要做类型转换
        }
    }
}

