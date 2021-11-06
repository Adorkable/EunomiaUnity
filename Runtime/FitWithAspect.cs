using Eunomia;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public class FitWithAspect : MonoBehaviour
    {
        private RectTransform parentRectTransform;
        private float ratio;
        private RectTransform rectTransform;
        private float startHeight;
        private float startWidth;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            parentRectTransform = rectTransform.parent.gameObject.GetComponent<RectTransform>();

            var sizeDelta = rectTransform.sizeDelta;
            startWidth = sizeDelta.x;
            startHeight = sizeDelta.y;

            ratio = startWidth / startHeight;
        }

        private void Update()
        {
            if (rectTransform.sizeDelta.x != parentRectTransform.sizeDelta.x &&
                rectTransform.sizeDelta.y != parentRectTransform.sizeDelta.y)
            {
                MatchParentSize();
            }
        }

        private void MatchParentSize()
        {
            var sizeDelta = parentRectTransform.sizeDelta;
            var (x, y) = Math.FitWithAspect((startWidth, startHeight),
                (sizeDelta.x, sizeDelta.y));
            rectTransform.sizeDelta = new Vector2(x, y);
        }
    }
}