using System;

namespace Wahren.Unity
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
public class ChooseScene : EditorWindow
{
    const string ImageFolder = ""Assets/Image/"";
    const string SceneFolder = ""Assets/Scenes/"";

    [MenuItem(""Wahren/Create Scenes"")]
    private static void Create()
    {
        CreateTitleScene();
        CreateScenarioChoiceScene();
    }
    static void AddComponentSizeFitter(GameObject obj)
    {
        var csf = obj.AddComponent<ContentSizeFitter>();
        csf.horizontalFit = csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }
    static T Add<T>(GameObject obj, int padding, int spacing = 0, TextAnchor childAlignment = TextAnchor.MiddleCenter)
        where T : HorizontalOrVerticalLayoutGroup
    {
        var answer = obj.AddComponent<T>();
        answer.padding = new RectOffset(padding, padding, padding, padding);
        answer.spacing = spacing;
        answer.childAlignment = childAlignment;
        answer.childForceExpandHeight = answer.childForceExpandWidth = false;
        return answer;
    }
    static RectTransform GetRect(GameObject obj, float width = 800, float height = 600)
    {
        var rect = obj.GetComponent<RectTransform>();
        if (rect == null)
            rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, height);
        return rect;
    }";
        public static string End => @"}
#endif";
    }
}