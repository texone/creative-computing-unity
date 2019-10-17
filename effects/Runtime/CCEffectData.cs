using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace cc.creativecomputing.effects
{
    [AddComponentMenu("effects/data")]
    public class CCEffectData : MonoBehaviour
    {
        public float amount = 1;
        public int id;

        public int group;
        public float groupBlend;

        public int groupID;
        public float groupIDBlend;

        public float idBlend;

        public float distBlend;

        public float random => Random(0);

        public float xBlend;
        public float yBlend;
        public float zBlend;

        public float angle = 0;

        private Dictionary<uint, float> randoms = new Dictionary<uint, float>();

        public float Random(uint seed)
        {
            if (!randoms.ContainsKey(seed))
            {
                randoms.Add(seed, UnityEngine.Random.Range(-1f, 1f));
            }

            return randoms[seed];
        }
    }
}
