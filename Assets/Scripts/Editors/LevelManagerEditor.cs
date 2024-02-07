#if UNITY_EDITOR
using UnityEngine;
using UnityEditor; // Import the UnityEditor namespace

[CustomEditor(typeof(LevelManager))] // Specify the target class this editor is for
public class LevelManagerEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        DrawDefaultInspector(); // Draw the default inspector

        LevelManager myScript = (LevelManager)target; // Get a reference to the LevelManager script

        // Add a button to the inspector
        if (GUILayout.Button("Delete/Reset Save JSON"))
        {
            // Confirm with the user before deleting the save file
            if (EditorUtility.DisplayDialog("Delete Save JSON",
                    "Are you sure you want to delete the save JSON file? This action cannot be undone.", "Delete", "Cancel"))
            {
                myScript.DeleteSaveFile(); // Call the method to delete the save file
            }
        }
    }
}
#endif
