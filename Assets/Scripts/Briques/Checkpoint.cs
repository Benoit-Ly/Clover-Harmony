using UnityEngine;

public class Checkpoint : TriggerAbstract
{
    public Transform playerOne;
    public Transform playerTwo;

    protected override void TriggerEnter()
    {
        ServiceLocator.Instance.GetService<GameManager>(ManagerType.GAME_MANAGER).RegisterCurrentCheckpoint(this);
        DesactivateTrigger();
    }
}
