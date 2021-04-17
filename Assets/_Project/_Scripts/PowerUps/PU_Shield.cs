using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PU_Shield_Script", menuName = "ScriptableBase/PU_Shield", order = 1)]
public class PU_Shield : Scriptable_Base
{
    public override string PowerUpName
    {
        get { return "Shield"; }
    }

    public override void OnPowerUpUse(AbstractPlayer player)
    {
        player.ActivateShield();
    }
}
