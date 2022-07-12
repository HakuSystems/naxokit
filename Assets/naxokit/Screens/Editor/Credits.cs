﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using naxokit.Styles;

namespace naxokit.Screens
{
    public class Credits
    {
        public static void HandleCreditsOpend()
        {
            var teamCreditsImage = Resources.Load("TeamCredits") as Texture2D;
            DrawLine.DrawHorizontalLine(1);
            var content = new GUIContent(teamCreditsImage);
            EditorGUILayout.LabelField(content, GUILayout.Height(300));
            DrawLine.DrawHorizontalLine(1);

        }
    }
}