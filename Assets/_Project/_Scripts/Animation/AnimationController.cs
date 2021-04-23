using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private List<Animator> CopCars;

    public void TriggerAllCopAnimations()
    {
        foreach(Animator anim in CopCars)
        {
            anim.SetBool("CanLeave", true);
        }
    }
}
