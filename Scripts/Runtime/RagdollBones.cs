using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace freakycheesy.PuppetDoll
{
    [AddComponentMenu("PuppetDoll/RagdollBone"), RequireComponent(typeof(Rigidbody))]
    public class RagdollBone : MonoBehaviour
    {
        public PuppetDoll puppetDoll;
        public float PinWeight => puppetDoll.Settings.PinWeight;
        public Transform virtualBone;
        [HideInInspector] public Rigidbody PhysicalBone() => GetComponent<Rigidbody>();

        void FixedUpdate()
        {
            if (!PhysicalBone() || puppetDoll.Settings.PinWeight <= 0)
            {
                return;
            }

            Vector3 distance = virtualBone.transform.position - PhysicalBone().position;
            PhysicalBone().linearVelocity = distance * PinWeight;
            Vector3 angularDistance = virtualBone.transform.eulerAngles - transform.eulerAngles;
            PhysicalBone().angularVelocity = angularDistance * PinWeight;
        }

    }

    [Serializable]
    public class RagdollBonesSettings
    {
        [Range(0, 1)]
        public float PinWeight = 1;
        public List<RagdollBone> RagdollBones;
        public Transform virtualArmature;
        public Transform physicalArmature;

        public void ValidateBones(PuppetDoll doll)
        {
            RagdollBones = doll.GetComponentsInChildren<RagdollBone>().ToList();
            foreach (var bone in RagdollBones)
            {
                bone.puppetDoll = doll;
                if (bone.PhysicalBone().transform == bone.virtualBone)
                {
                    bone.virtualBone = null;
                    Debug.LogError("Can not use same gameObject for physical and virutal Bone", bone.PhysicalBone());
                }
            }
        }
    }

    
}
