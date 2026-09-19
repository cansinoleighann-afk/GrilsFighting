using UnityEditor;
using UnityEngine;
using System.Reflection;

[InitializeOnLoad]
public static class AnimationPreviewAutoPlay
{
    private static Object lastSelection;

    static AnimationPreviewAutoPlay()
    {
        Selection.selectionChanged += OnSelectionChanged;
    }

    private static void OnSelectionChanged()
    {
        Object selected = Selection.activeObject;

        if (selected == lastSelection)
            return;

        lastSelection = selected;

        if (selected is AnimationClip)
        {
            EditorApplication.delayCall += TryAutoPlayPreview;
        }
    }

    private static void TryAutoPlayPreview()
    {
        var inspectorType = typeof(Editor).Assembly.GetType(
            "UnityEditor.InspectorWindow"
        );

        if (inspectorType == null)
            return;

        Object[] inspectors =
            Resources.FindObjectsOfTypeAll(inspectorType);

        foreach (Object inspector in inspectors)
        {
            var trackerProperty = inspectorType.GetProperty(
                "tracker",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

            if (trackerProperty == null)
                continue;

            var tracker = trackerProperty.GetValue(inspector) as ActiveEditorTracker;

            if (tracker == null)
                continue;

            foreach (Editor editor in tracker.activeEditors)
            {
                if (editor == null)
                    continue;

                if (!(editor.target is AnimationClip))
                    continue;

                TryPlayEditorPreview(editor);
            }
        }
    }

    private static void TryPlayEditorPreview(Editor editor)
    {
        var editorType = editor.GetType();

        FieldInfo previewField =
            editorType.GetField(
                "m_AvatarPreview",
                BindingFlags.Instance |
                BindingFlags.NonPublic
            );

        if (previewField == null)
            return;

        object avatarPreview = previewField.GetValue(editor);

        if (avatarPreview == null)
            return;

        var previewType = avatarPreview.GetType();

        PropertyInfo playingProperty =
            previewType.GetProperty(
                "playing",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        if (playingProperty != null &&
            playingProperty.CanWrite)
        {
            playingProperty.SetValue(avatarPreview, true);
            return;
        }

        MethodInfo playMethod =
            previewType.GetMethod(
                "Play",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        if (playMethod != null)
        {
            playMethod.Invoke(avatarPreview, null);
        }
    }
}