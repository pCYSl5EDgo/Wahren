using System;

namespace Wahren.Analysis.Unity
{
    public static partial class FileCreator
    {
        public static string Start => @"using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using UnityEngine.U2D;
using System.IO;
using UnityEditor;
#if UNITY_EDITOR
namespace Wahren.Analysis{
    public class ChooseScene : EditorWindow
    {
        const string ImageFolder = ""Assets/Image/"";
        const string SceneFolder = ""Assets/Scenes/"";
        [MenuItem(""Wahren/Create Scenes"")]
        private static void Create()
        {
            CreateTitleScene();
            CreateScenarioChoiceScene();
        }";
        public static string End => @"  }
}
#endif";
    }
}