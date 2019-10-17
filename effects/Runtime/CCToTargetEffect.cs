using UnityEngine;

namespace cc.creativecomputing.effects
{
    [AddComponentMenu("effects/effect/to target")]
    public class CCToTargetEffect : CCEffect
    {
        public GameObject target;

        public override float Apply(CCEffectData theData)
        {
            Vector3 toTarget = target.transform.position - theData.gameObject.transform.parent.position;

            Vector2 toTargetXZ = new Vector2(toTarget.x, toTarget.z);
            Vector2 elementDir = new Vector2(theData.gameObject.transform.parent.forward.x, theData.gameObject.transform.parent.forward.z);
            float dot = Vector2.Dot(toTarget, elementDir);

            float angle = Mathf.Atan2(-toTarget.y, toTargetXZ.magnitude) * Mathf.Rad2Deg;
            float d = -angle / effects.amount;
            //d = Mathf.Clamp(d, -1, 1);
            return d;
        }

        public override void Simulate(float theDeltaTime){ }
    }

}