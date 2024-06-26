﻿using System;                                
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : C_Hero
{
    [SerializeField] private LayerMask _layerMaskOfEnemy;
    [Header("Mục tiêu")] [SerializeField] private Transform posTower;
    [SerializeField] private Slider _sliderHeart;
    private SpriteRenderer spriteRenderer;
    private Transform target;
    private Vector2 direc;
    private C_Hero Enemy;
    private float timer;
    private IState currentState;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Data.LoadData_Hero(this,this.gameObject.tag);
        onHeroUpdateLevel();
        //set huong di chuyen
        direc = gameObject.tag.Equals("Player") ? Vector2.right : Vector2.left;
        //set first target
        target = posTower;
        //set state begin
        changeState(new StateMove());
        spriteRenderer.sortingOrder = 1-(int)transform.position.y;
    }
    
    private void Update()
    {
        _sliderHeart.value = Heart;
        if (Enemy == null) target = posTower;
        Debug.DrawLine(transform.position,transform.position + (Vector3)direc * RangeAttack,Color.blue);
        if (this.Heart <= 0 && currentState != null)
        {
            changeState(new StateDeath());
        }
        if (currentState != null)
        {
            currentState.OnExecute(this);
        }
    }

    public void onHeroUpdateLevel()
    {
        _sliderHeart.maxValue = Heart;
    }
    public void changeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = newState;
        if (currentState != null)
        {
            currentState.OnEnter(this);
        }
    }

    public void DiChuyen()
    {
        transform.position =
            Vector3.MoveTowards(transform.position, target.position, this.SpeedMove * Time.deltaTime);
        if (checkEnemy())
        {
            changeState(new StateAttack());
        }
    }

    public void DungDiChuyen()
    {
        if (Enemy != null)
        {
            target = Enemy.transform;
        }
    }
    public void TanCong()
    {
        if (timer < this.SpeedAttack)
        {
            timer += Time.fixedDeltaTime;
        }
        else
        {
            timer = 0;
            Enemy.Heart -= Dame;
        }

        if (!checkEnemy())
        {
            changeState(new StateMove());
        }
    }

    public void DungTanCong()
    {
        timer = 0;
    }

    public void Chet()
    {
        if (this.gameObject.tag.Equals("Player"))
        {
            QueueHeroDied.Instance.addPlayerDied(this);
        }
        // is Enemy died
        else
        {
            GameManager.Instance.IncCoin(this.Gia);
            QueueHeroDied.Instance.addEnemyDied(this);
        }
        this.gameObject.SetActive(false);
    }

    public void HoiSinh()
    {
        this.gameObject.SetActive(true);
    }
    // private bool checkEnemy()
    // {
    //     RaycastHit2D hit = Physics2D.Raycast(transform.position, direc, this.tamDanh, _layerMaskOfEnemy);
    //     if (hit.collider != null)
    //     {
    //         Enemy = hit.collider.gameObject.GetComponent<C_Hero>();
    //     }
    //     else
    //     {
    //         Enemy = null;
    //     }
    //     return hit.collider != null;
    // }
    private bool checkEnemy()
    {
        Collider2D[] arrEnemy = Physics2D.OverlapCircleAll(transform.position, RangeAttack, _layerMaskOfEnemy);
        if (arrEnemy.Length != 0)
        {
            Enemy = arrEnemy[0].gameObject.GetComponent<C_Hero>();
        }
        else
        {
            Enemy = null;
        }
        return arrEnemy.Length != 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,this.RangeAttack);
    }
}