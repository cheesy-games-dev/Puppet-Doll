using System;
using System.Collections.Generic;
using UnityEngine;

namespace freakycheesy.PuppetDoll {
    [AddComponentMenu("PuppetDoll/Puppet Doll")]
    public class PuppetDoll : MonoBehaviour {
        [Header("Bones")]
        public List<RagdollBone> bones;
        public Rigidbody physicalHip;
        public Transform virtualHip;
        public bool virtuaHipFollowPhysicalHip = true;
        public bool virtuaHipRotatePhysicalHip = false;
        [Header("PID and Physics")]
        public ForceMode forceMode = ForceMode.Force;
        public float inertiaTensorResitance = 0.2f;

        public float proportionalGain = 10f;
        public float derivativeGain = 1f;
        public float maxStrength = 100f;


        // Following sparksmints ragdoll doc
        private void FixedUpdate() {
            if (virtuaHipFollowPhysicalHip)
                virtualHip.position = physicalHip.position;
            if (virtuaHipRotatePhysicalHip)
                virtualHip.rotation = physicalHip.rotation;
            foreach (RagdollBone bone in bones) {
                bone.physicsBone.inertiaTensor = Vector3.one * inertiaTensorResitance;

                Quaternion deltaRotation = bone.virtualBone.rotation *
                  Quaternion.Inverse(bone.physicsBone.rotation);

                if (deltaRotation.w < 0)
                    deltaRotation = Quaternion.Inverse(deltaRotation);

                deltaRotation.ToAngleAxis(out var angle, out var axis);
                axis.Normalize();
                angle *= Mathf.Deg2Rad;
                Vector3 angularVelocityError = angle / Time.fixedDeltaTime * axis;

                Vector3 torque = proportionalGain * angularVelocityError - derivativeGain *
                  bone.physicsBone.angularVelocity;

                torque = Vector3.ClampMagnitude(torque, maxStrength);

                bone.physicsBone.AddTorque(torque, forceMode);
            }
        }
    }

    [Serializable]
    public struct RagdollBone {
        public Rigidbody physicsBone;
        public Transform virtualBone;
        public RagdollBone(Rigidbody physicsBone = null, Transform virtualBone = null) {
            this.physicsBone = physicsBone;
            this.virtualBone = virtualBone;
        }
        public RagdollBone(Transform virtualBone = null, Rigidbody physicsBone = null) {
            this.virtualBone = virtualBone;
            this.physicsBone = physicsBone;
        }

        public void SetPhysicsBone(Rigidbody newBone) => physicsBone = newBone;
        public void SetVirtualBone(Transform newBone) => virtualBone = newBone;
    }
}