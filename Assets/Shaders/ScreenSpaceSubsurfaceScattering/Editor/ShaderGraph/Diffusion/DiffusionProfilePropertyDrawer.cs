using System;
using System.Collections.Generic;
using UnityEditor.Graphing;
using System.Reflection;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine.Rendering.Universal;
using UnityEditor.ShaderGraph.Drawing.Inspector;

namespace UnityEditor.Rendering.Universal//.ShaderGraph
{
    [SGPropertyDrawer(typeof(SubsurfaceScatteringProfile))]
    class DiffusionProfilePropertyDrawer : IPropertyDrawer
    {
        internal delegate void ValueChangedCallback(SubsurfaceScatteringProfile newValue);

        internal VisualElement CreateGUI(
            ValueChangedCallback valueChangedCallback,
            SubsurfaceScatteringProfile fieldToDraw,
            string labelName,
            out VisualElement propertyColorField,
            int indentLevel = 0)
        {
            var objectField = new ObjectField { value = fieldToDraw, objectType = typeof(SubsurfaceScatteringProfile) };

            if (valueChangedCallback != null)
            {
                objectField.RegisterValueChangedCallback(evt => { valueChangedCallback((SubsurfaceScatteringProfile) evt.newValue); });
            }

            propertyColorField = objectField;

            var defaultRow = new PropertyRow(PropertyDrawerUtils.CreateLabel(labelName, indentLevel));
            defaultRow.Add(propertyColorField);
            defaultRow.styleSheets.Add(Resources.Load<StyleSheet>("Styles/PropertyRow"));
            return defaultRow;
        }

        public Action inspectorUpdateDelegate { get; set; }

        public VisualElement DrawProperty(PropertyInfo propertyInfo, object actualObject, InspectableAttribute attribute)
        {
            return this.CreateGUI(
                // Use the setter from the provided property as the callback
                newValue => propertyInfo.GetSetMethod(true).Invoke(actualObject, new object[] {newValue}),
                (SubsurfaceScatteringProfile) propertyInfo.GetValue(actualObject),
                attribute.labelName,
                out var propertyVisualElement);
        }
    }
}
