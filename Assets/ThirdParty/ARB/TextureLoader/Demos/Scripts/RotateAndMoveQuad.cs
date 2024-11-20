using UnityEngine;

namespace ARB.TextureLoader.Demos
{
    public class RotateAndMoveQuad : MonoBehaviour
    {
        // Rotation speed in degrees per second
        private float rotationSpeed = 50f;

        // Movement speed in units per second
        private float movementSpeed = 2f;

        // Range of the up and down movement
        private float movementRange = 2f;

        private Vector3 startPosition;

        private void Start()
        {
            rotationSpeed = Random.Range(10, 46);
            movementSpeed = Random.Range(2, 5);
            movementRange = Random.Range(2, 9);
            startPosition = transform.position;
        }

        private void Update()
        {
            float rotationThisFrame = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationThisFrame);

            float newY = startPosition.y + Mathf.Sin(Time.time * movementSpeed) * movementRange;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}