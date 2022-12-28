using PFCToolkit.Base;
using PFCToolkit.DrawingTools;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PFCToolkit.AvatarTools {
    public class HiearchyVisualizerEditorWindow : EditorWindow {
        public GameObject gameobject;
        private Button visualizeButton;
        private readonly HierarchyVisualizer visualizer = new HierarchyVisualizer();

        private void OnEnable() {
            SceneView.duringSceneGui += visualizer.DrawHandles;

            SerializedObject SO = new SerializedObject(this);

            VisualElement root = this.rootVisualElement;
            ObjectField avatarField = new ObjectField("Target");
            avatarField.objectType = typeof(GameObject);
            avatarField.bindingPath = nameof(gameobject);
            avatarField.Bind(SO);
            avatarField.RegisterValueChangedCallback(evt => CheckFields());
            root.Add(avatarField);

            visualizeButton = new Button(() => visualizer.VisualizeHierarchy(gameobject));
            visualizeButton.text = "Visualize Skeleton";
            root.Add(visualizeButton);

            Button ClearButton = new Button(() => visualizer.ClearVisualization());
            ClearButton.text = "Clear";
            root.Add(ClearButton);

            Foldout settingsFoldout = new Foldout();
            settingsFoldout.text = "Settings";
            root.Add(settingsFoldout);

            Toggle ToggleCapMaxSize = new Toggle(PFCDrawTools.CapMaxSize.name);
            ToggleCapMaxSize.value = PFCDrawTools.CapMaxSize.IsEnabled;
            ToggleCapMaxSize.RegisterValueChangedCallback(evt => {
                PFCDrawTools.CapMaxSize.IsEnabled = evt.newValue;
                SceneView.RepaintAll();
            });
            settingsFoldout.Add(ToggleCapMaxSize);

            Slider BoneThicknessSlider = new Slider(PFCDrawTools.BoneThickness.name, 1, 0);
            BoneThicknessSlider.value = Mathf.Log(PFCDrawTools.BoneThickness.Value, 0.01f);
            BoneThicknessSlider.RegisterValueChangedCallback(evt => {
                PFCDrawTools.BoneThickness.Value = Mathf.Pow(0.01f, evt.newValue);
                SceneView.RepaintAll();
            });
            settingsFoldout.Add(BoneThicknessSlider);

            Toggle ToggleSlowMode = new Toggle(HierarchyVisualizer.SlowMode.name);
            ToggleSlowMode.value = HierarchyVisualizer.SlowMode.IsEnabled;
            ToggleSlowMode.RegisterValueChangedCallback(evt => {
                HierarchyVisualizer.SlowMode.IsEnabled = evt.newValue;
                SceneView.RepaintAll();
            });
            settingsFoldout.Add(ToggleSlowMode);

            CheckFields();
        }
        private void OnDisable() {
            rootVisualElement.Clear();
            SceneView.duringSceneGui -= visualizer.DrawHandles;

        }

        private void CheckFields() {
            visualizeButton.SetEnabled(gameobject != null);
        }

        [MenuItem("PFCToolkit/AvatarTools/Visualizer")]
        public static void ShowWindow() {
            HiearchyVisualizerEditorWindow window = GetWindow<HiearchyVisualizerEditorWindow>("Hierarchy Visualzer");
            window.Show();
        }
    }
}