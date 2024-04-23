using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyBuff : PartyBuffInterface
{
    public List<Buff> buffs;

    public PartyBuff() {}

    public float getAttackBuff()
    {
        return 0;
    }

    public float getDefenseBuff()
    {
        return 0;
    }

    public float getFlatSPD()
    {
        return 0;
    }

    public float getMagicBuff()
    {
        return 0;
    }

    public float getPerSPD()
    {
        return 0;
    }
}
