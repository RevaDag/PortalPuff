using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Lever : MonoBehaviour, IInteractable
{
    public Portal switchTypePortal;
    public Portal switchTargetPortal;

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
        AudioManager.Instance?.PlaySFX("Lever");
        _anim.SetTrigger("Switch");

        if (switchTypePortal != null)
        {
            switchTypePortal.SwitchType();
        }

        if (movingPlatform != null)
        {
            movingPlatform.TogglePlatform();
        }

        if(switchTargetPortal != null)
        {
            switchTargetPortal.SwitchTarget();  
        }

        yield return new WaitForSeconds(cooldown);

        isInteracting = false;
    }
}
