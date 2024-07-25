using UnityEngine;
using Random = Unity.Mathematics.Random;

public static class RandomGenerator
{   public static Random GetRandomGenerator()
    {
        return new Random((uint)(float)System.DateTime.Now.TimeOfDay.TotalMilliseconds);
    }
}