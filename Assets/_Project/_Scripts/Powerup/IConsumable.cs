using UnityEngine;

public interface IConsumable
{
    /// <summary>
    /// Uses and consumes the consumable.
    /// </summary>
    public void Use();

    public bool IsUsed();

    public void SetUsed(bool used);
}
