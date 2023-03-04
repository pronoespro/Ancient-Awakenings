using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ancient_Awakenings_SoulNail_charm.Monobehaviors
{
    public class ProjCollision : MonoBehaviour
    {

        public SoulNail_proj proj;

        public void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.gameObject.layer > 10)
            {
                proj.Collide();
            }
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.layer >10)
            {
                proj.Collide();
            }
        }

    }
}
