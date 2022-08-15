using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonoStartGame : MonoBehaviour
{
    public Text text;
    
    public void GameStart()
    {
        if (text != null)
        {
            Debug.Log($"{text} 文本，游戏开始");
            text.text = "游戏开始";
        }
    }
}