using UnityEngine;

public interface IPlayerTurnStrategy
{
    public void doMainPhase();
    public void computePath();
    public void useItem();
}
