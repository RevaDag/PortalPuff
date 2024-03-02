using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Lever : MonoBehaviour, IInteractable
{
    private Animator _anim;
    [SerializeField] private Portal switchTypePortal;
    [SerializeField] private Portal switchTargetPortal;


    [SerializeField] private MovingPlatform movingPlatform;

    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private float effectSpeed = 100f;

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
            StartCoroutine(MoveEffectTowardsTarget(switchTypePortal.gameObject));
            switchTypePortal.SwitchType();

        }

        if (movingPlatform != null)
        {
            StartCoroutine(MoveEffectTowardsTarget(movingPlatform.gameObject));
            movingPlatform.TogglePlatform();

        }

        if (switchTargetPortal != null)
        {
            StartCoroutine(MoveEffectTowardsTarget(switchTargetPortal.gameObject));
            switchTargetPortal.SwitchTarget();
        }

        yield return new WaitForSeconds(cooldown);

        isInteracting = false;
    }

    private IEnumerator MoveEffectTowardsTarget ( GameObject target )
    {
        Vector3 targetPosition = target.transform.position;

        // Instantiate the effect object at the lever's position
        GameObject effectInstance = Instantiate(effectPrefab, transform.position, Quaternion.identity);

        // Move the effect towards the target
        while (Vector3.Distance(effectInstance.transform.position, targetPosition) > 0.1f)
        {
            effectInstance.transform.position = Vector3.MoveTowards(effectInstance.transform.position, targetPosition, effectSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }

        Destroy(effectInstance); // Optionally destroy the effect object upon reaching the target
    }

}
