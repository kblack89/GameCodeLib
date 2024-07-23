using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public Material outlineMaterial; // Reference to the outline material created with Shader Graph or any other outline shader
    private Material[] defaultMaterials; // Default materials of the object
    private BoxCollider detectionCollider; // Box collider to detect player's gaze
    public bool isHighlighted = false; // Track if the object is highlighted

    void Start()
    {
        // Get the default materials of the object
        Renderer renderer = GetComponent<Renderer>();
        defaultMaterials = renderer.materials;

        // Retrieve the existing Box Collider or add one if not present
        detectionCollider = GetComponent<BoxCollider>();
        if (detectionCollider == null)
        {
            detectionCollider = gameObject.AddComponent<BoxCollider>();
        }

        // Ensure element 0 and element 1 are initially the same material
        if (defaultMaterials.Length > 1)
        {
            defaultMaterials[1] = defaultMaterials[0];
        }

    }

    void Update()
    {
        RaycastHit hit;
        Vector3 rayOrigin = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (detectionCollider.Raycast(new Ray(rayOrigin, Camera.main.transform.forward), out hit, Mathf.Infinity))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject == gameObject)
            {
                // Apply outline effect if not already highlighted
                if (!isHighlighted)
                {
                    ApplyOutlineEffect(true);
                    isHighlighted = true;
                 
                }
            }
            else
            {
                // Remove outline effect if highlighted
                if (isHighlighted)
                {
                    ApplyOutlineEffect(false);
                    isHighlighted = false;
                 
                }
            }
        }
        else
        {
            // Remove outline effect if highlighted
            if (isHighlighted)
            {
                ApplyOutlineEffect(false);
                isHighlighted = false;
               
            }
        }
    }

    void ApplyOutlineEffect(bool apply)
    {
        Renderer renderer = GetComponent<Renderer>();
        Material[] materials = renderer.materials;

        if (apply)
        {
            materials[1] = outlineMaterial;
        }
        else
        {
            materials[1] = defaultMaterials[1];
        }

        renderer.materials = materials;
    }
}
