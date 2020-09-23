using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   
    [SerializeField]
    private float _speed = 5.0f;
    [SerializeField]
    private float _gravity = 1;
    [SerializeField]
    private float _jumpheight = 15.0f;
    private Vector3 _direction;
    private CharacterController _cc;
    private Animator _anim;
    private bool _jumping = false;
    private bool _onLedge;
    private Ledge _activeLedge;


    // Start is called before the first frame update
    void Start()
    {
        _cc = GetComponent<CharacterController>();
        _anim = GetComponentInChildren<Animator>();

        if (_cc == null)
            Debug.LogError("CC is NULL");

        if (_anim == null)
            Debug.LogError("ANIMATOR IS NULL");
    }

    // Update is called once per frame
    void Update()
    {

        CalculateMovement();
       
        if (_onLedge == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _anim.SetTrigger("ClimbUp");
            }
        }

    }

    void CalculateMovement()
    {
        if (_cc.isGrounded == true)
        {
            if (_jumping == true)
            {
                _jumping = false;
                _anim.SetBool("Jumping", _jumping);
            }

            float horizontal = Input.GetAxisRaw("Horizontal");
            _direction = new Vector3(0, 0, horizontal) * _speed;

            _anim.SetFloat("Speed", Mathf.Abs(horizontal));

            if (horizontal != 0) //Character flip
            {
                Vector3 facing = transform.localEulerAngles;
                facing.y = _direction.z > 0 ? 0 : 180;
                transform.localEulerAngles = facing;
            }


            //jumping
            if (Input.GetKeyDown(KeyCode.Space))
            { 

                _direction.y += _jumpheight;
                _jumping = true;
                _anim.SetBool("Jumping", _jumping);

            }
        }

        _direction.y -= _gravity * Time.deltaTime; //apply gravity

        _cc.Move(_direction * Time.deltaTime);

    }

    public void GrabLedge(Vector3 handPos, Ledge currentLedge)
    {
        _cc.enabled = false;
        _anim.SetBool("GrabLedge", true);
        _anim.SetFloat("Speed", 0.0f);
        _anim.SetBool("Jumping", false);
        _onLedge = true;
        transform.position = handPos;
        _activeLedge = currentLedge;
    }

    public void ClimbUpComplete() //snap position
    {
        transform.position = _activeLedge.GetStandPos();
        _anim.SetBool("GrabLedge", false);
        _cc.enabled = true;
    }
}
