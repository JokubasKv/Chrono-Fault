using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsManager : MonoSingleton<LevelsManager>
{
    //[SerializeField] public static Action ResetValues;

    [SerializeField] public int corridorLength = 14;
    [SerializeField] public int corridorCount = 5;
    [SerializeField][Range(0f, 1f)] public float roomPercent = 0.6f;

    [SerializeField] public float EnemyHealth = 10;
    [SerializeField] public int MinEnemies = 0;
    [SerializeField] public int MaxEnemies = 10;
    private int currentLevel = 1;

    public void ResetGame()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        Destroy(player);
        SceneManager.LoadScene("SampleScene");
    }

    public void NextLevel()
    {
        currentLevel++;

        corridorCount+=2;

        EnemyHealth += 10;

        MinEnemies++;
        MaxEnemies++;

        SceneManager.LoadScene("SampleScene");
    }

    protected override void InternalInit()
    {
    }

    protected override void InternalOnDestroy()
    {
    }
}
