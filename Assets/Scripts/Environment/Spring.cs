using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class Spring : MonoBehaviour
{

    private Animator _anim;

    private void Awake ()
    {
        _anim = GetComponent<Animator>();
    }



    private void OnCollisionEnter2D ( Collision2D collision )
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().SpringJump();
            _anim.SetTrigger("Release");
        }
    }
}
