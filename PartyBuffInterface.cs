public interface PartyBuffInterface
{
    // public static GameEvent startBuff 이런 식으로 맨 처음에 배틀 시작되면 이거롤 전부
    public float getPerSPD();
    public float getFlatSPD();
    public float getAttackBuff();
    public float getDefenseBuff();
    public float getMagicBuff();

}