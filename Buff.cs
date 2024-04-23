using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public float baseSpeed;
    public float baseAttack;
    public float baseDefense;
    public float perSpeed;
    public float perAttack;
    public float perDefense;
    public int turns;
    public int target;

    public Buff() {
        baseSpeed = 0;
        baseAttack = 0;
        baseDefense = 0;
        perSpeed = 0;
        perAttack = 0;
        perDefense = 0;
        turns = 0;
        target = 0;
    }

    public Buff(int target, float baseSpeed, float baseAttack, float baseDefense, float perSpeed, float perAttack, float perDefense, int turns) {
        this.target = target;
        this.baseSpeed = baseSpeed;
        this.baseAttack = baseAttack;
        this.baseDefense = baseDefense;
        this.perSpeed = perSpeed;
        this.perAttack = perAttack;
        this.perDefense = perDefense;
        this.turns = turns;
    }
}
