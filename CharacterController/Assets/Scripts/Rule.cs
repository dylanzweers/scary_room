using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class Rule
{
    public Func<bool> condition;
    public IEnumerator coroutine;
    public delegate void action();
    public action start;
    public action stop;
    private string name = "test";
    public bool active = false;
    public int prioriteit;

    /**
    *@brief Constructor
    *@param aConditon Func<bool> the condition for the rule if this is true the rule wil be activated.
    *@param aCoroutine the coroutine that wil be started if the condition of the rule is true.
    *@param aPrioriteit the priority of the rule.
    *@param aStart the start function that wil be called if the condition of the rule is true.
    *@param aStop the stop function that wil be called if the condition of the rule becomes false.
    *@param aName the name of the rule.
    */
    public Rule(Func<bool> aCondition, IEnumerator aCoroutine, int aPrioriteit, action aStart, action aStop, String aName = "rule")
    {
        name = aName;
        condition = aCondition;
        coroutine = aCoroutine;
        prioriteit = aPrioriteit;
        start = aStart;
        stop = aStop;
    }
}

public enum MovementAction
{
    JUMPING,
    WALKNG,
    CLIMBING,
    SPRINTING,
    SWIMMING,
    FALLING
}
