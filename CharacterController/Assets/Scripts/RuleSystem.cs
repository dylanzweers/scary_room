using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleSystem : MonoBehaviour
{
    [HideInInspector] public List<Rule> ruleList = new List<Rule> { };

    private List<Rule> activeRuleList = new List<Rule> { };
    private List<Rule> nonActiveRuleList = new List<Rule> { };

    protected IEnumerator RuleSystemCoroutine()
    {
        while (true)
        {
            SplitRules();
            OrderRules();
            TurnOffRules();
            TurnOnRules();
            yield return null;
        }
    }

    void SplitRules()
    {
        activeRuleList.Clear();
        nonActiveRuleList.Clear();
        foreach (Rule rule in ruleList)
        {
            if (rule != null)
            {
                if (rule.active)
                {
                    activeRuleList.Add(rule);
                }
                else
                {
                    nonActiveRuleList.Add(rule);
                }
            }
        }
    }

    void OrderRules()
    {
        activeRuleList.Sort((rule1, rule2) => rule1.prioriteit.CompareTo(rule2.prioriteit));
        nonActiveRuleList.Sort((rule1, rule2) => rule1.prioriteit.CompareTo(rule2.prioriteit));
    }

    void TurnOffRules()
    {
        foreach (Rule rule in activeRuleList)
        {
            if (rule != null)
            {
                if (!rule.condition())
                {
                    rule.active = false;
                    if (rule.coroutine != null)
                    {
                        StopCoroutine(rule.coroutine);
                    }
                    if (rule.stop != null)
                    {
                        rule.stop();
                    }
                }
            }
        }
    }

    void TurnOnRules()
    {
        foreach (Rule rule in nonActiveRuleList)
        {
            if (rule != null)
            {
                    if (rule.condition())
                    {
                        rule.active = true;
                        if (rule.coroutine != null)
                        {
                            StartCoroutine(rule.coroutine);
                        }
                        if (rule.start != null)
                        {
                            rule.start();
                        }
                    }
                

            }
        }
    }

}
