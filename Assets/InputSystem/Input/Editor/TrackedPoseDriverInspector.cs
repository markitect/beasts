using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Experimental.Input;



namespace UnityEngine.Experimental.Input.Spatial
{
    static class Styles
    {
        public static GUIContent deviceLabel = new GUIContent("Device Type");
        public static GUIContent controlLabel = new GUIContent("Control");
    }

    /// <summary>
    /// Base tracked object class.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TrackedPoseDriver))]
    public class TrackedPoseDriverInspector : Editor, IControlDomainSource
    {

        private static readonly GUIContent s_TrackingTypeLabelContent = new GUIContent("Tracking Type", "Track both rotation and position, or individually");        
        private static readonly GUIContent s_UseRelativeTransform = new GUIContent("Use Relative Transform", "Will offset any spatial tracking from the original transform");
      
        private SerializedProperty m_TrackingTypeProp = null;        
        private SerializedProperty m_UseRelativeTransformProp = null;

		TrackedPoseDriver m_SpatiallyTrackedComponent;
        void OnEnable()
        {
            m_SpatiallyTrackedComponent = target as TrackedPoseDriver;
            
            m_TrackingTypeProp = this.serializedObject.FindProperty("m_TrackingType");                        
            m_UseRelativeTransformProp = this.serializedObject.FindProperty("m_UseRelativeTransform");
 
        }

        public override void OnInspectorGUI()
        {

            
            m_SpatiallyTrackedComponent.deviceSlot.OnGUI(
            EditorGUILayout.GetControlRect(),
            Styles.deviceLabel,
            typeof(TrackedInputDevice));

            EditorGUI.BeginChangeCheck();
            float height = ControlGUIUtility.GetControlHeight(m_SpatiallyTrackedComponent.binding, Styles.controlLabel);
            Rect position = EditorGUILayout.GetControlRect(true, height);
            ControlGUIUtility.ControlField(position, m_SpatiallyTrackedComponent.binding, Styles.controlLabel, this,
                b => m_SpatiallyTrackedComponent.binding = b as ControlReferenceBinding<PoseControl, Pose>);
                        
            EditorGUILayout.PropertyField(m_TrackingTypeProp, s_TrackingTypeLabelContent);

			TrackedPoseDriver.TrackingType trackType = (TrackedPoseDriver.TrackingType)m_TrackingTypeProp.enumValueIndex;
            
            EditorGUILayout.PropertyField(m_UseRelativeTransformProp, s_UseRelativeTransform);
            this.serializedObject.ApplyModifiedProperties();         
        }
    

        public List<DomainEntry> GetDomainEntries()
        {
            return new List<DomainEntry>() { new DomainEntry() { name = "", hash = 0 } };
        }

        public List<DomainEntry> GetControlEntriesOfType(int domainId, Type controlType)
        {
            return InputDeviceUtility.GetDeviceControlEntriesOfType(m_SpatiallyTrackedComponent.deviceSlot, typeof(PoseControl));
        }

    }
}
