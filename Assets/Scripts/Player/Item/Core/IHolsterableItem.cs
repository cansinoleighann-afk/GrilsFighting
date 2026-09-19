namespace AwCon
{
    /// <summary>Optional staged unequip contract, independent of the concrete weapon type.</summary>
    public interface IHolsterableItem
    {
        float HolsterDuration { get; }
        void BeginHolster(float until);
    }
}
