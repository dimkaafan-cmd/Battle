using Scripts.Data;

[System.Serializable]
public class UnitIdentity
{
    public UnitShape shape;
    public UnitSize  size;
    public UnitColor color;

    public override string ToString() => $"{shape} {color} {size}";
}
