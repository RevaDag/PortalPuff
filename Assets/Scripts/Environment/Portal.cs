using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarodevController;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : MonoBehaviour, IInteractable
{

    [SerializeField] private PlayerDuplicationsManager duplicationsManager;
    public enum TeleportType
    {
        None = 0,
        Shrink = 1,
        Expand = 2,
        Multiply = 3,
        Gravity = 4
    }

    public enum PortalColor
    {
        Blue = 0,
        Pink = 1
    }


    [SerializeField] private TeleportType currentType;
    [SerializeField] private TeleportType alternativeType;

    public Portal pairedTeleporter;

    public float shrinkScale = 0.5f;

    private Animator portalCircleAnim;

    [Header("Electricity")]
    public PortalColor portalColor = PortalColor.Blue;
    [SerializeField] private Animator electricityAnim;
    [SerializeField] private Sprite[] lightSprites;

    [Header("Portal Icon")]
    public Transform portalIcon;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotationSpeed = 10f;
    private SpriteRenderer iconSpriteRenderer;
    [SerializeField] private Sprite[] icons;

    private bool isInteracting;
    public float cooldown = 1.0f;



    private void Awake ()
    {
        portalCircleAnim = GetComponentInChildren<Animator>();
        iconSpriteRenderer = portalIcon.GetComponent<SpriteRenderer>();
    }

    private void Start ()
    {
        ApplyAnimationType();
    }

    void Update ()
    {
        portalIcon.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }

    private void ApplyAnimationType ()
    {
        portalCircleAnim.SetInteger("Type", (int)currentType);

        if (currentType != TeleportType.None)
        {
            iconSpriteRenderer.sprite = icons[(int)currentType - 1];
        }
        else
        {
            iconSpriteRenderer.sprite = null;
        }

        if (portalColor == PortalColor.Pink)
        {
            electricityAnim.SetInteger("Color", (int)portalColor);
            SpriteRenderer[] spriteRenderers = electricityAnim.GetComponentsInChildren<SpriteRenderer>();

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].sprite = lightSprites[(int)portalColor];
            }
        }
    }

    public void Interact ( GameObject player )
    {
        if (!isInteracting)
        {
            StartCoroutine(HandleInteraction(player));
        }
    }

    private IEnumerator HandleInteraction ( GameObject player )
    {
        isInteracting = true;

        PlayerAnimator playerAnimator = player.GetComponentInChildren<PlayerAnimator>();
        playerAnimator.Fade(1, 0);

        ApplyTeleportEffect(player);
        yield return new WaitForSeconds(0.3f);

        if (pairedTeleporter != null)
        {
            player.transform.position = pairedTeleporter.portalIcon.position;
        }


        playerAnimator.Fade(0, 1);
        playerAnimator.ActivateAnimation(true);

        yield return new WaitForSeconds(cooldown);

        isInteracting = false;
    }

    private void ApplyTeleportEffect ( GameObject player )
    {
        switch (currentType)
        {
            case TeleportType.Shrink:
                ShrinkPlayer(player);
                break;
            case TeleportType.Expand:
                ExpandPlayer(player);
                break;
            case TeleportType.Multiply:
                MultiplyPlayer(player);
                break;
            case TeleportType.Gravity:
                FlipPlayerGravity(player);
                break;
        }
    }

    public void SwitchType ()
    {
        TeleportType temp = currentType;
        currentType = alternativeType;
        alternativeType = temp;

        ApplyAnimationType();
    }

    private void ShrinkPlayer ( GameObject player )
    {
        if (player.transform.localScale.x > 0.6)
        {
            Vector3 newScale = player.transform.localScale * shrinkScale;
            player.transform.localScale = newScale;
        }
    }
    private void ExpandPlayer ( GameObject player )
    {
        if (player.transform.localScale.x < 2.2)
        {
            Vector3 newScale = player.transform.localScale / shrinkScale;
            player.transform.localScale = newScale;
        }
    }
    private void MultiplyPlayer ( GameObject player )
    {
        duplicationsManager.CreateNewDuplication(player);
    }

    private void FlipPlayerGravity ( GameObject player )
    {
        player.GetComponent<PlayerController>().FlipGravity();
    }

}
