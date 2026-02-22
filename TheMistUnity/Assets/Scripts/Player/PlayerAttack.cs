using System;
using System.Collections;
using BayatGames.SaveGameFree;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAttack : MonoBehaviour
{
    [Header("Config")] 
    [SerializeField] private Weapon initialWeapon;
    [SerializeField] private Transform[] attackPositions;
    
    [Header("Melee Config")]
    [SerializeField] private ParticleSystem slashFX;
    [SerializeField] private float minDistanceMeleeAttack;
    [SerializeField] private LayerMask enemyLayer;
    
    public Weapon CurrentWeapon {get; set;}
    
    private PlayerActions actions;
    private PlayerAnimations playerAnimations;
    private PlayerMovement playerMovement;
    private PlayerMana playerMana;
    //private EnemyBrain enemyTarget;
    private Coroutine attackCoroutine;

    private Transform currentAttackPosition;
    private float currentAttackRotation;

    private void Awake()
    {
        actions = new PlayerActions();
        playerMana = GetComponent<PlayerMana>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimations = GetComponent<PlayerAnimations>();
        
    }

    private void Start()
    {
        actions.Attack.ClickAttack.performed += ctx => Attack();
    }

    private void Update()
    {
        GetFirePosition();
    }

    public void ResetInitialWeapon()
    {
        CurrentWeapon = initialWeapon;
    }

    private void Attack()
    {
        attackCoroutine = StartCoroutine(IEAttack());
    }

    private IEnumerator IEAttack()
    {
        if (currentAttackPosition == null)
        {
            yield break;    //IEnumerator version of return;
        }
        
        if (CurrentWeapon.WeaponType == WeaponType.Magic)
        {
            if (playerMana.CurrentMana < CurrentWeapon.RequiredMana)
            {
                yield break;   
            }
            MagicAttack();
        }
        else
        {
            MeleeAttack();
        }
        
        playerAnimations.SetAttackAnimation(true);
        yield return new WaitForSeconds(0.5f);
        playerAnimations.SetAttackAnimation(false);
    }

    private void MagicAttack()
    {
        AudioManager.Instance.PlayPlayerAttackMagicSound();
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, currentAttackRotation));
        Projectile projectile = Instantiate(CurrentWeapon.ProjectilePrefab, currentAttackPosition.position, rotation);
        projectile.Direction = Vector3.up;
        //projectile.Damage = GetAttackDamage();
        //playerMana.UseMana(CurrentWeapon.RequiredMana);
    }

    private void MeleeAttack()
    {
        AudioManager.Instance.PlayPlayerAttackMeleeSound();
        slashFX.transform.position = currentAttackPosition.position;
        slashFX.Play();
        
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, minDistanceMeleeAttack, enemyLayer);
        foreach (Collider2D enemy in enemiesInRange)
        {
            //enemy.GetComponent<IDamageable>()?.TakeDamage(GetAttackDamage());
        }
    }

    /*
    private float GetAttackDamage()
    {
        float damage = stats.BaseDamage;
        damage += CurrentWeapon.Damage;
        float randomPercent = Random.Range(0f, 100);
        
        if (randomPercent <= stats.CriticalChance)
        {
            damage += damage * (stats.CriticalDamage / 100f);
        }
        
        return damage;
    }
    */

    private void GetFirePosition()
    {
        Vector2 moveDirection = playerMovement.MoveDirection;
        switch (moveDirection.x)
        {
            case > 0f:  //Right
                currentAttackPosition = attackPositions[1];
                currentAttackRotation = -90f;
                break;
            case < 0f :  //Left
                currentAttackPosition = attackPositions[3];
                currentAttackRotation = -270f;
                break;
        }
        
        switch (moveDirection.y)
        {
            case > 0f:  //Up
                currentAttackPosition = attackPositions[0];
                currentAttackRotation = 0f;
                break;
            case < 0f :  //Down
                currentAttackPosition = attackPositions[2];
                currentAttackRotation = -180f;
                break;
        }
        
    }

    private void OnEnable()
    {
        actions.Enable();
    }

    private void OnDisable()
    {
        actions.Disable();
    }

}