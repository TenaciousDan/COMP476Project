using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PU_Rocket_Script", menuName = "ScriptableBase/PU_Rocket", order = 1)]
public class PU_Rocket : Scriptable_Base
{
    [SerializeField]
    private RocketManager rocketObject;

    /*
    Remove comments to test hard coded target

    private GameObject _target;
    private void OnEnable()
    {
        _target = GameObject.Find("TempTarget");
    }
    */

    public override void OnPowerUpUse(AbstractPlayer player, GameObject target)
    {
        //target = _target;
        Debug.Log(player.name + " fires rocket at " + target.name);
        Instantiate(rocketObject, new Vector3(player.transform.position.x, player.transform.position.y + 4, player.transform.position.z), Quaternion.Euler(90, 0, 0)).GetComponent<RocketManager>().FireRocket(target);
    }
}
