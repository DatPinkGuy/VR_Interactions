using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit;
using UnityEngine;


[CustomEditor(typeof(Weapon.Weapon), true), CanEditMultipleObjects]
public class WeaponEditor : XRGrabInteractableEditor
{
    private SerializedProperty m_AdditionalField;
    private SerializedProperty m_AdditionalField2;
    private SerializedProperty m_AdditionalField3;
    private SerializedProperty m_AdditionalField4;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_AdditionalField = serializedObject.FindProperty("breakDistance");
        m_AdditionalField2 = serializedObject.FindProperty("recoilAmount");
        m_AdditionalField3 = serializedObject.FindProperty("oneHanded");
        m_AdditionalField4 = serializedObject.FindProperty("isMachineGun");
    }

    protected override void DrawProperties()
    {
        base.DrawProperties();
        EditorGUILayout.LabelField("Weapon Parameters", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_AdditionalField);
        EditorGUILayout.PropertyField(m_AdditionalField2);
        EditorGUILayout.PropertyField(m_AdditionalField3);
        EditorGUILayout.PropertyField(m_AdditionalField4);
    }
}
