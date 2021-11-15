using System.Collections.Generic;
using UnityEngine;

namespace cc.creativecomputing.effects
{
    public class CCEffectDataRetreiver : CCAbstractEffectDataRetriever
    {
        private int _myGroupdID = -1;
        private int _myID = 0;
        
        public bool autoID = true;
        public bool autoGroupID = true;
        private void GetEffectObjects(GameObject theParent, ICollection<GameObject> effectNodes)
        {

            var childCount = theParent.transform.childCount;
            var startNewGroup = false;
            for (var i = 0; i < childCount; i++)
            {
                var myChild = theParent.transform.GetChild(i).gameObject;
                GetEffectObjects(myChild, effectNodes);
                var effectData = myChild.GetComponent<CCEffectData>();
                if (effectData == null) continue;
                
                if (!startNewGroup)
                {
                    startNewGroup = true;
                    _myGroupdID++;
                }

                if (autoID)
                {
                    effectData.id = _myID++;
                }

                if (autoGroupID)
                {
                    effectData.groupID = _myGroupdID;
                }
                effectNodes.Add(myChild);
            }
        }

        public override List<GameObject> RetrieveDatas(GameObject theParent)
        {
            _myGroupdID = -1;
            _myID = 0;
            var effectNodes = new List<GameObject>();
            GetEffectObjects(theParent, effectNodes);
            return effectNodes;
        }
    }
}