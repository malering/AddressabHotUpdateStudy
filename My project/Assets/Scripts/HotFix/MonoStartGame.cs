using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonoStartGame : MonoBehaviour
{
    public Text text;
    
    public void GameStart()
    {
        text.text = "游戏开始";
    }
}