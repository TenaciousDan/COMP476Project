using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Tenacious.Collections;

using Game.UI;

public class HumanPlayer : AbstractPlayer
{
    [SerializeField] private Transform hudParent;

    public PlayerHUD hud;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        hud.gameObject.SetActive(false);   
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
