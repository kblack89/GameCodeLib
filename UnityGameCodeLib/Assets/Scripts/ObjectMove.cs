using UnityEngine;

public class ObjectMove : MonoBehaviour
{
    private OutlineController outlineController; // Reference to OutlineController script
    private bool isMoving = false; // Flag to track if movement should occur
    private Rigidbody rb; // Reference to Rigidbody component
    private Vector3 initialPosition; // Initial position of the object when movement starts

    // Maximum distance the object can move from its initial position
    public float maxMoveDistance = 5f;

    void Start()
    {
        // Get the OutlineController script attached to the same GameObject
        outlineController = GetComponent<OutlineController>();

        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        if (outlineController == null)
        {
            Debug.LogError("OutlineController component not found on the same GameObject!");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on the same GameObject! Add a Rigidbody component to enable physics.");
        }
        else
        {
            // Make the Rigidbody kinematic initially so it doesn't fall due to gravity
            rb.isKinematic = true;
        }

        // Record initial position
        initialPosition = transform.position;
    }

    void Update()
    {
        // Check if the object is highlighted by OutlineController
        if (outlineController != null && outlineController.isHighlighted)
        {
            // Start moving the object if highlighted and left mouse button is held down
            if (Input.GetMouseButtonDown(0))
            {
                StartObjectMove();
            }
            // Stop moving the object if left mouse button is released
            if (Input.GetMouseButtonUp(0))
            {
                StopObjectMove();
            }
        }

        // Move the object if isMoving flag is true
        if (isMoving)
        {
            MoveObject();
        }
    }

    void StartObjectMove()
    {
        isMoving = true;
        // Enable Rigidbody kinematic to allow manual movement
        rb.isKinematic = false;

        // Record initial position
        initialPosition = transform.position;
    }

    void StopObjectMove()
    {
        isMoving = false;
        // Disable Rigidbody kinematic to let physics take over
        rb.isKinematic = true;

        // Snap object back to initial position if it exceeds maxMoveDistance
        if (Vector3.Distance(transform.position, initialPosition) > maxMoveDistance)
        {
            transform.position = initialPosition;
        }
    }

    void MoveObject()
    {
        // Get current mouse position in world coordinates
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            Vector3 targetPosition = hitInfo.point;

            // Clamp movement within maxMoveDistance
            if (Vector3.Distance(targetPosition, initialPosition) > maxMoveDistance)
            {
                // Calculate direction towards target within maxMoveDistance
                Vector3 direction = (targetPosition - initialPosition).normalized;
                targetPosition = initialPosition + direction * maxMoveDistance;
            }

            // Move the object towards the clamped target position using Rigidbody.MovePosition
            rb.MovePosition(targetPosition);
        }
    }
}
