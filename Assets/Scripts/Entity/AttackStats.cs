[System.Serializable]
public class AttackStats
{
    public enum Axis
    {
        NONE = 0,
        X,
        Z
    }

    public string name = "";
    public Entity damageProvider = null;
    public AttackType type = AttackType.LIGHT;
    public LaunchDirection launchDirection = LaunchDirection.NO_LAUNCH;
    public Axis axis = Axis.NONE;
    public float horizontalDir = 1f;
    public float targetPosition = 0f;
    public int damage = 0;
    public int stunDuration = 0;

    public AttackStats()
    {
    }

    public AttackStats(AttackStats source)
    {
        name = source.name;
        damageProvider = source.damageProvider;
        type = source.type;
        launchDirection = source.launchDirection;
        axis = source.axis;
        horizontalDir = source.horizontalDir;
        targetPosition = source.targetPosition;
        damage = source.damage;
        stunDuration = source.stunDuration;
    }
}

