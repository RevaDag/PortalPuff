using System.Collections;
using System.Collections.Generic;
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
        Divide = 4
    }

    [SerializeField] private TeleportType currentType;
    [SerializeField] private TeleportType alternativeType;

    public GameObject pairedTeleporter;

    public float shrinkScale = 0.5f;

    private Animator portalCircleAnim;

    private void Awake ()
    {
        portalCircleAnim = GetComponentInChildren<Animator>();
    }

    private void Start ()
    {
        ApplyAnimationType();
    }

    private void ApplyAnimationType ()
    {
        portalCircleAnim.SetInteger("Type", (int)currentType);
    }

    public void Interact ( GameObject player )
    {
        if (pairedTeleporter != null)
        {
            player.transform.position = pairedTeleporter.transform.position;
        }

        ApplyTeleportEffect(player);
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
            case TeleportType.Divide:
                DividePlayer(player);
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
        //GameObject clonedPlayer = Instantiate(player);

        duplicationsManager.CreateNewDuplication(player);
    }

    private void DividePlayer ( GameObject player ) { }

}
