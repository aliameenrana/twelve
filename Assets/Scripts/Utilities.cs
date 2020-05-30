using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    public static IEnumerator Run(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }
}
