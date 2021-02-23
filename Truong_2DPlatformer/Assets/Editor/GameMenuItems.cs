using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameMenuItems
{
    [MenuItem("Game/Reset")]
    public static void Reset()
    {
        var gameState = GameObject.FindObjectOfType<GameState>();
        if (gameState != null)
            gameState.Reset();
        else
            Debug.LogError($"{nameof(GameState)} not found");

       
    }
}
