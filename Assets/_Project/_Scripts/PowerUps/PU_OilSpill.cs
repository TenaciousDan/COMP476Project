using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PU_OilSpill_Script", menuName = "ScriptableBase/PU_OilSpill", order = 1)]
public class PU_OilSpill : Scriptable_Base
{
    [SerializeField]
    private OilSpillManager oilSpillObject;

    public override void OnPowerUpUse(AbstractPlayer player, GameObject target = null)
    {
        OilSpillManager oilSpill = Instantiate(oilSpillObject, new Vector3(target.transform.position.x, 0.1f, target.transform.position.z), Quaternion.identity).GetComponent<OilSpillManager>();
        oilSpill.transform.parent = target.transform;
    }
}
