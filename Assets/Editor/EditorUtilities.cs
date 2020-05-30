using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorUtilities
{
    [MenuItem("Utilities/Clear Playerprefs")]
    static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
