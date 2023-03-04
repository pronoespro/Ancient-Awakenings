using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Ancient_Awakenings_SoulNail_charm.Monobehaviors
{
    [RequireComponent(typeof(Canvas))]
    public class AncientUI : MonoBehaviour
    {

        private Canvas canvas;

        private void Start()
        {
            canvas = GetComponent<Canvas>();

            CanvasGroup _soulnaildisplay = transform.GetChild(0).GetComponent<CanvasGroup>();
            if (_soulnaildisplay != null)
            {
                _soulnaildisplay.alpha = 0;
            }
        }

        private void Update()
        {

            if (HeroController.instance != null)
            {
                if (canvas != null)
                {
                    CanvasGroup _soulnaildisplay = transform.GetChild(0).GetComponent<CanvasGroup>();
                    if (_soulnaildisplay != null)
                    {
                        if (PlayerData.instance.GetBool(nameof(PlayerData.instance.equippedCharm_15)))
                        {
                            _soulnaildisplay.alpha = 1;

                            Slider hitMetter = _soulnaildisplay.transform.GetChild(0).GetComponent<Slider>();
                            if (hitMetter != null)
                            {
                                Vector2 hits = Ancient_Awakenings_SoulNail_charm.Instance.GetSoulNailHit();
                                hitMetter.maxValue = hits.y - 1;
                                hitMetter.value = hits.x % hits.y;
                            }
                        }
                        else
                        {
                            _soulnaildisplay.alpha = 0;
                        }
                    }
                }
            }
            else
            {
                CanvasGroup _soulnaildisplay = transform.GetChild(0).GetComponent<CanvasGroup>();
                if (_soulnaildisplay != null)
                {
                    _soulnaildisplay.alpha = 0;
                }
            }
        }

    }
}
