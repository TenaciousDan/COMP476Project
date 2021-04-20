using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;

using Tenacious.Collections;

using Game.UI;

public class HumanPlayer : AbstractPlayer
{
    [SerializeField] private Transform hudParent;

    public PlayerHUD hud;

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

    private void Start()
    {
        hud.gameObject.SetActive(false);
    }

    [PunRPC]
    public override void InitializePlayer(float _maxActionPoints, Vector3 _positionOffset, string _startingNodeId, string name)
    {
        base.InitializePlayer(_maxActionPoints, _positionOffset, _startingNodeId, name);

        //PlayerHUDManager.Instance.InitializeUI(this);
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
    
    #region NETWORK

    [PunRPC]
    public void InitializePlayerOnNetwork(Player player)
    {
        photonPlayer = player;
        ID = player.ActorNumber;
        NetworkManager.Instance.humanPlayers[ID - 1] = this;

        // Only track local physics
        if (!photonView.IsMine)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    
    #endregion
}
