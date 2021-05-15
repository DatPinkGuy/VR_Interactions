using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit;


[CustomEditor(typeof(Weapon.HandHold), true), CanEditMultipleObjects]
public class HandHoldEditor : XRBaseInteractableEditor
{
    private SerializedProperty m_AdditionalField;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_AdditionalField = serializedObject.FindProperty("handsVisible");
    }

    protected override void DrawProperties()
    {
        base.DrawProperties();
        EditorGUILayout.LabelField("Hand Hold parameters", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_AdditionalField);
        EditorGUILayout.LabelField("End of Hand Hold parameters", EditorStyles.boldLabel);
    }
}
