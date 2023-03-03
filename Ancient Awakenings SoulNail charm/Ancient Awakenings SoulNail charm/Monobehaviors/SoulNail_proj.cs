using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;

namespace Ancient_Awakenings_SoulNail_charm.Monobehaviors
{
    public class SoulNail_proj:MonoBehaviour
    {

        private float timeToTransform = 1f;
        private float projSpeed = 30;

        private float transformTimer;
        private Animator anim;
        private HealthManager target;
        private float disableTimer;
        private bool targeted;

        public void Restart()
        {
            disableTimer = 0;
            anim.SetBool("transformed", false);
            anim.SetFloat("sibilingType", UnityEngine.Random.Range(0, 3));
            transformTimer = 0;
            targeted = false;

            transform.rotation = Quaternion.identity;
        }

        public void Collide()
        {
            disableTimer = 3f-Time.deltaTime-0.001f;
        }

        private void Start()
        {
            anim = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            Restart();
        }

        private void GetTarget()
        {
            foreach(HealthManager health in FindObjectsOfType<HealthManager>())
            {
                if (target==null || (
                    Vector3.Magnitude(health.transform.position-transform.position)<Vector3.Magnitude(target.transform.position-transform.position)+2f
                    &&
                    Vector3.Dot(Vector3.Normalize(health.transform.position-transform.position),transform.up)>
                    Vector3.Dot(Vector3.Normalize(target.transform.position-transform.position),transform.up))) { 

                    target = health;
                }
            }
        }

        private void Update()
        {

            if (target == null)
            {
                GetTarget();
            }

            transformTimer += Time.deltaTime;

            if (transformTimer > timeToTransform){
                if (!targeted)
                {
                    transform.up =  Vector3.Normalize(target.transform.position - transform.position);
                    targeted = true;
                }

                anim.SetBool("transformed", true);
                transform.position += transform.up * Time.deltaTime*projSpeed;

                if (target != null)
                {
                    transform.up = Vector3.Lerp(transform.up, Vector3.Normalize(target.transform.position - transform.position), Time.deltaTime * 10);
                }
            }
            else
            {
                anim.SetBool("transformed", false);
                transform.up=Vector3.up;
            }

            disableTimer += Time.deltaTime;
            if (disableTimer >= 3f)
            {
                gameObject.SetActive(false);
            }
        }

    }
}
