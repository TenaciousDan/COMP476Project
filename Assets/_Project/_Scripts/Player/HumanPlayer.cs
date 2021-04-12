using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Photon.Realtime;

using Tenacious.Collections;

using Game.UI;

public class HumanPlayer : AbstractPlayer
{
    [SerializeField] private PlayerHUD hud;

    public Player photonPlayer;
    [HideInInspector]
    public int ID
    {
        get; private set;
    }
    
    protected override void Awake()
    {
        base.Awake();
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

    public void InitializePlayer(Player player, float _maxActionPoints, Vector3 _positionOffset)
    {
        // Initialize Networking Variables
        photonPlayer = player;
        ID = player.ActorNumber;
        
        base.InitializePlayer(_maxActionPoints, _positionOffset);
        
        // TODO: Ensure game manager knows that player is initialized
        // GameManager.instance.players[id - 1] = this;
        
        // Need to figure out how to make this a PUN RPC? Send call back to network manager?
    }
}
