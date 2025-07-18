using System;
using System.Collections.Generic;
using UnityEngine;

namespace freakycheesy.PuppetDoll {
    [AddComponentMenu("PuppetDoll/Puppet Doll")]
    public class PuppetDoll : MonoBehaviour {
        public List<RagdollBone> bones;

        public float inertiaTensorResitance = 0.2f;

        public float proportionalGain = 1f;
        public float derivativeGain = 0.1f;
        public float maxStrength = 100f;
        
        // Following sparksmints ragdoll doc
        private void FixedUpdate() {
            foreach (RagdollBone bone in bones) {
                Torque(bone);
            }
        }

        private void Torque(RagdollBone bone) {
            Quaternion deltaRotation = GetRotationDelta(bone);

            // Basically Absolute Value equivalent for the Quaternion.
            if (deltaRotation.w < 0)
                deltaRotation = Quaternion.Inverse(deltaRotation);

            deltaRotation.ToAngleAxis(out var angle, out var axis);

            axis.Normalize();

            angle *= Mathf.Deg2Rad;

            Vector3 angularVelocityError = angle / Time.fixedDeltaTime * axis;

            Vector3 torque = proportionalGain * angularVelocityError - derivativeGain *
              bone.physicsBone.angularVelocity;

            torque = Vector3.ClampMagnitude(torque, maxStrength);

            bone.physicsBone.AddTorque(torque, ForceMode.VelocityChange);
        }

        private static Quaternion GetRotationDelta(RagdollBone bone) {
            // Gets the difference between the physics bone rot and the virtual bone rot.
            // This is the Quaternion of getting a linear delta.
            return bone.virtualBone.rotation *
              Quaternion.Inverse(bone.physicsBone.rotation);
        }
    }

    [Serializable]
    public struct RagdollBone {
        public Rigidbody physicsBone;
        public Transform virtualBone;

        public void SetPhysicsBone(Rigidbody newBone) => physicsBone = newBone;
        public void SetVirtualBone(Transform newBone) => virtualBone = newBone;
    }
}
