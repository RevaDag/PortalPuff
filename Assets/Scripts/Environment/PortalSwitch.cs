using UnityEngine;
using System.Collections;


public class PortalSwitch : MonoBehaviour, IInteractable
{
    public Portal connectedPortal;
    private Animator _anim;
    [SerializeField] private MovingPlatform movingPlatform;

    private bool isInteracting;
    public float cooldown = 1.0f;

    private void Awake ()
    {
        _anim = GetComponent<Animator>();
    }

    public void Interact ( GameObject player )
    {
        if (!isInteracting)
        {
            StartCoroutine(HandleInteraction());
        }
    }

    private IEnumerator HandleInteraction ()
    {
        isInteracting = true;

        _anim.SetTrigger("Switch");

        if (connectedPortal != null)
        {
            connectedPortal.SwitchType();
        }
        else
        {
            Debug.LogWarning("No connected portal assigned to the switch.");
        }

        if (movingPlatform != null)
        {
            movingPlatform.TogglePlatform();
        }

        yield return new WaitForSeconds(cooldown);

        isInteracting = false;
    }
}
