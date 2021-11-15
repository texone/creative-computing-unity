using System.Collections.Generic;
using UnityEngine;

namespace cc.creativecomputing.effects
{
    public abstract class CCAbstractEffectDataRetriever : MonoBehaviour
    {
        public abstract List<GameObject> RetrieveDatas(GameObject theParent);
    }
}