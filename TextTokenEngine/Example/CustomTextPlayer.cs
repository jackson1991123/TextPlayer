
/*例子：testString 为“   this is my 我的！ {wait   =3 } blood.{audiostart=sondtest,2}....hehehehe”
TextPlayerSolutionA su = new TextPlayerSolutionA(testString, source);
StartCoroutine(su.Play(()=> { Debug.Log("finish"); }));*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TextTokenEngine.EXAMPLE
{
    using TextTokenEngine.Core;

    /// <summary>
    /// 例子：实现一个真正的执行器，给你token你去异步播放
    /// </summary>
    public class CustomTextPlayer : TextPlayer
    {
        /// <summary>
        /// 音频播放源
        /// </summary>
        public AudioSource audioSource { get; private set; }

        /// <summary>
        /// 解决方案A
        /// 实现了 word ：debug log形式输出
        /// 实现了 audiostart ：加载resource下的音频
        /// 实现了 wait ：等待延迟
        /// </summary>
        public CustomTextPlayer(string richText, AudioSource audioSource) : base(richText)
        {
            this.audioSource = audioSource;
            this.Add(TokenType.word, Execute_Word);
            this.Add(TokenType.wait, Execute_Wait);
            this.Add(TokenType.audiostart, Execute_AudioStart);

            this.Add(TokenType.camerashake, Execute_CameraShake);
        }

        private IEnumerator Execute_AudioStart(Token token)
        {
            if (audioSource != null)
            {
                AudioClip clip = Resources.Load<AudioClip>(token.content);
                audioSource.clip = clip;
                audioSource.Play();
                audioSource.volume = (float)(token.paramlist[0]);
            }
            yield break;
        }

        private IEnumerator Execute_Word(Token token)
        {
            Debug.Log(token.content);
            yield break;
        }

        private IEnumerator Execute_Wait(Token token)
        {
            yield return new WaitForSeconds((float)token.paramlist[0]);
        }

        private IEnumerator Execute_CameraShake(Token token)
        {
            Debug.Log("摄像机摇晃，1秒后打出摇晃类型的log");
            yield return new WaitForSeconds(1);
            Debug.Log(token.paramlist[0]);
        }
    }
}

