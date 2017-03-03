using System;
using scm = System.ComponentModel;
using uei = UnityEngine.Internal;

namespace UnityEngine
{
    public struct Pose
    {
        public Vector3 translation;
        public Quaternion rotation;        

        public Pose(Vector3 tr, Quaternion rt) { translation = tr;  rotation = rt; }
        
        public override int GetHashCode()
        {
            return translation.GetHashCode() ^ rotation.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is Pose)) return false;

            Pose rhs = (Pose)other;
            return rhs.translation == translation && rhs.rotation == rotation;
        }

        public override string ToString()
        {            
			return String.Format("({0}, {1})", translation.ToString(), rotation.ToString());
		}

        public string ToString(string format)
        {
			return String.Format("({0}, {1})", translation.ToString(format), rotation.ToString(format));
		}
    }
}