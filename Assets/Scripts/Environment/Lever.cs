using UnityEngine;
using System.Collections;

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

        GameObject effectInstance = Instantiate(effectPrefab, transform.position, Quaternion.identity);

        while (Vector3.Distance(effectInstance.transform.position, targetPosition) > 0.1f)
        {
            effectInstance.transform.position = Vector3.MoveTowards(effectInstance.transform.position, targetPosition, effectSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(effectInstance);
    }

}
