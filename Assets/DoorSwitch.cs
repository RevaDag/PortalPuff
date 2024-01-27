using UnityEngine;

public class DoorSwitch : MonoBehaviour, IInteractable
{
    public Door connectedDoor;
    private Animator _anim;
    private bool isInteracting;

    private void Awake ()
    {
        _anim = GetComponent<Animator>();
    }

    public void Interact ( GameObject player )
    {
        if (isInteracting) return;

        isInteracting = true;

        if (connectedDoor != null)
        {
            _anim.SetTrigger("Switch");
            connectedDoor.ToggleDoor();

        }
        else
        {
            Debug.LogWarning("No connected door assigned to the switch.");
        }

        isInteracting = false;
    }
}
