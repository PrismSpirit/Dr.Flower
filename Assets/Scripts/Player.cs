﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    private new AudioSource audio;
    BoxCollider2D b_collider;
    public GameObject clearSet;
    public GameObject dontClearSet;
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
    bool sliding;
    System.DateTime currentTime;
    static float h;
    int sliding_distance;

    void sliding_collider()
    {
        b_collider.offset = new Vector2(0.03f,-0.434f);
        b_collider.size = new Vector2(1.564f,1.132f);
    }
    void reset_collider()
    {
        b_collider.offset = new Vector2(0f,-0.171f);
        b_collider.size = new Vector2(0.9f,1.659f);
    }
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        sliding_distance = 300;
        CanJump = true;
        b_collider = GetComponent<BoxCollider2D>();
        reset_collider();
    }
    
    void Update()
    {
        if (Input.GetButton("Down")&&!sliding)
        {
            if (Input.GetButton("Jump") && CanJump)
            {
                CanJump = false;
                currentTime = DateTime.Now;
                sliding = true;
                sliding_collider();
                anim.SetBool("IsSliding", true);
                return;
            }
        }
        
        if (sliding)
        {
            System.TimeSpan diff = DateTime.Now - currentTime;
            if (diff.Milliseconds < sliding_distance)
            {
                if (spriteRenderer.flipX == true)
                {
                    rigid.velocity = new Vector2((-1) * maxSpeed*2f, rigid.velocity.y);
                }
                else
                {
                    rigid.velocity = new Vector2(maxSpeed*2f, rigid.velocity.y);
                }
            }
            else
            {
                reset_collider();
                anim.SetBool("IsSliding", false);
                rigid.velocity = new Vector2(0, rigid.velocity.y);
                sliding = false;
            }
            return;
        }
        
        //jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("IsJumping") && CanJump)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("IsJumping", true);
        }
        //move
        //flip player
        if (Input.GetButton("Horizontal"))
        {
            h = Input.GetAxisRaw("Horizontal");
            spriteRenderer.flipX = h == -1;
            rigid.AddForce(Vector2.right * (h) * Time.deltaTime*50, ForceMode2D.Impulse);
            
        }
        else
        {
            h = 0;
        }
        
        rigid.AddForce(Vector2.right * ((-1) * windpower) * Time.deltaTime * 50, ForceMode2D.Impulse);
        //break when button up
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * reduceSpeed, rigid.velocity.y);
        }
        
        //set,reset walk animation
        if (Mathf.Abs(rigid.velocity.x) < 1.5f)
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
            audio.Play();
            collision.gameObject.SetActive(false);
            manager.GetItem(flowerCount);
        }
        else if (collision.gameObject.tag == "Flower2")
        {
            flowerCount++;
            flowerCount++;
            audio.Play();
            collision.gameObject.SetActive(false);
            manager.GetItem(flowerCount);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            Time.timeScale = 0;
            // Clear
            if (flowerCount >= (manager.totalItemCount * 4 / 5))
            {
                clearSet.SetActive(true);
                if (!ButtonManager.clearStateDic.ContainsKey(SceneManager.GetActiveScene().name))
                    ButtonManager.clearStateDic.Add(SceneManager.GetActiveScene().name, true);
            }
            // Restart
            else
            {
                dontClearSet.SetActive(true);
            }
        }
        
    }

    void FixedUpdate()
    {
        //move(left,right)
        /*float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * (h-windpower), ForceMode2D.Impulse);*/





        //maxSpeed control
        if (rigid.velocity.x > maxSpeed && !sliding)
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
        else if (rigid.velocity.x < maxSpeed * (-1) && !sliding)
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


        Vector2 frontVec = new Vector2(rigid.position.x + 0.44f, rigid.position.y);
        Vector2 backVec = new Vector2(rigid.position.x + -0.44f, rigid.position.y);
        RaycastHit2D rayHit_f = Physics2D.Raycast(frontVec, Vector3.down, 1.01f, LayerMask.GetMask("Platform"));
        RaycastHit2D rayHit_b = Physics2D.Raycast(backVec, Vector3.down, 1.01f, LayerMask.GetMask("Platform"));
        //set layer as "Platform"
        if (rayHit_f.collider != null || rayHit_b.collider != null)
        {    
            CanJump = true;
            anim.SetBool("IsJumping", false);
        }
        else
        {
            CanJump = false;
        }
        
    }
}
