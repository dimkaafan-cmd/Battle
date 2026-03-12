using Scripts.Data;
using UnityEngine;

public static class UnitFactory
{
    public static UnitStats CreateStats(UnitConfig    baseConfig,
                                        ShapeModifier shape,
                                        SizeModifier  size,
                                        ColorModifier color)
    {
        int   hp     = baseConfig.baseHP + shape.hpDelta + size.hpDelta + color.hpDelta;
        int   atk    = baseConfig.baseATK + shape.atkDelta + color.atkDelta;
        float spd    = baseConfig.baseSpeed + color.speedDelta;
        float atkSpd = baseConfig.baseAtkSpd + color.atkspdDelta;

        return new UnitStats
        {
            MaxHP     = Mathf.Max(1, hp),
            CurrentHP = Mathf.Max(1, hp),
            ATK       = Mathf.Max(1, atk),
            Speed     = spd,
            AtkSpd    = Mathf.Max(0.1f, atkSpd)
        };
    }

    public static UnitIdentity CreateIdentity(ShapeModifier shape,
                                               SizeModifier  size,
                                               ColorModifier color) =>
        new() { shape = shape.shape, size = size.size, color = color.unitColor };
}
