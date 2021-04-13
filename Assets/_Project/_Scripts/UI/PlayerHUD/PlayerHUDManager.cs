using Game.UI;
using System.Collections;
using System.Collections.Generic;
using Tenacious;
using UnityEngine;

public class PlayerHUDManager : MBSingleton<PlayerHUDManager>
{
    [SerializeField] private List<PlayerHUD> huds;
    public Transform playerHudParent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
