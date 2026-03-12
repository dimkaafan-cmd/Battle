using UnityEngine;

[System.Serializable]
public class UnitStats
{
    public int   MaxHP;
    public int   CurrentHP;
    public int   ATK;
    public float Speed;
    public float AtkSpd;

    public bool IsAlive => CurrentHP > 0;

    public void TakeDamage(int damage) => CurrentHP = Mathf.Max(0, CurrentHP - damage);
}
