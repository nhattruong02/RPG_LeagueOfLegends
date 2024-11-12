using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public enum PlayerAttackType { NormalAttack, Ranged, Heath, Mage };
    public PlayerAttackType heroAttackType;

    public GameObject targetedEnemy;
    public float attackRange;
    public float rotateSpeedForAttack;

    private Player _playerScript;
    public Stats statsScript;
    private Animator _animator;
    public bool basicAtkIdle = false;
    public bool isPlayerAlive = true;
    public bool performNormalAttack = true;

    [SerializeField] private Animator enemyAnimator;

    [Header("Ranged Variables")]
    public bool performRangedAttack = true;
    [SerializeField] GameObject rangedPrefab;
    [SerializeField] Transform spawnSkill1;
    [SerializeField] float attackRanged;

    [Header("Mage Variables")]
    public bool performMage = true;
    [SerializeField] GameObject magePrefab;
    [SerializeField] float attackMage;

    [Header("Health Variables")]
    public bool performHealth = true;
    [SerializeField] GameObject healthPrefab;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    // Start is called before the first frame update
    void Start()
    {
        _playerScript = GetComponent<Player>();
        statsScript = GetComponent<Stats>();
        _animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (targetedEnemy != null && isPlayerAlive)
        {
            if (heroAttackType == PlayerAttackType.NormalAttack)
            {
                attackRange = 2;
                if (Vector3.Distance(gameObject.transform.position, targetedEnemy.transform.position) > attackRange)
                {
                    _playerScript.Agent.SetDestination(targetedEnemy.transform.position);
                    _playerScript.Agent.stoppingDistance = attackRange;

                    Quaternion rotationToLookAt = Quaternion.LookRotation(targetedEnemy.transform.position - transform.position);
                    float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y,
                        rotationToLookAt.eulerAngles.y,
                        ref _playerScript.rotateVelocity,
                        rotateSpeedForAttack * (Time.deltaTime * 5));
                    transform.eulerAngles = new Vector3(0, rotationY, 0);
                }
                else
                {
                    if (performNormalAttack)
                    {
                        StartCoroutine(NormalAttackInterval());
                    }
                }
            }
            if (heroAttackType == PlayerAttackType.Ranged)
            {
                if (performRangedAttack)
                {
                    attackRange = attackRanged;
                    _playerScript.Agent.stoppingDistance = attackRange;
                }
            }
            if (heroAttackType == PlayerAttackType.Mage)
            {
                if (performMage)
                {
                    attackRange = attackRanged;
                    _playerScript.Agent.stoppingDistance = attackRange;
                }
            }
        }
    }

    public IEnumerator HealthInterval()
    {
        performHealth = false;
        _animator.SetTrigger(Common.recoverHP);
        yield return new WaitForSeconds(statsScript.attackTime / ((100 + statsScript.attackTime) * 0.01f));
        performHealth = true;
    }

    public IEnumerator MageAttackInterval()
    {
        performMage = false;
        _animator.SetTrigger(Common.mageAttack);
        if (targetedEnemy == null)
        {
            performMage = false;
        }
        yield return new WaitForSeconds(statsScript.attackTime / ((100 + statsScript.attackTime) * 0.01f));
        performMage = true;
    }

    public IEnumerator RangedAttackInterval()
    {
        performRangedAttack = false;
        _animator.SetTrigger(Common.rangedAttack);
        if (targetedEnemy == null)
        {
            performRangedAttack = false;
        }
        yield return new WaitForSeconds(statsScript.attackTime / ((100 + statsScript.attackTime) * 0.01f));
        performRangedAttack = true;
    }

    IEnumerator NormalAttackInterval()
    {
        performNormalAttack = false;
        _animator.SetTrigger(Common.normalAttack);
        if (targetedEnemy == null)
        {
            performNormalAttack = false;
        }
        yield return new WaitForSeconds(statsScript.attackTime / ((100 + statsScript.attackTime) * 0.01f));
        performNormalAttack = true;

    }

    //event animator
    public void NormalAttack()
    {
        if (targetedEnemy != null)
        {
            if (targetedEnemy.GetComponent<Targetable>().CompareTag(Common.enemy))
            {
                _playerScript.transform.rotation = targetedEnemy.transform.rotation;
                statsScript.TakeDamage(targetedEnemy, statsScript.attackDmg);
            }
        }
        performNormalAttack = true;
    }

    //event animator
    public void RangedAttack()
    {
        if (targetedEnemy != null)
        {
            if (targetedEnemy.GetComponent<Targetable>().CompareTag(Common.enemy))
            {
                _playerScript.transform.rotation = targetedEnemy.transform.rotation;
                _playerScript.Agent.stoppingDistance = attackRange;
                spawnRangedAttack(targetedEnemy);
            }
        }
        performRangedAttack = true;
    }

    //event animator
    public void MageAttack()
    {
        if (targetedEnemy != null)
        {
            if (targetedEnemy.GetComponent<Targetable>().CompareTag(Common.enemy))
            {
                _playerScript.transform.rotation = targetedEnemy.transform.rotation;
                spawnMageAttack(targetedEnemy);
            }
        }
        performRangedAttack = true;
    }

    //event animator
    public void RecoveryHP()
    {
        healthPrefab.GetComponent<Skill2>().player = _playerScript.gameObject;
        performHealth = true;
        Instantiate(healthPrefab);
    }



    private void spawnMageAttack(GameObject targetedEnemy)
    {
        magePrefab.GetComponent<Skill3>().target = targetedEnemy;
        Instantiate(magePrefab);
    }

    private void spawnRangedAttack(GameObject targetedEnemy)
    {
        rangedPrefab.GetComponent<Skill1>().target = targetedEnemy;
        Instantiate(rangedPrefab, spawnSkill1.transform.position, this.gameObject.transform.rotation);

    }
}
