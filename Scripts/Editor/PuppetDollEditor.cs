using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace freakycheesy.PuppetDoll.Editor
{
    [CustomEditor(typeof(PuppetDoll))]
    public class PuppetDollEditor : UnityEditor.Editor
    {
        bool clearBones;
        PuppetDoll doll;
        public override void OnInspectorGUI()
        {
            doll = (PuppetDoll)target;
            base.OnInspectorGUI();
            if (Application.isPlaying) return;
            if (GUILayout.Button("Find Bones")) FindBones();
            clearBones = GUILayout.Toggle(clearBones, "Clear Bones");
            if (clearBones) if (GUILayout.Button("Clear Bones")) ClearBones();
        }

        private void ClearBones()
        {
            doll.bones.Clear();
            Debug.Log("Cleared bones.");
        }


        private void FindBones()
        {
            if(clearBones) ClearBones();
            var bones = new List<RagdollBone>();
            var rigidbodies = doll.physicalHip.GetComponentsInChildren<Rigidbody>(true);
            foreach (var rb in rigidbodies)
            {
                bones.Add(new(rb));
            }

            var virtuals = doll.virtualHip.GetComponentsInChildren<Transform>(true);
            foreach (var virtua in virtuals)
            {
                for (int i = 0; i < bones.Count; i++)
                {
                    if (bones[i].physicsBone.name.Contains(virtua.name)) bones[i] = new(bones[i].physicsBone, virtua);
                }
            }

            doll.bones.AddRange(bones);
            AssetDatabase.SaveAssetIfDirty(doll);
            EditorUtility.SetDirty(doll);
            Debug.Log($"Found {bones.Count} bones.");
        }
    }
}
