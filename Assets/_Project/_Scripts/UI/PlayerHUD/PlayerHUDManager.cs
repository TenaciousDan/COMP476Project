using Game.UI;
using System.Collections;
using System.Collections.Generic;
using Tenacious;
using UnityEngine;

public class PlayerHUDManager : MBSingleton<PlayerHUDManager>
{
    public List<PlayerHUD> huds = new List<PlayerHUD>();
    public Transform playerHudParent;
    [SerializeField] private GameObject hudPrefab;

    // Update is called once per frame
    void Update()
    {
        foreach (var hud in huds)
        {
            //hud.gameObject.SetActive(hud.player.Phase == AbstractPlayer.EPlayerPhase.Main);
            
            hud.gameObject.SetActive(hud.player.photonView.IsMine && hud.player.Phase == AbstractPlayer.EPlayerPhase.Main);
        }
    }

    public void InitializeUI(HumanPlayer player)
    {
        var hudObj = Instantiate(hudPrefab, playerHudParent);
        player.hud = hudObj.GetComponent<PlayerHUD>();
        huds.Add(player.hud);
        player.hud.player = player;
    }
}
