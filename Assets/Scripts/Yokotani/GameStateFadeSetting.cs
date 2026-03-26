using System;
using UnityEngine;

[Serializable]
public class GameStateFadeSetting
{
    public GameState state;

    public float fadeOutTime = 1f;
    public float fadeInTime = 1f;
}