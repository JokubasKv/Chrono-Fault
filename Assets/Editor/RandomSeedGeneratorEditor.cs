using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Seed), true)]
public class RandomSeedGeneratorEditor : Editor
{
    Seed generator;

    private void Awake()
    {
        generator = (Seed)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Set Seed"))
        {
            generator.SetSeed();
        }
        if (GUILayout.Button("Generate Game Seed"))
        {
            generator.GenerateGameSeed();
        }
    }
}
