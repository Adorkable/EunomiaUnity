using Eunomia;
using UnityEngine;

namespace EunomiaUnity
{
    public class FitWithAspect : MonoBehaviour
    {
        private RectTransform rectTransform;
        private RectTransform parentRectTransform;
        private float startWidth;
        private float startHeight;
        private float ratio;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            parentRectTransform = rectTransform.parent.gameObject.GetComponent<RectTransform>();
            startWidth = rectTransform.sizeDelta.x;
            startHeight = rectTransform.sizeDelta.y;
            ratio = startWidth / startHeight;
        }

        void Update()
        {
            if (rectTransform.sizeDelta.x != parentRectTransform.sizeDelta.x && rectTransform.sizeDelta.y != parentRectTransform.sizeDelta.y)
            {
                MatchParentSize();
            }
        }

        void MatchParentSize()
        {
            var result = Math.FitWithAspect((startWidth, startHeight), (parentRectTransform.sizeDelta.x, parentRectTransform.sizeDelta.y));
            rectTransform.sizeDelta = new Vector2(result.Item1, result.Item2);
        }
    }
}
