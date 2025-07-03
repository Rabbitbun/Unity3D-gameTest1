using UnityEditor;
using UnityEngine;
using TheKiwiCoder;
using System.Linq;
using System.Reflection;

[CustomPropertyDrawer(typeof(Blackboard))]
public class BlackboardPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //EditorGUI.BeginProperty(position, label, property);

        //SerializedProperty iterator = property.Copy();
        //iterator.Next(true); // 進入第一個子屬性

        //Rect rect = position;
        //rect.height = EditorGUIUtility.singleLineHeight;

        //while (iterator.NextVisible(false))
        //{
        //    EditorGUI.PropertyField(rect, iterator, true);
        //    rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        //}

        //EditorGUI.EndProperty();


        //EditorGUI.BeginProperty(position, label, property);

        //// 獲取 Blackboard 引用
        //BehaviourTree behaviourTree = (BehaviourTree)property.serializedObject.targetObject;

        //if (behaviourTree != null && behaviourTree.blackboard != null)
        //{
        //    string[] propertyNames = behaviourTree.blackboard.GetPropertyNames();
        //    int selectedIndex = System.Array.IndexOf(propertyNames, property.FindPropertyRelative("propertyName").stringValue);
        //    selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, propertyNames);
        //    property.FindPropertyRelative("propertyName").stringValue = propertyNames[selectedIndex];

        //    // 獲取屬性類型
        //    System.Type propertyType = behaviourTree.blackboard.GetPropertyType(propertyNames[selectedIndex]);
        //    property.FindPropertyRelative("propertyType").managedReferenceValue = propertyType;
        //}
        //else
        //{
        //    EditorGUI.PropertyField(position, property, label);
        //}

        //EditorGUI.EndProperty();
    }
}
