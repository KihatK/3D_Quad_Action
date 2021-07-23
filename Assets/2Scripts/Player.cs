using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;

    Rigidbody rigid;
    Animator anim;
    float hAxis;
    float vAxis;
    bool isBorder;
    bool wDown;
    bool jDown;
    bool isJump;
    bool isDodge;

    Vector3 moveVec;
    Vector3 dodgeVec;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate() {
        StopToWall();
    }

    void StopToWall() {
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    private void Update() {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
    }

    void GetInput() {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
    }

    void Move() {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge) {
            moveVec = dodgeVec;
        }

        if (!isBorder) {
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        }

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn() {
        //rotation by keyboard
        transform.LookAt(transform.position + moveVec);
    }

    void Jump() {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge) {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            isJump = true;
            anim.SetTrigger("doJump");
            anim.SetBool("isJump", isJump);
        }
    }

    void Dodge() {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge) {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut() {
        speed *= 0.5f;
        isDodge = false;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Floor") {
            isJump = false;
            anim.SetBool("isJump", isJump);
        }
    }
}
