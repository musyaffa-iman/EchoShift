using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomWalkParameters_", menuName = "Data/RandomWalkData")]
public class RandomWalkData : ScriptableObject
{
    public int iterations = 10, walkLength = 10;
    public bool startRandom = true;
}
