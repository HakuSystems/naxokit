using System.Collections.Generic;
using UnityEditor;

namespace naxokit.Screens
{
    public class Credits : EditorWindow
    {
        public static void HandleCreditsOpend()
        {
            EditorGUILayout.LabelField("Developers", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.LabelField("Everspace", EditorStyles.foldout);
            EditorGUILayout.LabelField("lyze", EditorStyles.foldout);
            EditorGUILayout.LabelField("Texotek", EditorStyles.foldout);
            EditorGUILayout.LabelField("SupportTeam", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.LabelField("ZKWolf", EditorStyles.foldout);
            EditorGUILayout.LabelField("SlySnake96", EditorStyles.foldout);
            EditorGUILayout.LabelField("enemy of the state(ukeid)", EditorStyles.foldout);

        }
    }
}
