using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace naxokit.Screens
{
    public class Premium
    {
        public static void HandlePremiumOpend()
        {
            EditorGUILayout.LabelField("Premium", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Comming Soon...", EditorStyles.centeredGreyMiniLabel);
        }
    }
}
