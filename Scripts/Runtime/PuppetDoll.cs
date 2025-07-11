using UnityEngine;

namespace freakycheesy.PuppetDoll
{
    [AddComponentMenu("PuppetDoll/Puppet Doll")]
    public class PuppetDoll : MonoBehaviour
    {
        public RagdollBonesSettings Settings = new();

        private void OnValidate()
        {
            Settings.ValidateBones(this);
        }
    }
}
