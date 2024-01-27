using UnityEngine;

public class PortalSwitch : MonoBehaviour, IInteractable
{
    public Portal connectedPortal;
    private Animator _anim;

    private void Awake ()
    {
        _anim = GetComponent<Animator>();
    }

    public void Interact ( GameObject player )
    {
        if (connectedPortal != null)
        {
            _anim.SetTrigger("Switch");
            connectedPortal.SwitchType();
        }
        else
        {
            Debug.LogWarning("No connected portal assigned to the switch.");
        }
    }
}
