using UnityEngine;

namespace EunomiaUnity
{
    public static class RectTransformExtensions
    {
        /// <summary>
        /// Aligns `this` with provided `RectTransform` in provided `RectTransform`'s parent
        /// </summary>
        /// <param name="rectTransform">`RectTranform` to align</param>
        /// <param name="alignWith">`RectTranform` to align to and whose parent is used</param>
        public static void AlignWithInParent(this RectTransform rectTransform, RectTransform alignWith)
        {
            rectTransform.SetParent(alignWith.parent);
            rectTransform.anchorMin = alignWith.anchorMin;
            rectTransform.anchorMax = alignWith.anchorMax;
            rectTransform.anchoredPosition3D = alignWith.anchoredPosition3D;
            rectTransform.pivot = alignWith.pivot;
            rectTransform.sizeDelta = alignWith.sizeDelta;
            rectTransform.localEulerAngles = new Vector3(0, 0, 0);
            rectTransform.localScale = new Vector3(1, 1, 1);
        }

        /// <summary>
        /// *Untested, do not use yet*
        /// Aligns `this` with provided `RectTransform` and makes self a child of said `RectTransform`
        /// </summary>
        /// <param name="rectTransform">`RectTransform` to align</param>
        /// <param name="alignWith">`RectTransform` to align to and who to make new parent</param>
        public static void AlignWithAsChild(this RectTransform rectTransform, RectTransform alignWith)
        {
            rectTransform.SetParent(alignWith);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.localEulerAngles = new Vector3(0, 0, 0);
            rectTransform.localScale = new Vector3(1, 1, 1);
        }
    }
}