using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PU_Random_Script", menuName = "ScriptableBase/PU_Random", order = 1)]
public class PU_Random : Scriptable_Base
{
    public List<Scriptable_Base> powerUps;

    public override void OnPowerUpGet(AbstractPlayer player, int randomIndex)
    {
        player.Inventory.AddItem(powerUps[randomIndex]);
    }
}
