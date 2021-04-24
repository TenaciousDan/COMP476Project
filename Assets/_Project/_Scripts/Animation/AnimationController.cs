using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public void LiftAllCopCars() => GetComponent<Animator>().SetBool("CanLeave", true);
    public void LandAllCopCars() => GetComponent<Animator>().SetBool("CanLeave", false);
}
