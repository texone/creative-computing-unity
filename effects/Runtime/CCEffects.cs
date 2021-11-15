using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cc.creativecomputing.math.interpolate;
using cc.creativecomputing.math.util;
using cc.creativecomputing.ui;

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
        public List<CCEffect> effects = new List<CCEffect>();

        private CCFilter[] _filters;
        private CCEffectApplier[] _effectAppliers;
        public CCAbstractEffectDataRetriever effectDataRetriever;
        
        public float amount = 35;

        private void InitBounds(GameObject theEffectedNode)
        {
            if (bounds.center.Equals(new Vector3()))
            {
                bounds = new Bounds(theEffectedNode.transform.position, new Vector3());
            }
            bounds.Encapsulate(theEffectedNode.transform.position);
        }

        public int maxID = 0;

        private void InitEffectData(List<GameObject> theEffectNodes)
        {
            theEffectNodes.ForEach(go =>
            {
                var t = go.transform;

                var myData = t.gameObject.GetComponent<CCEffectData>();
                var position = t.position;
                myData.x = (position.x - bounds.min.x) / bounds.size.x * 2 - 1;
                myData.y = (position.y - bounds.min.y) / bounds.size.y * 2 - 1;
                myData.z = (position.z - bounds.min.z) / bounds.size.z * 2 - 1;

                effectDatas.Add(myData);
            });

            maxID = 0;
            var maxGroup = 0;
            var maxUnit = 0;
            var maxGroupIDs = new Dictionary<int, int>();
            var maxUnitIDs = new Dictionary<int, int>();

            effectDatas.ForEach(e =>
            {
                maxID = Math.Max(maxID, e.id);
                maxGroup = Math.Max(maxGroup, e.group);
                maxUnit = Math.Max(maxGroup, e.unit);

                if (!maxGroupIDs.ContainsKey(e.group))
                {
                    maxGroupIDs.Add(e.group, e.groupID);
                }
                if (!maxUnitIDs.ContainsKey(e.unit))
                {
                    maxUnitIDs.Add(e.unit, e.unitID);
                }
                maxGroupIDs[e.group] = Math.Max(maxGroupIDs[e.group], e.groupID);
                maxUnitIDs[e.unit] = Math.Max(maxUnitIDs[e.unit], e.unitID);
            });

            effectDatas.ForEach(e =>
            {
                e.idBlend = e.id / (float)maxID * 2 - 1;
                e.groupBlend = maxGroup > 0 ? e.group / (float)maxGroup * 2 - 1 : 0;
                e.groupIDBlend = maxGroupIDs[e.group] > 0 ? e.groupID / (float)maxGroupIDs[e.group] * 2 - 1 : 0;
                e.unitBlend = maxUnit > 0 ? e.unit / (float)maxUnit * 2 - 1 : 0;
                e.unitIDBlend = maxUnitIDs[e.unit] > 0 ? e.unitID / (float)maxUnitIDs[e.unit] * 2 - 1 : 0;
            });
        }
        
        [CCUIButton]
        private void Init()
        {
            Debug.Log("INIT");
            effectDataRetriever = effectDataRetriever ? effectDataRetriever : gameObject.AddComponent<CCEffectDataRetreiver>();

            effectNodes = effectDataRetriever.RetrieveDatas(effectParent);
            effectDatas.Clear();
            effectNodes.ForEach(InitBounds);

            InitEffectData(effectNodes);
            effects.Clear();
            effects.AddRange(GetComponentsInChildren<CCEffect>());
            effects.ForEach(effect =>
            {
                effect.effects = this;
                effect.dataAmounts.Clear();
                for (var i = 0; i <= maxID; i++)
                {
                    effect.dataAmounts.Add(1);
                }
            });

            _filters = GetComponents<CCFilter>() ?? new CCFilter[0];
            _effectAppliers = GetComponents<CCEffectApplier>() ?? new CCEffectApplier[0];
            
            foreach (var ccEffectApplier in _effectAppliers)
            {
                foreach (var ccEffectData in effectDatas)
                {
                    ccEffectApplier.SetupEffectData(ccEffectData);
                }
            }
        }

        private void OnValidate()
        {
            //Init();
        }

        // Use this for initialization
        private void Start()
        {
            Init();
        }

        public bool useDataAmount = true;
        public float userAmountScale = 1;


        public float blendAmp = 1;

        public CCInterpolator interpolator;

        public CCEffectModulation modulation;
        private static readonly int ID = Shader.PropertyToID("_ID");

        public float EffectBlend(CCEffectData theData, float theBlend)
        {
            switch (theBlend)
            {
                case 0:
                    return 0;
                case 1:
                    return 1;
            }

            var myOffsetSum = modulation.OffsetSum() * blendAmp;

            var myModulation = modulation.Modulation(theData) * blendAmp;
            var myBlend = (myModulation - myOffsetSum) + theBlend * (1 + myOffsetSum * 2);
            myBlend = CCMath.Saturate(myBlend);

            if (interpolator) myBlend = interpolator.Interpolate(myBlend);

            return myBlend;
        }

        // Update is called once per frame
        private void Update()
        {

            effects.ForEach(effect => effect.Simulate(Time.deltaTime));

            effectNodes.ForEach(element =>
            {

                float myEffectOutput = 0;
                var myData = element.GetComponent<CCEffectData>();

                effects.ForEach(effect =>
                {
                    var myEffectAmount = effect.Apply(myData) * EffectBlend(myData, effect.amount);
                    //if(myData.id == 0)Debug.Log(myEffectAmount);
                    if (float.IsNaN(myEffectAmount))
                    {
                        myEffectAmount = 0;
                    }
                    myEffectOutput += myEffectAmount;
                });

                foreach (var filter in _filters)
                {
                    myEffectOutput = filter.Process(myData.id, myEffectOutput, Time.deltaTime);
                }
                myEffectOutput *= amount * (useDataAmount ? myData.amount * userAmountScale : 1);

                foreach (var applier in _effectAppliers)
                {
                    applier.ApplyEffect(myData, myEffectOutput);
                }

            });
        }

    }
}
