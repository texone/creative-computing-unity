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
        [Range(-1, 1)]
        public float angle;

        public uint randomSeed;

        [Range(-1, 1)]
        public float id;
        [Range(-1, 1)]
        public float idMod;
        [Range(-1, 1)]
        public float unit;
        [Range(-1, 1)]
        public float unitMod;
        [Range(-1, 1)]
        public float unitId;
        [Range(-1, 1)]
        public float unitIdMod;
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
                Unit(theData) +
                UnitMod(theData) +
                UnitId(theData) +
                UnitIdMod(theData) +
                Group(theData) +
                GroupId(theData) +
                GroupMod(theData) +
                Distance(theData) +
                Random(theData) +
                Constant(theData) + 
                Angle(theData);
        }

        public float ModulationWithoutPosition(CCEffectData theData)
        {
            return
                Id(theData) +
                IdMod(theData) +
                Unit(theData) +
                UnitMod(theData) +
                UnitId(theData) +
                UnitIdMod(theData) +
                Group(theData) +
                GroupId(theData) +
                GroupMod(theData) +
                Distance(theData) +
                Random(theData) +
                Constant(theData) + 
                Angle(theData);
        }

        public float X(CCEffectData theData)
        {
            return theData.x * x;
        }

        public float Y(CCEffectData theData)
        {
            return theData.y * y;
        }

        public float Z(CCEffectData theData)
        {
            return theData.z * z;
        }

        public float Angle(CCEffectData theData)
        {
            return theData.angle * angle;
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
        
        public float Unit(CCEffectData theData)
        {
            return theData.unitBlend * unit;
        }

        public float UnitMod(CCEffectData theData)
        {
            return ((theData.unit % 2) * 2 - 1) * unitMod;
        }
        
        public float UnitId(CCEffectData theData)
        {
            return theData.unitIDBlend * unitId;
        }

        public float UnitIdMod(CCEffectData theData)
        {
            return ((theData.unitID % 2) * 2 - 1) * unitIdMod;
        }

        public float Distance(CCEffectData theData)
        {
            return theData.dist * dist;
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
                angle +
                id +
                idMod +
                unitId +
                unitIdMod +
                group +
                groupMod +
                groupId +
                dist +
                random +
                constant;
        }
    }
}
