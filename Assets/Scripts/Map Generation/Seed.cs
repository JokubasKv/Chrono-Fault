using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Seed : MonoBehaviour
{
    [SerializeField] public string gameSeed = "Bruh";
    [SerializeField] public int currentSeed = 0;

    const string glyphs = "abcdefgjklmnopqrstuvwxyz0123456789";

    public void SetSeed()
    {
        currentSeed = gameSeed.GetHashCode();
        Random.InitState(currentSeed);
    }
    public void GenerateGameSeed()
    {
        string newSeed="";
        for (int i = 0; i < 6; i++)
        {
            newSeed += glyphs[Random.Range(0, glyphs.Length)];
        }
        gameSeed = newSeed;
    }
}
