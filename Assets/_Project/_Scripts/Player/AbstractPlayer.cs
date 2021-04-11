using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public abstract class AbstractPlayer : MonoBehaviour
{
    public enum EPlayerPhase { Waiting, Standby, Main, End }

    public EPlayerPhase Phase { get; set; }

    private PlayerController controller;
    public PlayerController Controller 
    { 
        get
        {
            if (controller == null)
                controller = GetComponent<PlayerController>();
            return controller;
        }
        set => controller = value; 
    }

    protected virtual void Awake()
    {
        //
    }

    public virtual void StandbyPhase()
    {
        if (Phase != EPlayerPhase.Waiting)
            Phase = EPlayerPhase.Standby;

        // TODO: do standby phase things

        Phase = EPlayerPhase.Main;
    }

    public abstract void MainPhase();

    public virtual void EndPhase()
    {
        if (Phase != EPlayerPhase.Main)
            Phase = EPlayerPhase.End;

        // TODO: do end phase things

        Phase = EPlayerPhase.Waiting;
    }
}
