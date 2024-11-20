using System.Collections;
using ARB.TextureLoader.Extensions;
using UnityEngine;

namespace ARB.TextureLoader.Demos
{
    public class LoadingIndicatorView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform gradientRectTransform;

        private readonly float duration = 1;
        private RectTransform parent;
        private Vector2 startPosition;
        private Vector2 endPosition;
        private Coroutine coroutine;

        private void Awake()
        {
            parent = transform.parent as RectTransform;
            gradientRectTransform.SetAnchor(RectTransformExtensions.Anchor.MiddleCenter, true);
        }

        private void OnEnable() => PlayAnimation();

        private void OnDisable() => StopAnimation();

        private IEnumerator AnimateGradient()
        {
            gradientRectTransform.sizeDelta = parent.sizeDelta;
            startPosition = new Vector2(-parent.rect.width, 0);
            endPosition = new Vector2(parent.rect.width, 0);
            gradientRectTransform.anchoredPosition = startPosition;

            float time = Time.time;

            while (gradientRectTransform.anchoredPosition.x < endPosition.x)
            {
                gradientRectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, (Time.time - time) / duration);
                yield return null;
            }

            PlayAnimation();
        }

        private void PlayAnimation()
        {
            if (!gameObject.activeInHierarchy) return;
            StopAnimation();
            coroutine = StartCoroutine(AnimateGradient());
        }

        private void StopAnimation()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}
