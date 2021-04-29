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
        if (GameplayManager.Instance.hasInitializedPlayers)
        {
            foreach (var hud in huds)
            {
                if (hud.player != null)
                {
                    bool show = GameplayManager.Instance.usingNetwork ?
                        hud.player.photonView.IsMine && hud.player.Phase == AbstractPlayer.EPlayerPhase.Main :
                        hud.player.Phase == AbstractPlayer.EPlayerPhase.Main;

                    hud.gameObject.SetActive(show);
                }
            }
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
