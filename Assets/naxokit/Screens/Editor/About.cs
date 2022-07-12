using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace naxokit.Screens
{
    public class About
    {
        public static void HandleAboutOpend()
        {
            EditorGUILayout.LabelField("About", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Comming Soon...", EditorStyles.centeredGreyMiniLabel);
        }

    }
}
