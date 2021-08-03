using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;

    public GameObject[] grenades;
    public int ammo;
    public int coin;
    public int health;
    public int hasGrenades;
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    Rigidbody rigid;
    Animator anim;
    float hAxis;
    float vAxis;
    bool isBorder;
    bool wDown;
    bool jDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool fDown;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady = true;

    Vector3 moveVec;
    Vector3 dodgeVec;

    GameObject nearObject;
    int equipWeaponIndex = -1;

    Weapon equipWeapon;
    public GameObject[] weapons;
    public bool[] hasWeapons;

    float fireDelay;

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
        Attack();
        Dodge();
        Swap();
        Interaction();
    }

    void GetInput() {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move() {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge) {
            moveVec = dodgeVec;
        }

        if (isSwap || !isFireReady) {
            moveVec = Vector3.zero;
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
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap) {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            isJump = true;
            anim.SetTrigger("doJump");
            anim.SetBool("isJump", isJump);
        }
    }

    void Attack() {
        if (equipWeapon == null) {
            return;
        }

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap) {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Dodge() {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap) {
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

    void Swap() {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) {
            return;
        }
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) {
            return;
        }
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2)) {
            return;
        }

        int weaponIndex = -1;
        if (sDown1) {
            weaponIndex = 0;
        }
        if (sDown2) {
            weaponIndex = 1;
        }
        if (sDown3) {
            weaponIndex = 2;
        }

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge) {
            if (equipWeapon != null) {
                equipWeapon.gameObject.SetActive(false);
            }

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            weapons[weaponIndex].SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut() {
        isSwap = false;
    }

    void Interaction() {
        if (iDown && nearObject != null && !isJump && !isDodge) {
            if (nearObject.tag == "Weapon") {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Item") {
            Item item = other.GetComponent<Item>();

            switch (item.type) {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo) {
                        ammo = maxAmmo;
                    }
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin) {
                        coin = maxCoin;
                    }
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth) {
                        health = maxHealth;
                    }
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades) {
                        hasGrenades = maxHasGrenades;
                    }
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.tag == "Weapon") {
            nearObject = other.gameObject;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Floor") {
            isJump = false;
            anim.SetBool("isJump", isJump);
        }
    }
}
