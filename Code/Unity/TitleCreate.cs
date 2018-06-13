using System.Text;

namespace Wahren.Unity
{
    public static partial class FileCreator
    {
        public static string TitleFunction(this ContextData context)
        {
            var buf = new StringBuilder()
            .Append(@"static void CreateTitleScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var canvas = new GameObject(""Canvas"", typeof(GraphicRaycaster));
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        GetRect(canvas).position = Vector3.zero;
        canvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        new GameObject(""EventSystem"", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        var cvt = canvas.transform;")
            .Append($@"
            PlayerSettings.productName = @""{context.TitleName}"";");
            buf.Append(@"
        {
            var blackBack = new GameObject(""BlackBack"");
            blackBack.transform.parent = cvt;
            GetRect(blackBack).anchoredPosition = Vector2.zero;
            {
                var image = blackBack.AddComponent<Image>();
                image.color = Color.black;
                image.raycastTarget = false;
            }
        }
        {
            var title = new GameObject(""Title"");
            title.transform.parent = cvt;
            GetRect(title).anchoredPosition = Vector2.zero;
            {
                var image = title.AddComponent<Image>();
                image.raycastTarget = false;
                image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(ImageFolder, ""title.png""));
                image.preserveAspect = true;
                image.type = Image.Type.Simple;
            }
        }
        {
            var buttonList = new GameObject(""ButtonList"");
            buttonList.transform.parent = cvt;
            AddComponentSizeFitter(buttonList);
            var blt = buttonList.transform;
            buttonList.AddComponent<WindowHolder>().Window = AssetDatabase.LoadAssetAtPath<GameObject>(""Assets/Scenes/TitleSceneScripts/DescriptionPanel.prefab"");
            {
                var rect = buttonList.GetComponent<RectTransform>();
                rect.pivot = new Vector2(0.5f, 1);");
            if (context.TitleMenuRight < 0)
            {
                if (context.TitleMenuTop < 0)
                    buf.Append("\nrect.anchorMin = rect.anchorMax = new Vector2(").Append(1 + context.TitleMenuRight * 0.01f).Append("f,").Append(1 + context.TitleMenuTop * 0.01f).Append("f);\nrect.anchoredPosition = Vector2.zero;\n");
                else
                    buf.Append("\nrect.anchorMin = rect.anchorMax = new Vector2(").Append(1 + context.TitleMenuRight * 0.01f).Append("f, 1);\nrect.anchoredPosition = new Vector2(").Append(-context.TitleMenuRight).Append(",0);\n");
            }
            else
            {
                if (context.TitleMenuTop < 0)
                    buf.Append("\nrect.anchorMin = rect.anchorMax = new Vector2(1, ").Append(1 + context.TitleMenuTop * 0.01f).Append("f);\nrect.anchoredPosition = Vector2.zero;\n");
                else
                    buf.Append("\nrect.anchorMin = rect.anchorMax = new Vector2(1, 1);\nrect.anchoredPosition = new Vector2(").Append(-context.TitleMenuRight).Append(',').Append(-context.TitleMenuTop).Append(");\n");
            }
            buf.Append("}var list2 = new string[]{");
            for (int i = 0; i < context.ModeText.Length; i++)
                buf.Append("\"").Append(context.ModeText[i].SimpleEscape()).Append("\",\n");
            buf.AppendLine("};");
            buf.Append(@"var list = new string[] { ""Easy"", ""Normal"", ""Hard"", ""Luna"", ""Continue"", ""Tool"" };
            Add<VerticalLayoutGroup>(buttonList, 30, 30);
            for (int i = 0; i < 6; i++)
            {
                var btn = new GameObject(list[i]);
                btn.transform.parent = blt;
                AddComponentSizeFitter(btn);
                var image = btn.AddComponent<Image>();
                image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(ImageFolder, list[i].ToLower() + "".png""));
                image.type = Image.Type.Simple;
                switch (i)
                {
                    case 4:
                        btn.AddComponent<ContinueButton>().Template = AssetDatabase.LoadAssetAtPath<Canvas>(""Assets/Scenes/TitleSceneScripts/ContinueMenu.prefab"");
                        break;
                    case 5:
                        btn.AddComponent<ToolButton>().Template = AssetDatabase.LoadAssetAtPath<Canvas>(""Assets/Scenes/TitleSceneScripts/ToolMenu.prefab"");
                        break;
                    default:
                        var td = btn.AddComponent<TitleDifficulty>();
                        td.Difficulty = (byte)i;
                        td.Description = list2[i];
                        td.image = image;
                        break;
                }
            }
            EditorSceneManager.SaveScene(scene, Path.Combine(SceneFolder, ""TitleScene.unity""));
        }
    }");
            return buf.ToString();
        }
    }
}