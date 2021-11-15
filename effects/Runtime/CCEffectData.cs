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
        public float idBlend;
        
        public int group;
        public float groupBlend;

        public int groupID;
        public float groupIDBlend;

        public int unit;
        public float unitBlend;
        public int unitID;
        public float unitIDBlend;
        
        public float dist;

        public float random => Random(0);

        public float x;
        public float y;
        public float z;

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
