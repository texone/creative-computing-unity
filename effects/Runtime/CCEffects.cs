using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using cc.creativecomputing.math.interpolate;
using cc.creativecomputing.math.util;

namespace cc.creativecomputing.effects
{
    [ExecuteAlways]
    [AddComponentMenu("effects/effect controller")]
    public class CCEffects : MonoBehaviour
    {
        public Bounds bounds;

        public GameObject effectParent;

        public List<GameObject> effectNodes = new List<GameObject>();
        public List<CCEffectData> effectDatas = new List<CCEffectData>();
        List<CCEffect> effects = new List<CCEffect>();

        private CCFilter[] filters;

        public float amount = 35;

        private void InitBounds(GameObject theEffectedNode)
        {
            if (bounds.center.Equals(new Vector3()))
            {
                bounds = new Bounds(theEffectedNode.transform.parent.localPosition, new Vector3());
            }
            bounds.Encapsulate(theEffectedNode.transform.parent.localPosition);
        }

        public int maxID = 0;

        private void InitEffectData(List<GameObject> theEffectNodes)
        {
            int i = 0;
            theEffectNodes.ForEach(go =>
            {
                Transform t = go.transform;

                var myData = t.gameObject.GetComponent<CCEffectData>();
                myData.xBlend = (t.parent.localPosition.x - bounds.min.x) / bounds.size.x * 2 - 1;
                myData.yBlend = (t.parent.localPosition.y - bounds.min.y) / bounds.size.y * 2 - 1;
                myData.zBlend = (t.parent.localPosition.z - bounds.min.z) / bounds.size.z * 2 - 1;

                effectDatas.Add(myData);

                var renderer = t.gameObject.GetComponent<MeshRenderer>();
                var material = new Material(renderer.sharedMaterial);
                material.SetFloat("_ID", (myData.id + 0.5f) / 20f);
                renderer.sharedMaterial = material;
            });

            maxID = 0;
            int maxGroup = 0;
            Dictionary<int, int> maxGroupIDs = new Dictionary<int, int>();

            effectDatas.ForEach(e =>
            {
                maxID = Math.Max(maxID, e.id);
                maxGroup = Math.Max(maxGroup, e.group);

                if (!maxGroupIDs.ContainsKey(e.group))
                {
                    maxGroupIDs.Add(e.group, e.groupID);
                }
                maxGroupIDs[e.group] = Math.Max(maxGroupIDs[e.group], e.groupID);
            });

            effectDatas.ForEach(e =>
            {
                e.idBlend = e.id / (float)maxID * 2 - 1;
                e.groupBlend = maxGroup > 0 ? e.group / (float)maxGroup * 2 - 1 : 0;
                e.groupIDBlend = maxGroupIDs[e.group] > 0 ? e.groupID / (float)maxGroupIDs[e.group] * 2 - 1 : 0;
            });
        }

        private void GetEffectObjects(GameObject theParent)
        {

            var childCount = theParent.transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                GameObject myChild = theParent.transform.GetChild(i).gameObject;
                GetEffectObjects(myChild);
                if (myChild.GetComponent<CCEffectData>() != null)
                {
                    effectNodes.Add(myChild);
                }
            }
        }

        private void Init()
        {
            effectNodes.Clear();
            GetEffectObjects(effectParent);
            effectDatas.Clear();
            effectNodes.ForEach(go => InitBounds(go));

            InitEffectData(effectNodes);
            effects.Clear();
            effects.AddRange(GetComponentsInChildren<CCEffect>());
            effects.ForEach(effect =>
            {
                effect.effects = this;
                effect.dataAmounts.Clear();
                for (int i = 0; i <= maxID; i++)
                {
                    effect.dataAmounts.Add(1);
                }
            });

            filters = GetComponents<CCFilter>();
        }

        private void OnValidate()
        {
            Init();
        }

        // Use this for initialization
        void Start()
        {
            Init();
        }

        public bool useDataAmount = true;
        public float userAmountScale = 1;


        public float blendAmp = 1;

        public CCInterpolator interpolator;

        public CCEffectModulation modulation;

        public float EffectBlend(CCEffectData theData, float theBlend)
        {
            if (theBlend == 0) return 0;
            if (theBlend == 1) return 1;

            float myOffsetSum = modulation.OffsetSum() * blendAmp;

            float myModulation = modulation.Modulation(theData) * blendAmp;
            float myBlend = (myModulation - myOffsetSum) + theBlend * (1 + myOffsetSum * 2);
            myBlend = CCMath.Saturate(myBlend);

            if (interpolator != null) myBlend = interpolator.Interpolate(myBlend);

            return myBlend;
        }

        // Update is called once per frame
        void Update()
        {

            effects.ForEach(effect => effect.Simulate(Time.deltaTime));

            effectNodes.ForEach(element =>
            {

                float myAngle = 0;
                CCEffectData myData = element.GetComponent<CCEffectData>();

                effects.ForEach(effect =>
                {
                    float myEffectAngle = effect.Apply(myData) * EffectBlend(myData, effect.amount);
                    if (float.IsNaN(myEffectAngle))
                    {
                        myEffectAngle = 0;
                    }
                    myAngle += myEffectAngle;
                });

                foreach (CCFilter filter in filters)
                {
                    myAngle = filter.Process(myData.id, myAngle, Time.deltaTime);
                }

                myAngle *= amount * (useDataAmount ? myData.amount * userAmountScale : 1);

                var localAngles = element.transform.localEulerAngles;
                myData.angle = myAngle * Mathf.Deg2Rad;
                element.transform.localEulerAngles = new Vector3(localAngles.x, localAngles.y, myAngle);

            });
        }

    }
}
