using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ancient_Awakenings_SoulNail_charm
{

    public class HitData
    {
        public Transform attack, hitTarget;
        public float hitTime,maxTime;

        public HitData(Transform attack,Transform hitTarget,float maxTime,float hitTime = -1)
        {
            this.attack = attack;
            this.hitTarget = hitTarget;
            this.maxTime = maxTime;


            this.hitTime = (hitTime < 0) ? Time.time : hitTime;
        }

    }

    public class HitDataList
    {
        public List<HitData> dataList;

        public HitDataList()
        {
            dataList = new List<HitData>();
        }

        public void Update()
        {
            foreach (HitData dat in dataList)
            {
                if (Time.time > dat.hitTime + dat.maxTime)
                {
                    dataList.Remove(dat);
                    break;
                }
            }
        }

        public void Add(HitData data)
        {
            if (!dataList.Contains(data))
            {
                dataList.Add(data);
            }
        }

        public void Remove(HitData data)
        {
            if (dataList.Contains(data))
            {
                dataList.Remove(data);
            }
        }

        public bool Contains(Transform attack,Transform hitTarget)
        {
            foreach (HitData hit in dataList)
            {
                if (hit.attack == attack && hit.hitTarget == hitTarget)
                {
                    return true;
                }
            }
            return false;
        }

    }

}
