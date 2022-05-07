using PFCToolkit.Lib;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PFCToolkit.Core
{
    public class PFCToolKitWindow : EditorWindow
    {
        private static ErrorLog log;

        private bool NeedsUpdate = true;
        private ObjectField targetField;
        private GameObject target;
        private ScrollView debug;


        [MenuItem("Tools/PFCToolkit")]
        public static void ShowWindow()
        {
            PFCToolKitWindow window = GetWindow<PFCToolKitWindow>();
            window.titleContent = new GUIContent("PFCToolkit");
            window.minSize = new Vector2(280, 50);
        }

        private void CreateGUI()
        {

            VisualElement root = rootVisualElement;

            DrawToolbar(root);

            targetField = new ObjectField();
            targetField.label = "Name:";
            targetField.objectType = typeof(GameObject);
            targetField.RegisterValueChangedCallback(target => UpdateTarget());
            root.Add(targetField);

            debug = root.AddElement<ScrollView>();

            log = new ErrorLog();
            log.ClearLog();
            log.AddNote("Hello world!", "This is a example notification!");
            root.Add(log.view);

        }

        public void OnFocus()
        {
            if (NeedsUpdate)
            {
                NeedsUpdate = false;
                GetAllData();
            }
        }

        public void OnHierarchyChange()
        {
            NeedsUpdate = true;
        }

        private void GetAllData()
        {
            if (target == null)
            {
                return;
            }

            System.DateTime startTime = System.DateTime.Now;

            System.TimeSpan Time = System.DateTime.Now.Subtract(startTime);
            Debug.Log($"Processed object: {target.name} Took {Time.TotalMinutes:0}m {Time.Seconds:00}s {Time.Milliseconds}ms");
        }

        private void DrawToolbar(VisualElement root)
        {

            Toolbar toolbar = root.AddElement<Toolbar>();

            ToolbarButton btn_refresh = toolbar.AddElement<ToolbarButton>();
            btn_refresh.text = "Refresh";
            btn_refresh.clicked += UpdateTarget;

            ToolbarSpacer spacer = toolbar.AddElement<ToolbarSpacer>();
            spacer.flex = true;

            ToolbarButton btn_sceneSelect = toolbar.AddElement<ToolbarButton>();
            btn_sceneSelect.text = "Select From Scene";
            btn_sceneSelect.clicked += SelectFromScene;
        }

        private void SelectFromScene()
        {
            log.ClearLog();
            if (Selection.activeGameObject == null)
            {
                return;
            }

            targetField.value = Selection.activeGameObject;
        }
        private void UpdateTarget()
        {
            target = targetField.value as GameObject;
            GetAllData();
        }
    }
}