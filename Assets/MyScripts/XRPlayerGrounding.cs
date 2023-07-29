using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPlayerGrounding : MonoBehaviour
{
    public Transform groundingPoint;
    public LayerMask groundMask;
    public float groundDistance = 0.2f;
    public XRBaseController leftController;
    public XRBaseController rightController;

    private bool isGrounded;

    private void Update()
    {
        // Check if the XR Rig (player) is grounded
        isGrounded = Physics.CheckSphere(groundingPoint.position, groundDistance, groundMask);

        // If the XR Rig is not grounded, adjust its position to be grounded
        if (!isGrounded)
        {
            Vector3 groundedPosition = new Vector3(transform.position.x, groundingPoint.position.y - groundDistance, transform.position.z);
            transform.position = groundedPosition;

            // Optionally, you can reset the XR Controllers' position to avoid drift
            if (leftController != null && rightController != null)
            {
                leftController.transform.localPosition = Vector3.zero;
                rightController.transform.localPosition = Vector3.zero;
            }
        }
    }
}