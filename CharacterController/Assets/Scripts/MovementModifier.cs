using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("movement/movmentModifier")]
public class MovementModifier : MonoBehaviour
{
    public List<MovementMod> modifiers;
}

public enum MovementMod
{
    NONCLIMABLE,
    SWIMMABLE
}

