using System;

public static class MathUtil
{
    static Random rand = new Random();

    public static float GetRandomValue(float min, float max)
    {
        return (float)(rand.NextDouble() * (max - min) + min);
    }
}