using System;
using scm = System.ComponentModel;
using uei = UnityEngine.Internal;

namespace UnityEngine
{
    public struct StereoPose 
    {
        public Pose centerEye;
        public Pose leftEye;
        public Pose rightEye;

        public StereoPose(Pose left, Pose right, Pose center)
        {
            leftEye = left;
            rightEye = right;
            centerEye = center;
        }
        
        public override int GetHashCode()
        {
            return centerEye.GetHashCode() ^ leftEye.GetHashCode() ^ rightEye.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is StereoPose)) return false;

            StereoPose rhs = (StereoPose)other;
            return rhs.centerEye.Equals(centerEye) && rhs.leftEye.Equals(leftEye) && rhs.rightEye.Equals(rightEye) ;
        }

        public override string ToString()
        {
            return centerEye.ToString() + leftEye.ToString() + rightEye.ToString();
        }

        public string ToString(string format)
        {
            return centerEye.ToString(format) + leftEye.ToString(format) + rightEye.ToString(format);
        }
    }
}