using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    public GameObject clearSet;
    public GameObject clearState;
    public Dictionary<string, bool> clearStateDic;
    public float jumpPower;
    public float maxSpeed;
    public float reduceSpeed;
    public int flowerCount;
    public GameManagerLogic manager;
    public bool windswitch;
    public float windpower;
    public float maxSpeedMultiplierwithWind;
    bool CanJump;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        CanJump = true;
        
    }

    void Update()
    {
        //jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("IsJumping") && CanJump)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("IsJumping", true);
        }
        //break when button up
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * reduceSpeed, rigid.velocity.y);
        }
        //flip player
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
        //set,reset walk animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3f)
        {
            anim.SetBool("IsWalking", false);
        }
        else
        {
            anim.SetBool("IsWalking", true);
        }


    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Flower")
        {
            flowerCount++;
            collision.gameObject.SetActive(false);
            manager.GetItem(flowerCount);
        }
        else if (collision.gameObject.tag == "Flower2")
        {
            flowerCount++;
            flowerCount++;
            collision.gameObject.SetActive(false);
            manager.GetItem(flowerCount);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            clearSet.SetActive(true);
            Time.timeScale = 0;
            // Clear
            if (flowerCount > manager.totalItemCount * (4/5))
            {
                clearSet.SetActive(true);
                Time.timeScale = 0;
                if (!ButtonManager.clearStateDic.ContainsKey(SceneManager.GetActiveScene().name))
                    ButtonManager.clearStateDic.Add(SceneManager.GetActiveScene().name, true);
            }
            // Restart
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    void FixedUpdate()
    {
        //move(left,right)
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * (h-windpower), ForceMode2D.Impulse);





        //maxSpeed control
        if (rigid.velocity.x > maxSpeed)
        {
            if(windswitch)
            {
                if(windpower > 0)
                {
                    if (h > 0 && rigid.velocity.x > maxSpeed / (maxSpeedMultiplierwithWind + windpower))
                    {
                        rigid.velocity = new Vector2(h*maxSpeed / (maxSpeedMultiplierwithWind + windpower), rigid.velocity.y);
                    }   
                }
                else if(windpower < 0)
                {
                    
                    if (h > 0 && rigid.velocity.x > maxSpeed * (maxSpeedMultiplierwithWind + (-1)*windpower))
                    {
                        rigid.velocity = new Vector2(h * maxSpeed * (maxSpeedMultiplierwithWind + (-1) * windpower), rigid.velocity.y);
                    }
                    else if (h == 0)
                    {
                        rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
                    }
                }
                
            }
            else
            {
                rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
            }
        }
        else if (rigid.velocity.x < maxSpeed * (-1))
        {
            if (windswitch)
            {
                if (windpower > 0)
                {    
                    if (h < 0 && rigid.velocity.x < (-1) * maxSpeed * (maxSpeedMultiplierwithWind + windpower))
                    {
                        rigid.velocity = new Vector2(h * maxSpeed * (maxSpeedMultiplierwithWind + windpower), rigid.velocity.y);
                    }
                    else if (h == 0)
                    {
                        rigid.velocity = new Vector2((-1) * maxSpeed, rigid.velocity.y);
                    }
                }
                else if (windpower < 0)
                {
                    if (h < 0 && rigid.velocity.x < (-1) * maxSpeed / (maxSpeedMultiplierwithWind + (-1) * windpower))
                    {
                        rigid.velocity = new Vector2(h * maxSpeed / (maxSpeedMultiplierwithWind + (-1) * windpower), rigid.velocity.y);
                    }
                }
            }
            else
            {
                rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
            }
        }


        Vector2 frontVec = new Vector2(rigid.position.x + 0.43f, rigid.position.y);
        Vector2 backVec = new Vector2(rigid.position.x + -0.43f, rigid.position.y);
        RaycastHit2D rayHit_f = Physics2D.Raycast(frontVec, Vector3.down, 1.2f, LayerMask.GetMask("Platform"));
        RaycastHit2D rayHit_b = Physics2D.Raycast(backVec, Vector3.down, 1.2f, LayerMask.GetMask("Platform"));
        //set layer as "Platform"
        if (rayHit_f.collider != null || rayHit_b.collider != null)
        {
            CanJump = true;
            if (rayHit_f.distance < 1.3f || rayHit_b.distance < 1.3f)
                anim.SetBool("IsJumping", false);
        }
        else
        {
            CanJump = false;
        }
        
    }
}
