public class Win : TriggerAbstract
{
    protected override void TriggerEnter()
    {
        ServiceLocator.Instance.GetService<GameManager>(ManagerType.GAME_MANAGER).Win();
        DesactivateTrigger();
    }
}
