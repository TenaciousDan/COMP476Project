using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PU_Random_Script", menuName = "ScriptableBase/PU_Random", order = 1)]
public class PU_Random : Scriptable_Base
{
    private int randomIndex;

    [SerializeField] private List<Scriptable_Base> powerUps;

    public override void OnPowerUpGet(AbstractPlayer player)
    {
        randomIndex = Random.Range(0, powerUps.Count);
        player.Inventory.AddItem(powerUps[randomIndex]);
    }

    public override void OnPowerUpUse(AbstractPlayer player)
    {
        powerUps[randomIndex].OnPowerUpUse(player);
    }
}
