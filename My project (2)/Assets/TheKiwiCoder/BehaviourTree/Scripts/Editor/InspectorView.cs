using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using System.Linq;
using UnityEngine;
using System.Reflection;

namespace TheKiwiCoder {
    public class InspectorView : VisualElement {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        Editor editor;

        private DropdownField blackboardPropertyDropdown;
        private DropdownField enumValueDropdown;
        private Blackboard blackboard;
        private ConditionNode currentConditionNode;
        private Dictionary<string, Type> blackboardPropertyTypes = new Dictionary<string, Type>();

        //private EnumConditionEditor enumConditionEditor;

        public InspectorView() {

        }
        private void Initialize()
        {
            // 初始化 blackboard Property Dropdown
            blackboardPropertyDropdown = new DropdownField("Blackboard Property");
            blackboardPropertyDropdown.RegisterValueChangedCallback(OnBlackboardPropertyChanged);
            blackboardPropertyDropdown.style.minWidth = 200;
            blackboardPropertyDropdown.style.minHeight = 20;
            blackboardPropertyDropdown.style.display = DisplayStyle.None;
            Add(blackboardPropertyDropdown);

            enumValueDropdown = new DropdownField("enumValue");
            enumValueDropdown.RegisterValueChangedCallback(OnEnumValueChanged);
            enumValueDropdown.style.minWidth = 200;
            enumValueDropdown.style.minHeight = 20;
            enumValueDropdown.style.display = DisplayStyle.None;
            Add(enumValueDropdown);
        }

        internal void UpdateSelection(NodeView nodeView) {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);

            currentConditionNode = nodeView.node as ConditionNode;

            editor = Editor.CreateEditor(nodeView.node);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor && editor.target)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);

            // Ensure the DropdownField is re-added after clearing
            if (nodeView.node is ConditionNode)
            {
                InitializeBlackboardPropertyDropdown(nodeView.node.blackboard);

                blackboardPropertyDropdown.style.display = DisplayStyle.Flex;
                // Debug.Log(currentConditionNode.selectedBlackboardProperty);
                // Restore the previously selected property
                if (!string.IsNullOrEmpty(currentConditionNode.selectedBlackboardProperty))
                {
                    blackboardPropertyDropdown.value = currentConditionNode.selectedBlackboardProperty;
                }
                else if (blackboardPropertyDropdown.choices.Count > 0)
                {
                    blackboardPropertyDropdown.value = blackboardPropertyDropdown.choices[0];
                }
                if (!string.IsNullOrEmpty(currentConditionNode.selectedEnumValue))
                {
                    enumValueDropdown.value = currentConditionNode.selectedEnumValue;
                }
                else if (enumValueDropdown.choices.Count > 0)
                {
                    enumValueDropdown.value = enumValueDropdown.choices[0];
                }
            }
            //else
            //{
            //    blackboardPropertyDropdown.style.display = DisplayStyle.None;
            //}

        }

        //private void InitializeBlackboardPropertyDropdown(Blackboard blackboard)
        //{
        //    Initialize();

        //    List<string> propertyNames = new List<string>();
        //    if (blackboard != null)
        //    {
        //        FieldInfo[] fields = blackboard.GetType().GetFields();
        //        foreach (var field in fields)
        //        {
        //            propertyNames.Add(field.Name);
        //        }
        //        PropertyInfo[] properties = blackboard.GetType().GetProperties();
        //        foreach (var property in properties)
        //        {
        //            propertyNames.Add(property.Name);
        //        }
        //    }
        //    blackboardPropertyDropdown.choices = propertyNames;
        //}

        private void InitializeBlackboardPropertyDropdown(Blackboard blackboard)
        {
            Initialize();

            this.blackboard = blackboard;
            List<string> propertyNames = new List<string>();

            if (blackboard != null)
            {
                propertyNames = blackboard.GetAllNames().ToList();
                // 得到型別
                foreach (var propertyName in propertyNames)
                {
                    Type dataType = blackboard.GetDataType(propertyName);
                    Type enumType = GetEnumTypeFromData(dataType);
                    if (enumType == null) continue;
                    blackboardPropertyTypes[propertyName] = enumType;
                }
            }

            blackboardPropertyDropdown.choices = propertyNames;
        }

        private void OnBlackboardPropertyChanged(ChangeEvent<string> evt)
        {
            string selectedProperty = evt.newValue;
            if (currentConditionNode != null)
            {
                currentConditionNode.selectedBlackboardProperty = selectedProperty;
            }

            // Get the data type of the selected property
            if (blackboardPropertyTypes.TryGetValue(selectedProperty, out Type propertyType))
            {
                if (propertyType.IsEnum)
                {
                    // Show the Enum value dropdown
                    enumValueDropdown.style.display = DisplayStyle.Flex;

                    // Populate the Enum value dropdown with the values of the selected Enum
                    string[] enumValues = Enum.GetNames(propertyType);
                    enumValueDropdown.choices = enumValues.ToList();
                }
                else
                {
                    // Hide the Enum value dropdown if the property is not an Enum
                    enumValueDropdown.style.display = DisplayStyle.None;
                }
            }
            else
            {
                //Debug.LogError($"Property type not found for property: {selectedProperty}");
            }

            // Do something with the selected Blackboard property
            Debug.Log($"Selected Blackboard property: {selectedProperty}");
        }
        private void OnEnumValueChanged(ChangeEvent<string> evt)
        {
            string selectedEnumValue = evt.newValue;
            if (currentConditionNode != null)
            {
                currentConditionNode.selectedEnumValue = selectedEnumValue;
            }
            // Do something with the selected Blackboard property
            Debug.Log($"Selected Blackboard property: {selectedEnumValue}");
        }

        private Type GetEnumTypeFromData(Type dataType)
        {
            if (dataType == typeof(AIStateBlackboardData))
            {
                return typeof(AIState);
            }
            else if (dataType == typeof(AICombatStateBlackboardData))
            {
                return typeof(AICombatState);
            }
            else
            {
                //Debug.LogError($"不是有效的枚举数据类型: {dataType}");
                return null;
            }
        }
    }
}