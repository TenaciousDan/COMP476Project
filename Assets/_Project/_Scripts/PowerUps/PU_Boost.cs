using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PU_Boost_Script", menuName = "ScriptableBase/PU_Boost", order = 1)]
public class PU_Boost : Scriptable_Base
{
    private int numExtraActionPoints = 2;

    public override void OnPowerUpUse(AbstractPlayer player, GameObject target = null)
    {
        player.State = AbstractPlayer.EPlayerState.Busy;
        player.photonView.RPC("AddActionPoints", Photon.Pun.RpcTarget.All, (float)numExtraActionPoints, false);
        player.State = AbstractPlayer.EPlayerState.Waiting;
    }
}
