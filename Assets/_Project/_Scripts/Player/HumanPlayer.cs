using UnityEngine;

using Game.UI;

public class HumanPlayer : AbstractPlayer
{
    [SerializeField] private PlayerHUD hud;

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Will be called by GameManager in the Update() as long as Phase = Main
    /// </summary>
    public override void MainPhase()
    {
        Phase = EPlayerPhase.Main;
    }
}
