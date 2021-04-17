using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PU_Rocket_Script", menuName = "ScriptableBase/PU_Rocket", order = 1)]
public class PU_Rocket : Scriptable_Base
{
    public override string PowerUpName
    {
        get { return "Rocket"; }
    }

    public override void OnPowerUpUse(AbstractPlayer player, AbstractPlayer target)
    {
        Debug.Log(player.name + " fires rocket at " + target.name);
    }
}
