using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Tenacious.Collections;

using Game.UI;

public class HumanPlayer : AbstractPlayer
{
    [SerializeField] private GameObject hudPrefab;
    [SerializeField] private PlayerHUD hud;
    [SerializeField] private Transform hudParent;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        //hud = FindObjectOfType<PlayerHUD>(); // To change
        var hudObj = Instantiate(hudPrefab, PlayerHUDManager.Instance.playerHudParent);
        //print(hudObj.transform.position);
        //hudObj.SetActive(true);
        hud = hudObj.GetComponent<PlayerHUD>();
        hud.player = this;
    }

    /// <summary>
    /// Will be called by GameManager in the Update() as long as Phase = Main
    /// </summary>
    public override void MainPhaseUpdate()
    {
        Phase = EPlayerPhase.Main;
    }

    public void Move(List<GraphNode<GameObject>> path)
    {
        StartCoroutine(CRMove(path));
    }
}
