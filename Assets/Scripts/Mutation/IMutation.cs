public interface IMutation
{
    string MutationName { get; }
    void Apply(UnitStats killer, UnitStats victim);
}
