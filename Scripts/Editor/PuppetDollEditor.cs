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
        public override void OnInspectorGUI()
        {
            PuppetDoll doll = (PuppetDoll)target;
            base.OnInspectorGUI();
            if (GUILayout.Button("Try Match Bones")) TryMatchBones(doll);
            clearBones = GUILayout.Toggle(clearBones, "Clear Bones");
            if (clearBones) if (GUILayout.Button("Clear Bones")) doll.bones.Clear();
        }

        private void TryMatchBones(PuppetDoll doll)
        {
            foreach (var bone in doll.bones)
            {
                bone.SetVirtualBone(GameObject.Find(bone.physicsBone.name).transform);
            }
        }
    }
}
