using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class Spring : MonoBehaviour
{

    private Animator _anim;

    [SerializeField] private float jumpMultiply = 2f;

    private void Awake ()
    {
        _anim = GetComponent<Animator>();
    }



    private void OnCollisionEnter2D ( Collision2D collision )
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().ExecuteJump(jumpMultiply);
            _anim.SetTrigger("Release");
        }
    }
}
