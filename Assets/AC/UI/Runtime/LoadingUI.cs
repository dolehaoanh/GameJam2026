using System.Collections;
using UnityEngine;
using AC.GameTool.UI;
using TMPro;
using AC.Core;
using AC.Attribute;
using DG.Tweening;
using System;
using UnityEngine.UI;

namespace AC.GameTool.UI
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _txtLoading;
        [SerializeField, ReadOnlly] protected float _loadPercent;
        [SerializeField] Slider _loadingBar;
        [SerializeField] Image _slideImage;
        [SerializeField] float _timeSpeed;

        Tween _loadTween;
        private void Awake()
        {
            RegisterEventCallback();
            //StartLoading(5);
        }

        private void OnDestroy()
        {
            _loadTween.Kill();
            RemoveEventCallback();
        }
        void RegisterEventCallback()
        {
            // Dang ky su kien o day
        }

        void RemoveEventCallback()
        {
            // Xoa nhan su kien.

        }

        protected virtual void OnClick()
        {

        }

        public void StartLoading(float minTimeLoad, Action completed = null, params CheckLoadCompleted[] checkLoadCompleted)
        {
            _loadPercent = 0;
            ShowLoadPercent(_loadPercent);
            StartCoroutine(LoadingUIparocess(minTimeLoad, completed, checkLoadCompleted));
        }

        IEnumerator LoadingUIparocess(float minTimeLoad, Action completed = null, params CheckLoadCompleted[] checkLoadCompleted)
        {
            float timeDelta = minTimeLoad / (checkLoadCompleted.Length + 1);
            float percentDelta = 1f / (checkLoadCompleted.Length + 1);
            for (int i = 0; i < checkLoadCompleted.Length; i++)
            {
                float timeLoading = 0;
                while (!checkLoadCompleted[i].IsLoadCompleted)
                {
                    yield return null;
                    timeLoading += Time.unscaledDeltaTime;
                }
                FakeLoadPercent(percentDelta * (i + 1), timeLoading, timeDelta);
                yield return new WaitForSecondsRealtime(Mathf.Max(timeDelta - timeLoading, 0.1f));
            }
            FakeLoadPercent(1f, 0f, timeDelta);
            yield return new WaitForSecondsRealtime(timeDelta);
            completed?.Invoke();
        }

        void FakeLoadPercent(float newPercent, float currentTime, float timeDelta)
        {
            float timeRunAni = timeDelta - currentTime;
            timeRunAni = Mathf.Max(timeRunAni, 0.1f);
            _loadTween.Kill();
            _loadTween = DOTween.To(() => _loadPercent, per => _loadPercent = per, newPercent, timeRunAni).SetEase((Ease)UnityEngine.Random.Range(1, 5)).OnUpdate(() =>
            {
                ShowLoadPercent(_loadPercent);
            }).OnComplete(() =>
            {
                _loadPercent = newPercent;
                ShowLoadPercent(_loadPercent);
            });
        }

        public virtual void ShowLoadPercent(float percent)
        {
            if (_txtLoading != null)
            {
                _txtLoading.SetText("Loading...{0}%", Mathf.FloorToInt(percent * 100f));
            }

            if (_loadingBar != null)
            {
                _loadingBar.value = percent;
            }

            if (_slideImage != null)
            {
                _slideImage.fillAmount = percent * _timeSpeed;
            }
        }
    }
}

