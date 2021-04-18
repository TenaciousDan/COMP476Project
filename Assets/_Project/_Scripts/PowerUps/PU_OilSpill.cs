using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PU_OilSpill_Script", menuName = "ScriptableBase/PU_OilSpill", order = 1)]
public class PU_OilSpill : Scriptable_Base
{
    //public override string PowerUpName
    //{
    //    get { return "OilSpill"; }
    //}

    private int numActionPointsToRemove = 2;

    public override void OnPowerUpGet(AbstractPlayer player)
    {
        player.RemoveActionPoints(numActionPointsToRemove);
    }

    public override void OnPowerUpUse(AbstractPlayer player)
    {
        throw new System.NotImplementedException();
    }
}
