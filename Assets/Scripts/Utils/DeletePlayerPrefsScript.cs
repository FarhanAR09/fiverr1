#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class DeletePlayerPrefsScript : EditorWindow
{
    [MenuItem("Window/Delete PlayerPrefs (All)")]
    static void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
#endif