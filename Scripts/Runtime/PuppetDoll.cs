using System;
using System.Collections.Generic;
using UnityEngine;

namespace freakycheesy.PuppetDoll
{
    [AddComponentMenu("PuppetDoll/Puppet Doll")]
    public class PuppetDoll : MonoBehaviour
    {
        public List<RagdollBone> bones;
        public Rigidbody physicalHip;
        public Transform virtualHip;
        public bool virtuaHipFollowPhysicalHip = true;
        public bool virtuaHipRotatePhysicalHip = false;
        public ForceMode forceMode = ForceMode.Force;
        public PIDSettings pidSettings = new(0.2f);


        // Following sparksmints ragdoll doc
        private void FixedUpdate()
        {
            if (virtuaHipFollowPhysicalHip) virtualHip.position = physicalHip.position;
            if (virtuaHipRotatePhysicalHip) virtualHip.rotation = physicalHip.rotation;
            foreach (RagdollBone bone in bones)
            {
                Torque(bone);
            }
        }
        public void Torque(RagdollBone bone)
        {
            bone.physicsBone.inertiaTensor = Vector3.one * pidSettings.inertiaTensorResitance;
            Quaternion deltaRotation = GetRotationDelta(bone);

            // Basically Absolute Value equivalent for the Quaternion.
            if (deltaRotation.w < 0)
                deltaRotation = Quaternion.Inverse(deltaRotation);

            deltaRotation.ToAngleAxis(out var angle, out var axis);

            axis.Normalize();

            angle *= Mathf.Deg2Rad;

            Vector3 angularVelocityError = angle / Time.fixedDeltaTime * axis;

            Vector3 torque = pidSettings.proportionalGain * angularVelocityError - pidSettings.derivativeGain *
              bone.physicsBone.angularVelocity;

            torque = Vector3.ClampMagnitude(torque, pidSettings.maxStrength);

            bone.physicsBone.AddTorque(torque, ForceMode.Force);
        }
        public static Quaternion GetRotationDelta(RagdollBone bone)
        {
            // Gets the difference between the physics bone rot and the virtual bone rot.
            // This is the Quaternion of getting a linear delta.
            return bone.virtualBone.rotation *
              Quaternion.Inverse(bone.physicsBone.transform.rotation);
        }
    }

    [Serializable]
    public struct PIDSettings
    {
        public float inertiaTensorResitance;

        public float proportionalGain;
        public float derivativeGain;
        public float maxStrength;

        public PIDSettings(float inertiaTensorResitance, float proportionalGain, float derivativeGain, float maxStrength)
        {
            this.inertiaTensorResitance = inertiaTensorResitance;
            this.proportionalGain = proportionalGain;
            this.derivativeGain = derivativeGain;
            this.maxStrength = maxStrength;
        }
        public PIDSettings(float inertiaTensorResitance = 0.2f)
        {
            this.inertiaTensorResitance = inertiaTensorResitance;
            this.proportionalGain = 1f;
            this.derivativeGain = 0.1f;
            this.maxStrength = 100f;
        }
    }

    [Serializable]
    public struct RagdollBone
    {
        public Rigidbody physicsBone;
        public Transform virtualBone;
        public RagdollBone(Rigidbody physicsBone = null, Transform virtualBone = null)
        {
            this.physicsBone = physicsBone;
            this.virtualBone = virtualBone;
        }
        public RagdollBone(Transform virtualBone = null, Rigidbody physicsBone = null)
        {
            this.virtualBone = virtualBone;
            this.physicsBone = physicsBone;
        }

        public void SetPhysicsBone(Rigidbody newBone) => physicsBone = newBone;
        public void SetVirtualBone(Transform newBone) => virtualBone = newBone;
    }
}