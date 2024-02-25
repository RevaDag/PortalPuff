using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarodevController;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : MonoBehaviour, IInteractable
{
    public enum TeleportType
    {
        None = 0,
        Shrink = 1,
        Expand = 2,
        Multiply = 3,
        Gravity = 4
    }

    public enum ElectricityColor
    {
        Blue = 0,
        Pink = 1
    }

    public enum PortalColor
    {
        Blue,
        Green,
        Pink,
        Yellow,
        Red
    }

    [Header("Portal Settings")]
    public PortalColor portalColor;

    [SerializeField] private TeleportType currentType;
    [SerializeField] private TeleportType alternativeType;

    [SerializeField] private Portal currentTargetPortal;
    [SerializeField] private Portal alternativeTargetPortal;

    private float shrinkScale = 0.5f;

    private Animator portalCircleAnim;

    private Transform spawnPoint;

    [Header("Electricity")]
    [SerializeField] private ElectricityColor electricityColor = ElectricityColor.Blue;
    [SerializeField] private Animator electricityAnim;
    [SerializeField] private Sprite[] lightSprites;

    [Header("Portal Icon")]
    private SpriteRenderer iconSpriteRenderer;
    [SerializeField] private Transform portalIcon;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Sprite[] icons;

    private bool isInteracting;
    [SerializeField] private float cooldown = 1.0f;



    private void Awake ()
    {
        portalCircleAnim = GetComponentInChildren<Animator>();
        iconSpriteRenderer = portalIcon.GetComponent<SpriteRenderer>();
        spawnPoint = transform.GetChild(0).GetComponent<Transform>();
    }

    private void Start ()
    {
        ApplyPortalColor();
        ApplyAnimationType();
    }

    private void ApplyPortalColor ()
    {

        SpriteRenderer gateRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        switch (portalColor)
        {
            case PortalColor.Blue:
                gateRenderer.color = new Color(0.34f, 0.89f, 0.96f);
                break;

            case PortalColor.Green:
                gateRenderer.color = new Color(0.49f, 0.95f, 0.30f);
                break;

            case PortalColor.Pink:
                gateRenderer.color = new Color(0.96f, 0.34f, 0.96f);
                break;

            case PortalColor.Yellow:
                gateRenderer.color = new Color(0.96f, 0.94f, 0.34f);
                break;

            case PortalColor.Red:
                gateRenderer.color = new Color(0.96f, 0.34f, 0.40f);
                break;
        }
    }

    private void ApplyAnimationType ()
    {
        int targetPortalColorIndex = Array.IndexOf(Enum.GetValues(typeof(PortalColor)), currentTargetPortal.portalColor);
        portalCircleAnim.SetInteger("Type", targetPortalColorIndex);

        if (currentType != TeleportType.None)
        {
            iconSpriteRenderer.sprite = icons[(int)currentType - 1];
        }
        else
        {
            iconSpriteRenderer.sprite = null;
        }

        if (electricityColor == ElectricityColor.Pink)
        {
            electricityAnim.SetInteger("Color", (int)electricityColor);
            SpriteRenderer[] spriteRenderers = electricityAnim.GetComponentsInChildren<SpriteRenderer>();

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].sprite = lightSprites[(int)electricityColor];
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

        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.Freeze();

        AudioManager.Instance?.PlaySFX("Portal");

        PlayerAnimator playerAnimator = player.GetComponentInChildren<PlayerAnimator>();
        playerAnimator.Fade(1, 0);

        ApplyTeleportEffect(player);
        yield return new WaitForSeconds(0.3f);

        if (currentTargetPortal != null)
        {
            player.transform.position = currentTargetPortal.spawnPoint.position;
        }


        playerAnimator.Fade(0, 1);

        playerController.Unfreeze();
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

    public void SwitchTarget ()
    {
        Portal temp = currentTargetPortal;
        currentTargetPortal = alternativeTargetPortal;
        alternativeTargetPortal = temp;

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
        if (player.transform.localScale.x < 2)
        {
            Vector3 newScale = player.transform.localScale / shrinkScale;
            player.transform.localScale = newScale;
        }
    }
    private void MultiplyPlayer ( GameObject player )
    {
        PlayersManager.Instance?.CreateNewDuplication(player);
    }

    private void FlipPlayerGravity ( GameObject player )
    {
        player.GetComponent<PlayerController>().FlipGravity();
    }

}

