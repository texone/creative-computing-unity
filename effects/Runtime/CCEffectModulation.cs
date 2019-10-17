using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cc.creativecomputing.effects
{
    [System.Serializable]
    public struct CCEffectModulation
    {
        [Range(-1, 1)]
        public float constant;

        [Range(-1, 1)]
        public float x;
        [Range(-1, 1)]
        public float y;
        [Range(-1, 1)]
        public float z;
        [Range(-1, 1)]
        public float random;

        public uint randomSeed;

        [Range(-1, 1)]
        public float id;
        [Range(-1, 1)]
        public float idMod;
        [Range(-1, 1)]
        public float groupId;
        [Range(-1, 1)]
        public float group;
        [Range(-1, 1)]
        public float groupMod;
        [Range(-1, 1)]
        public float dist;

        public float Modulation(CCEffectData theData)
        {
            return
                X(theData) +
                Y(theData) +
                Z(theData) +
                Id(theData) +
                IdMod(theData) +
                Group(theData) +
                GroupId(theData) +
                GroupMod(theData) +
                Distance(theData) +
                Random(theData) +
                Constant(theData);
        }

        public float X(CCEffectData theData)
        {
            return theData.xBlend * x;
        }

        public float Y(CCEffectData theData)
        {
            return theData.yBlend * y;
        }

        public float Z(CCEffectData theData)
        {
            return theData.zBlend * z;
        }

        public float Random(CCEffectData theData)
        {
            return theData.Random(randomSeed) * random;
        }

        public float Id(CCEffectData theData)
        {
            return theData.idBlend * id;
        }

        public float IdMod(CCEffectData theData)
        {
            return ((theData.id % 2) * 2 - 1) * idMod;
        }

        public float Constant()
        {
            return constant;
        }

        public float Group(CCEffectData theData)
        {
            return theData.groupBlend * group;
        }

        public float GroupId(CCEffectData theData)
        {
            return theData.groupIDBlend * groupId;
        }

        public float GroupMod(CCEffectData theData)
        {
            return ((theData.group % 2) * 2 - 1) * groupMod;
        }

        public float Distance(CCEffectData theData)
        {
            return theData.distBlend * dist;
        }

        public float Constant(CCEffectData theData)
        {
            return constant;
        }

        public float OffsetSum()
        {
            return
                x +
                y +
                z +
                id +
                idMod +
                group +
                groupMod +
                groupId +
                dist +
                random +
                constant;
        }
    }
}
