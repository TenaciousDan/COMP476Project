using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public abstract class AbstractPlayer : MonoBehaviour
{
    public enum EPlayerPhase { Standby, Main, End }

    public EPlayerPhase Phase { get; set; }

    private PlayerController controller;
    public PlayerController Controller { get => controller; set => controller = value; }

    protected virtual void Awake()
    {
        controller = GetComponent<PlayerController>();
        Phase = EPlayerPhase.End;
    }

    public virtual void StandbyPhase()
    {
        Phase = EPlayerPhase.Standby;
    }

    public virtual void MainPhase()
    {
        Phase = EPlayerPhase.Main;
    }

    public virtual void EndPhase()
    {
        Phase = EPlayerPhase.End;
    }
}
