using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TextTokenEngine.EXAMPLE
{
    public class TextTokenEngineTest : MonoBehaviour
    {
        public string richText = "   this is my 我的！ {wait   =3 } blood.{audiostart=sondtest,2}....hehehe{camerashake=1}he";

        private void Awake()
        {
            CustomTextPlayer player = new CustomTextPlayer(richText, null);
            StartCoroutine(player.Play(() => { Debug.Log("PLAYFINISH"); }));
        }
    }
}

