using System.Collections.Generic;
using UnityEngine;

namespace cc.creativecomputing.render.util
{
    [CreateAssetMenu(fileName = "transform position data", menuName = "gpu data/transform position to texture")]
    public class CCTransformPositionProvider : CCDataProvider
    {

        public List<GameObject> gameObjects = new List<GameObject>();

        public override void GetData(RenderTexture theTargetMap, List<Vector4> theDataList)
        {
            if (gameObjects.Count <= 0) return;
            foreach (var t in gameObjects)
            {
                var position = t.transform.position;
                theDataList.Add(new Vector4(position.x,position.y,position.z,0));
            }
        }
    }
}