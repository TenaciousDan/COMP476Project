using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PU_Rocket_Script", menuName = "ScriptableBase/PU_Rocket", order = 1)]
public class PU_Rocket : Scriptable_Base
{
    public override void OnPowerUpUse(AbstractPlayer player, GameObject target)
    {
        PhotonNetwork.Instantiate("Rocket", new Vector3(player.transform.position.x, player.transform.position.y + 4, player.transform.position.z), Quaternion.Euler(90, 0, 0)).GetComponent<RocketManager>().FireRocket(player, target);
    }
}
