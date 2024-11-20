using UnityEngine;

namespace ARB.TextureLoader.Extensions
{
    /// <summary>
    /// Extension methods for RectTransform.
    /// </summary>
    internal static class RectTransformExtensions
    {
        /// <summary>
        /// RectTransform anchor points.
        /// </summary>
        public enum Anchor
        {
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight,
            StretchLeft,
            StretchCenter,
            StretchRight,
            StretchTop,
            StretchMiddle,
            StretchBottom,
            Stretch
        }

        /// <summary>
        /// Sets anchor without changing size and with the option to set position.
        /// </summary>
        /// <param name="rectTransform">RectTransform object that this function is being run on</param>
        /// <param name="anchor">Anchor point to set on this RectTransform</param>
        /// <param name="shouldSetPosition">True to force the RectTransform to reposition to its new anchor point</param>
        public static void SetAnchor(this RectTransform rectTransform, Anchor anchor, bool shouldSetPosition = false)
        {
            switch (anchor)
            {
                case Anchor.TopLeft:
                    rectTransform.SetAnchors(new Vector2(0f, 1f), new Vector2(0f, 1f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(0f, 1f);
                    break;
                case Anchor.TopCenter:
                    rectTransform.SetAnchors(new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(0.5f, 1f);
                    break;
                case Anchor.TopRight:
                    rectTransform.SetAnchors(new Vector2(1f, 1f), new Vector2(1f, 1f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(1f, 1f);
                    break;
                case Anchor.MiddleLeft:
                    rectTransform.SetAnchors(new Vector2(0f, 0.5f), new Vector2(0f, 0.5f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(0f, 0.5f);
                    break;
                case Anchor.MiddleCenter:
                    rectTransform.SetAnchors(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case Anchor.MiddleRight:
                    rectTransform.SetAnchors(new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(1f, 0.5f);
                    break;
                case Anchor.BottomLeft:
                    rectTransform.SetAnchors(new Vector2(0f, 0f), new Vector2(0f, 0f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(0f, 1f);
                    break;
                case Anchor.BottomCenter:
                    rectTransform.SetAnchors(new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(0.5f, 0f);
                    break;
                case Anchor.BottomRight:
                    rectTransform.SetAnchors(new Vector2(1f, 0f), new Vector2(1f, 0f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(1f, 0f);
                    break;
                case Anchor.StretchLeft:
                    rectTransform.SetAnchors(new Vector2(0f, 0f), new Vector2(0f, 1f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(0f, 0.5f);
                    break;
                case Anchor.StretchCenter:
                    rectTransform.SetAnchors(new Vector2(0.5f, 0f), new Vector2(0.5f, 1f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case Anchor.StretchRight:
                    rectTransform.SetAnchors(new Vector2(1f, 0f), new Vector2(1f, 1f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(1f, 0.5f);
                    break;
                case Anchor.StretchTop:
                    rectTransform.SetAnchors(new Vector2(0f, 1f), new Vector2(1f, 1f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(0.5f, 1f);
                    break;
                case Anchor.StretchMiddle:
                    rectTransform.SetAnchors(new Vector2(0f, 0.5f), new Vector2(1f, 0.5f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case Anchor.StretchBottom:
                    rectTransform.SetAnchors(new Vector2(0f, 0f), new Vector2(1f, 0f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(0.5f, 0f);
                    break;
                case Anchor.Stretch:
                    rectTransform.SetAnchors(new Vector2(0f, 0f), new Vector2(1f, 1f));
                    if (shouldSetPosition) rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    break;
            }

            if (shouldSetPosition)
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }

        /// <summary>
        /// Sets min/max anchors without changing size and position.
        /// </summary>
        public static void SetAnchors(this RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax)
        {
            Vector2 size = rectTransform.rect.size;
            Vector2 localPosition = rectTransform.localPosition;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.sizeDelta = size;
            rectTransform.localPosition = localPosition;
        }

        /// <summary>
        /// Sets pivot without changing position.
        /// </summary>
        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            Vector2 size = rectTransform.rect.size;
            Vector2 deltaPivot = rectTransform.pivot - pivot;
            Vector3 deltaPosition = new(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }

        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }

        public static bool IsInsideScreen(this RectTransform rt, Camera camera = null)
        {
            if (rt == null) return false;

            Rect screenBounds = new(0f, 0f, Screen.width, Screen.height);

            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            foreach (Vector3 corner in corners)
            {
                Vector3 point = camera == null ? corner : camera.WorldToScreenPoint(corner);
                if (!screenBounds.Contains(point)) return false;
            }

            return true;
        }

        public static bool IsPartiallyInsideScreen(this RectTransform rt, Camera camera = null)
        {
            if (rt == null) return false;

            Rect screenBounds = new(0f, 0f, Screen.width, Screen.height);

            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            foreach (Vector3 corner in corners)
            {
                Vector3 point = camera == null ? corner : camera.WorldToScreenPoint(corner);
                if (screenBounds.Contains(point)) return true;
            }

            return false;
        }

        public static bool IsInside(this RectTransform rt, RectTransform rectTransform)
        {
            if (rectTransform == null || rt == null) return false;

            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            Vector3[] rectTransformCorners = new Vector3[4];
            rectTransform.GetWorldCorners(rectTransformCorners);

            foreach (Vector3 corner in corners)
            {
                if (!IsPointInside(rectTransformCorners, corner)) return false;
            }

            return true;
        }

        public static bool IsPartiallyInside(this RectTransform rt, RectTransform rectTransform)
        {
            if (rectTransform == null || rt == null) return false;

            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            Vector3[] rectTransformCorners = new Vector3[4];
            rectTransform.GetWorldCorners(rectTransformCorners);

            foreach (Vector3 corner in corners)
            {
                if (IsPointInside(rectTransformCorners, corner)) return true;
            }

            return false;
        }

        private static bool IsPointInside(Vector3[] rectCorners, Vector3 point)
        {
            return point.x >= rectCorners[0].x && point.x <= rectCorners[2].x &&
                   point.y >= rectCorners[0].y && point.y <= rectCorners[2].y;
        }
    }
}
