using System.Text;
using System.Collections.Generic;
using static Farmhash.Sharp.Farmhash;

namespace Wahren.Analysis.Unity
{
    public static partial class FileCreator
    {
        public static string ScenarioChoiceFunction()
        {
            var buf = new StringBuilder();
            buf.Append(@"static void CreateScenarioChoiceScene()
{
    var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    var canvas = new GameObject(""Canvas"", typeof(GraphicRaycaster));
    canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
    canvas.GetRect().position = Vector3.zero;
    canvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    new GameObject(""EventSystem"", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
    {
        var blackBack = new GameObject(""BlackBack"");
        blackBack.transform.parent = canvas.transform;
        blackBack.AddComponent<BackToTitle>();
        blackBack.GetRect().anchoredPosition = Vector2.zero;
        {
            var image = blackBack.AddComponent<Image>();
            image.color = Color.black;
            image.raycastTarget = false;}}
            {
                var bg = new GameObject(""BackGround"", typeof(Image));
                bg.transform.SetParent(canvas.transform);
                bg.AddComponent<BackToTitle>();
                bg.AddComponent<BackGroundInitialization>().pre = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(ImageFolder + ""preAtlas.spriteatlas"");
                bg.GetRect().anchoredPosition = Vector2.zero;}
                {
                    var layout = new GameObject(""HorizontalLayoutPanel"");
                    layout.transform.parent = canvas.transform;
                    layout.GetRect();
                    layout.AddComponent<BackToTitle>();
                    {
                        var hor = layout.Add<HorizontalLayoutGroup>(16);
                        hor.childControlWidth = hor.childControlHeight = hor.childForceExpandHeight = true;}
                        var scrollPanel = AssetDatabase.LoadAssetAtPath<GameObject>(SceneFolder+""ChooseScenarioScripts/ScrollPanel.prefab"");
                        {
                            var v = new GameObject(""VerticalLayoutPanel"");
                            v.transform.parent = layout.transform;
                            v.AddComponent<BackToTitle>();
                            v.Add<VerticalLayoutGroup>(0, childAlignment: TextAnchor.UpperLeft).childControlWidth = true;
                            var descriptionList = new string[] { ""シナリオ選択");
            if (ScriptLoader.Context.ScenarioSelect2On)
                buf.Append("\",\"").Append(ScriptLoader.Context.ScenarioSelect2.StringEscape());
            buf.Append("\" };\nvar scenarios = new(string name, string id, string desc)[][]{");
            if (ScriptLoader.Context.ScenarioSelect2On)
            {
                var list1 = new SortedList<int, SortedList<ulong, (string, string, string)>>();
                var list2 = new SortedList<int, SortedList<ulong, (string, string, string)>>();
                var scs = ScriptLoader.Scenarios;
                for (int i = 0; i < scs.Length; i++)
                {
                    var sc = scs[i].Scenario;
                    var key = sc.SortKey;
                    if (key.HasValue && key.Value < 0)
                    {
                        if (list1.TryGetValue(key.Value, out var s))
                            s.Add(Hash64(sc.Name), (sc.DisplayName.StringEscape(), sc.Name, sc.DescriptionText.Replace("、$", "、").StringEscape()));
                        else
                        {
                            var tmp = new SortedList<ulong, (string, string, string)>();
                            tmp.Add(Hash64(sc.Name), (sc.DisplayName.StringEscape(), sc.Name, sc.DescriptionText.Replace("、$", "、").StringEscape()));
                            list1.Add(key.Value, tmp);
                        }
                    }
                    else
                    {
                        if (list2.TryGetValue(key ?? 0, out var s))
                            s.Add(Hash64(sc.Name), (sc.DisplayName.StringEscape(), sc.Name, sc.DescriptionText.Replace("、$", "、").StringEscape()));
                        else
                        {
                            var tmp = new SortedList<ulong, (string, string, string)>();
                            tmp.Add(Hash64(sc.Name), (sc.DisplayName.StringEscape(), sc.Name, sc.DescriptionText.Replace("、$", "、").StringEscape()));
                            list2.Add(key ?? 0, tmp);
                        }
                    }
                }
                buf.Append("new (string,string,string)[]{");
                foreach (var collection in list1.Values)
                    foreach (var (name, id, desc) in collection.Values)
                        buf.Append("(\"").Append(name).Append("\",\"").Append(id).Append("\",\"").Append(desc).Append("\"),\n");
                buf.Append("},\nnew (string,string,string)[]{");
                foreach (var collection in list2.Values)
                    foreach (var (name, id, desc) in collection.Values)
                        buf.Append("(\"").Append(name).Append("\",\"").Append(id).Append("\",\"").Append(desc).Append("\"),\n");
            }
            else
            {
                var list = new SortedList<int, SortedList<ulong, (string, string, string)>>();
                var scs = ScriptLoader.Scenarios;
                for (int i = 0; i < scs.Length; i++)
                {
                    var sc = scs[i].Scenario;
                    var key = sc.SortKey ?? 0;
                    if (list.TryGetValue(key, out var s))
                        s.Add(Hash64(sc.Name), (sc.DisplayName.StringEscape(), sc.Name, sc.DescriptionText.Replace("、$", "、").StringEscape()));
                    else
                    {
                        var tmp = new SortedList<ulong, (string, string, string)>();
                        tmp.Add(Hash64(sc.Name), (sc.DisplayName.StringEscape(), sc.Name, sc.DescriptionText.Replace("、$", "、").StringEscape()));
                        list.Add(key, tmp);
                    }
                }
                foreach (var collection in list.Values)
                    foreach (var (name, id, desc) in collection.Values)
                        buf.Append("(\"").Append(name).Append("\",\"").Append(id).Append("\",\"").Append(desc).Append("\"),\n");
            }
            buf.Append(@"}
            };
            GameObject CreateNode((string name, string id, string desc) val)
            {
                var node = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(SceneFolder+""ChooseScenarioScripts/Node.prefab""));
                node.GetComponentInChildren<Text>().text = val.name;
                var sc = node.GetComponent<ChooseScenario>();
                sc.ScenarioName = val.name;
                sc.ScenarioIdentifier = val.id;
                sc.ScenarioDescription = val.desc;
                return node;
            }
            for (int i = 0; i< descriptionList.Length; i++)
            {
                var sc = Instantiate(scrollPanel);
                sc.transform.SetParent(v.transform);
                sc.GetRect(0,");
            if (ScriptLoader.Context.ScenarioSelect2On)
                buf.Append("i==0 ? ").Append(5.76 * ScriptLoader.Context.ScenarioSelect2Percentage).Append("f : ").Append(5.76 * (100 - ScriptLoader.Context.ScenarioSelect2Percentage)).Append("f);\n");
            else
                buf.Append("576);\n");
            buf.Append(@"sc.GetComponentInChildren<Text>().text = descriptionList[i];
            var tmpParent = sc.transform.Find(""Scroll View"").Find(""Viewport"").Find(""Content"");
            for (int j = 0; j<scenarios[i].Length; j++)
                CreateNode(scenarios[i][j]).transform.SetParent(tmpParent);
            }
        }
        {
            var descriptionPanel = Instantiate(scrollPanel);
            descriptionPanel.name = ""Description Panel"";
            descriptionPanel.transform.SetParent(layout.transform);
            descriptionPanel.GetRect(0, 576);
            descriptionPanel.GetComponentInChildren<Text>().text = """";
            {
                var scrollRect = descriptionPanel.GetComponentInChildren<ScrollRect>();
                scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
                scrollRect.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 500);
                scrollRect.scrollSensitivity = 5;
            }
            var content = descriptionPanel.GetComponentInChildren<RectMask2D>().GetComponentInChildren<VerticalLayoutGroup>().gameObject;
            GameObject.DestroyImmediate(content.GetComponent<VerticalLayoutGroup>());
            content.AddComponent<Text>().raycastTarget = false;
        }
    }
    EditorSceneManager.SaveScene(scene, Path.Combine(SceneFolder, ""ChooseScenarioScene.unity""));
}");
            return buf.ToString();
        }
    }
}