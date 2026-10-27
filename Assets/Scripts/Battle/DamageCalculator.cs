using System;

public static class DamageCalculator
{
    public static int CalculateDamage(int attack, float power, int shell)
    {
        int hit = (int)Math.Round(attack * power, MidpointRounding.AwayFromZero);
        return Math.Max(1, hit - shell);
    }
}