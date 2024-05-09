using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSAnimLibrary : MonoBehaviour
{
    public List<AttackCategory> allAttacks = new List<AttackCategory>();

    [Header("Fight Animations")]
    public Attack defaultFightAnimation;

    [Header("Final Attack Animations")]
    public Attack defaultFinalAttackAnimation;

    [Header("Death Animations")]
    public string defaultDeathAnimation = "";
    public string defaultHitAnimation = "";
    public string defaultIdleAnimation = "";
    public string defaultBuffAnimation = "";
    public List<string> deathAnimations = new List<string>();

    float transitionTime = 0.1f;

    // Called by character to get an attack from the library stored above
    public Attack GetAttack(FightStyle activeStyle, bool isFinalHit, Animator myAnim)
    {
        switch (isFinalHit)
        {
            case true:
                for (int i = 0; i < allAttacks.Count; i++)
                {
                    switch (allAttacks[i].style == activeStyle)
                    {
                        case true:
                            int a = Random.Range(0, allAttacks[i].finalAttacks.Count);
                            myAnim.CrossFadeInFixedTime(allAttacks[i].finalAttacks[a].attack, transitionTime);
                            //myAnim.Play(allAttacks[i].finalAttacks[a].attack);
                            return allAttacks[i].finalAttacks[a];
                        case false:
                            break;
                    }
                }
                defaultFinalAttackAnimation.style = activeStyle;
                return defaultFinalAttackAnimation;
            case false:
                for (int i = 0; i < allAttacks.Count; i++)
                {
                    switch (allAttacks[i].style == activeStyle)
                    {
                        case true:
                            int b = Random.Range(0, allAttacks[i].attacks.Count);
                            myAnim.CrossFadeInFixedTime(allAttacks[i].attacks[b].attack, transitionTime);
                            //myAnim.Play(allAttacks[i].attacks[b].attack);
                            return allAttacks[i].attacks[b];
                        case false:
                            break;
                    }
                }
                defaultFightAnimation.style = activeStyle;
                return defaultFightAnimation;
        }
    }

    // Called by character to get an attack string from the library stored above
    public Attack GetAttackStr(FightStyle activeStyle, bool isFinalHit)
    {
        switch (isFinalHit)
        {
            case true:
                for (int i = 0; i < allAttacks.Count; i++)
                {
                    switch (allAttacks[i].style == activeStyle)
                    {
                        case true:
                            int a = Random.Range(0, allAttacks[i].finalAttacks.Count);
                            return allAttacks[i].finalAttacks[a];
                        case false:
                            break;
                    }
                }
                defaultFinalAttackAnimation.style = activeStyle;
                return defaultFinalAttackAnimation;
            case false:
                for (int i = 0; i < allAttacks.Count; i++)
                {
                    switch (allAttacks[i].style == activeStyle)
                    {
                        case true:
                            int b = Random.Range(0, allAttacks[i].attacks.Count);
                            return allAttacks[i].attacks[b];
                        case false:
                            break;
                    }
                }
                defaultFightAnimation.style = activeStyle;
                return defaultFightAnimation;
        }
    }
}

[System.Serializable]
public class AttackCategory
{
    public string name;
    public FightStyle style;
    public List<Attack> attacks = new List<Attack>();
    public List<Attack> finalAttacks = new List<Attack>();
}
