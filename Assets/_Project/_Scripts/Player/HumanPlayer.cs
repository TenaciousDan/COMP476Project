using UnityEngine;

using Game.UI;

public class HumanPlayer : AbstractPlayer
{
    [SerializeField] private PlayerHUD hud;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void MainPhase()
    {
        if (Phase != EPlayerPhase.End)
            Phase = EPlayerPhase.Main;
    }
}
