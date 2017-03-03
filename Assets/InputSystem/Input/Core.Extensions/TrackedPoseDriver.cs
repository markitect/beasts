// comment out to use the old VR Node system
#define USE_NEW_INPUT_SYSTEM

using System;
using UnityEngine;
using UnityEngineInternal.Input;
using Assets.Utilities;
using System.Linq;

namespace UnityEngine.Experimental.Input.Spatial
{
	/// <summary>
	/// Base tracked object class.
	/// </summary>
	/// 
	[Serializable]
	[AddComponentMenu("Core.Extensions/TrackedPoseDriver")]
    public class TrackedPoseDriver : MonoBehaviour, ISerializationCallbackReceiver
    {
        // reserved for future extensibility
        public enum TrackingSource
        {
            Device,
            Action,
        }
        TrackingSource m_TrackingSource = TrackingSource.Device;
      
        [SerializeField]
        DeviceSlot m_DeviceSlot;
        public DeviceSlot deviceSlot
        {
            get { return m_DeviceSlot; }
            set { m_DeviceSlot = value; }
        }

        [NonSerialized]
        public ControlReferenceBinding<PoseControl, Pose> m_Binding = new ControlReferenceBinding<PoseControl, Pose>();
        public ControlReferenceBinding<PoseControl, Pose> binding
        {
            get { return m_Binding; }
            set { m_Binding = value;  m_BindingDirty = true; }
        }

        bool m_BindingDirty = false;

        [SerializeField]
        SerializationHelper.JSONSerializedElement m_SerializedBinding;        

        public enum TrackingType
        {
            Full,
            RotationOnly,
            PositionOnly          
        }

        [SerializeField]
        TrackingType m_TrackingType;
        public TrackingType trackingType
        {
            get { return m_TrackingType; }
            set { m_TrackingType = value; }
        }

        [SerializeField]
        bool m_UseRelativeTransform;
        public bool UseRelativeTransform
        {
            get { return m_UseRelativeTransform; }
            set { m_UseRelativeTransform = value; }
        }

        Vector3 m_OriginTransform;
        Quaternion m_OriginRotation;
        
        protected virtual void CacheLocalPosition()
        {
            m_OriginTransform = transform.localPosition;
            m_OriginRotation = transform.localRotation;
        }
        protected virtual void ResetToCachedLocalPosition()
        {
            SetLocalTransform(m_OriginTransform, m_OriginRotation);
        }

        protected virtual void Awake()
        {
            CacheLocalPosition();

            if (this.enabled)
                AquireDevice();

            Application.onBeforeRender += OnBeforeRender;

            // camera check code
            var parentCamera = GetComponent<Camera>();
            if(parentCamera != null)
            {
                parentCamera.vrIgnoreImplicitCameraUpdate = true;
            }
            
        }

        protected virtual void OnDestroy()
        {
            Application.onBeforeRender -= OnBeforeRender;
            var parentCamera = GetComponent<Camera>();
            if (parentCamera != null)
            {
                parentCamera.vrIgnoreImplicitCameraUpdate = false;
            }
        }
        
        protected virtual void OnEnable()
        {
            // register for delegate for late update           
            CacheLocalPosition();            
        }

        protected virtual void AquireDevice()
        {  
            // if we're using the device grab the first one that matches the tag (need to fix this)
            if (m_TrackingSource == TrackingSource.Device && 
                m_DeviceSlot != null && 
                m_DeviceSlot.type.value != null &&
                m_Binding != null)
            {
                var device = InputSystem.devices.Where(
                    x => m_DeviceSlot.type.value.IsAssignableFrom(x.GetType()) &&
                    x.tagIndex == m_DeviceSlot.tagIndex
                    ).FirstOrDefault();
                if (device != null)
                {
                    m_Binding.Initialize(device);
                    m_BindingDirty = false;
                }
            }
            
            // FIXME - error
        }

        protected virtual void OnDisable()
        {
            // remove delegate registration            
            ResetToCachedLocalPosition();           
        }

        protected virtual void FixedUpdate()
        {            
            DoEarlyUpdate();
        }

        protected virtual void DoEarlyUpdate()
        {
            PerformUpdate();            
        }

        protected virtual void OnBeforeRender()
        {
            DoLateUpdate();
        }

        protected virtual void DoLateUpdate()
        {
            PerformUpdate();
        }


        protected virtual bool ResolveValues(ref Vector3 pos, ref Quaternion quat)
        {

            if (m_BindingDirty == true)
            {
                AquireDevice();               
            }
            
            if(m_BindingDirty == false && m_TrackingSource == TrackingSource.Device && m_Binding.sourceControl != null)
            {
                
                m_Binding.EndUpdate();

                Pose tmp = m_Binding.value;
                pos = tmp.translation;
                quat = tmp.rotation;				
                return true;
            }
           
            return false;
        }

        protected virtual void SetLocalTransform(Vector3 newPosition, Quaternion newRotation)
        {
            if (m_TrackingType == TrackingType.Full ||
                m_TrackingType == TrackingType.RotationOnly)
            {            
                transform.localRotation = newRotation;              
            }
        
            if (m_TrackingType == TrackingType.Full ||
                m_TrackingType == TrackingType.PositionOnly)
            {
                transform.localPosition = newPosition;                
            }
        }


        protected virtual void PerformUpdate()
        {
            if (!this.enabled)
                return;
            
            Vector3 newPosition = Vector3.zero;
            Quaternion newRotation = Quaternion.identity;
            if (ResolveValues(ref newPosition, ref newRotation))
            {             
                if (m_UseRelativeTransform)
                {
                    newPosition = m_OriginTransform + (m_OriginRotation * newPosition);
                    newRotation = m_OriginRotation * newRotation;
                }

                SetLocalTransform(newPosition, newRotation);
            }
        }

        public virtual void OnBeforeSerialize()
        {
            m_SerializedBinding = SerializationHelper.SerializeObj(binding);
        }

        public virtual void OnAfterDeserialize()
        {
            binding = SerializationHelper.DeserializeObj<ControlReferenceBinding<PoseControl, Pose>>(m_SerializedBinding, new object[] { });
            m_SerializedBinding = new SerializationHelper.JSONSerializedElement();
        }
    }
}