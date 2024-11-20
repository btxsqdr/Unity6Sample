using UnityEngine;
using UnityEngine.UIElements;

namespace ARB.TextureLoader.Demos
{
    public class ScrollViewPointerManipulator : PointerManipulator
    {
        private int pointerId;
        private bool isDragging;
        private Vector3 lastPointerPosition;


        public ScrollViewPointerManipulator()
        {
            pointerId = -1;
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            isDragging = false;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected void OnPointerDown(PointerDownEvent e)
        {
            if (isDragging)
            {
                e.StopImmediatePropagation();
                return;
            }

            if (CanStartManipulation(e))
            {
                pointerId = e.pointerId;
                isDragging = true;
                lastPointerPosition = e.localPosition;
                target.CapturePointer(pointerId);
                //e.StopPropagation();
            }
        }

        protected void OnPointerMove(PointerMoveEvent e)
        {
            if (!isDragging || !target.HasPointerCapture(pointerId)) return;

            Vector2 pointerDelta = e.localPosition - lastPointerPosition;
            ScrollView scrollView = (ScrollView)target;

            if (scrollView.mode == ScrollViewMode.Vertical)
            {
                pointerDelta.x = 0;
            }
            else if (scrollView.mode == ScrollViewMode.Horizontal)
            {
                pointerDelta.y = 0;
            }

            scrollView.scrollOffset -= pointerDelta;
            lastPointerPosition = e.localPosition;
            //e.StopPropagation();
        }

        protected void OnPointerUp(PointerUpEvent e)
        {
            if (!isDragging || !target.HasPointerCapture(pointerId) || !CanStopManipulation(e)) return;

            isDragging = false;
            target.ReleaseMouse();
            //e.StopPropagation();
        }
    }
}
