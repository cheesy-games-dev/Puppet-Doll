using UnityEngine;
using UltEvents;

namespace freakycheesy.PuppetDoll
{
    public class RagdollCharacter : MonoBehaviour
    {
        public PuppetDoll puppetDoll;
        private float _ogStrength;
        public bool isRagdoll;
        public float impulseAmountToRagdoll = 20;
        public bool autoUnragdoll = true;
        public float timeToUnragdoll;
        public UltEvent RagdollEvent;
        public UltEvent UnragdollEvent;
        void Start()
        {
            puppetDoll ??= GetComponent<PuppetDoll>();
            foreach (var bone in puppetDoll.bones)
            {
                Bone newBone = bone.physicsBone.gameObject.AddComponent<Bone>();
                newBone.character = this;
                newBone.impulseAmountToRagdoll = impulseAmountToRagdoll;
            }
        }

        public void Ragdoll()
        {
            if (isRagdoll) return;
            _ogStrength = puppetDoll.maxStrength;
            puppetDoll.maxStrength = 0;
            isRagdoll = true;
            RagdollEvent.Invoke();
            if (autoUnragdoll) Invoke(nameof(Unragdoll), timeToUnragdoll);
        }
        public void Unragdoll()
        {
            if (!isRagdoll)
            {
                if (puppetDoll.maxStrength <= 0) _ogStrength = 100;
                else return;
            }
            puppetDoll.maxStrength = _ogStrength;
            _ogStrength = 0;
            isRagdoll = false;
            UnragdollEvent.Invoke();
        }
    }

    public class Bone : MonoBehaviour
    {
        public RagdollCharacter character;
        public float impulseAmountToRagdoll;
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.impulse.magnitude >= impulseAmountToRagdoll)
            {
                character.Ragdoll();
            }
        }
    }
}