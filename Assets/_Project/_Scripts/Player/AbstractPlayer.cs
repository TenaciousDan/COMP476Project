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

    /// <summary>
    /// Will be called by GameManager in the Update() as long as Phase = Standby
    /// </summary>
    public virtual void StandbyPhase()
    {
        Phase = EPlayerPhase.Standby;

        // TODO: do standby phase things

        Phase = EPlayerPhase.Main;
    }

    /// <summary>
    /// Will be called by GameManager in the Update() as long as Phase = Main
    /// </summary>
    public abstract void MainPhase();

    /// <summary>
    /// Will be called by GameManager in the Update() as long as Phase = End
    /// </summary>
    public virtual void EndPhase()
    {
        Phase = EPlayerPhase.End;

        // TODO: do end phase things

        Phase = EPlayerPhase.Waiting;
    }


}
