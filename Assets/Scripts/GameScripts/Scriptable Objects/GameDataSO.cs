using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameDataSO : ScriptableObject
{
    [SerializeField] private bool isCrystalCollected = false;
    [SerializeField] private bool isGameStarted = false;

    public bool CrystalCollected {
        get { return isCrystalCollected; }
        set { isCrystalCollected = value; }
    }

    public bool GameStarted
    {
        get { return isGameStarted; }
        set { isGameStarted = value; }
    }
}
