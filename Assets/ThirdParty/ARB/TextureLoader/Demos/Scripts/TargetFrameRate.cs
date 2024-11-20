using UnityEngine;

namespace ARB.TextureLoader.Demos
{
    public class TargetFrameRate : MonoBehaviour
    {
        void Start()
        {
            Application.targetFrameRate = 60;
        }
    }
}
