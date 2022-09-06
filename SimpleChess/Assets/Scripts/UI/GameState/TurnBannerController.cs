using System;
using AIPlayer;
using echo17.Signaler.Core;
using TMPro;
using UnityEngine;

namespace UI.GameState
{
    public class TurnBannerController : MonoBehaviour,ISubscriber
    {

        [Header("Objects")]
        [SerializeField] private GameObject layout;
        [SerializeField] private GameObject banner;
        [SerializeField] private TextMeshProUGUI turnText;
        [Header("Animations")]
        [SerializeField] private LeanTweenType bannerAnimationEase = LeanTweenType.easeInOutCubic;

        [SerializeField] private float bannerMoveInTime = 0.35f;
        [SerializeField] private float bannerHoldTime = 0.5f;
        [SerializeField] private float bannerMoveOutTime = 0.35f;

        private void Awake()
        {
            Signaler.Instance.Subscribe<StartPlayerTurn>(this, OnPlayerTurnStarted);
            Signaler.Instance.Subscribe<StartEnemyTurn>(this, OnEnemyTurnStarted);
        }

        private bool OnPlayerTurnStarted(StartPlayerTurn signal)
        {
            LeanTween.cancel(gameObject);
            
            turnText.text = "Player Turn";
            banner.transform.localPosition = new Vector3(0,-Screen.height,0); 
            layout.SetActive(true);
            AnimateBanner();
            return true;
        }

        private bool OnEnemyTurnStarted(StartEnemyTurn signal)
        {
            LeanTween.cancel(gameObject);

            turnText.text = "Enemy Turn";
            banner.transform.localPosition = new Vector3(0,-Screen.height,0); 
            layout.SetActive(true);
            AnimateBanner();
            return true;
        }

        private void AnimateBanner()
        {
            banner.SetActive(true);
            LeanTween.moveLocalY(banner, 0, bannerMoveInTime).setEase(bannerAnimationEase).setOnComplete(() =>
            {
                var seq = LeanTween.sequence();
                seq.append(bannerHoldTime);
                seq.append(() =>
                {
                    LeanTween.moveLocalY(banner, -Screen.height, bannerMoveOutTime).setEase(bannerAnimationEase).setOnComplete(() => banner.SetActive(false));
                });
            });
        }
    }
}
