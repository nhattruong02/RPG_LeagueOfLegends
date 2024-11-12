using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Skill1 : MonoBehaviour
{
    [SerializeField] float damage;
    public GameObject target;
    // Start is called before the first frame update
    private Stats stats;
    // Update is called once per frame
    private void OnEnable()
    {
        stats = target.GetComponent<Stats>();
        stats.TakeDamage(target, damage);
    }
    void Update()
    {
        if (target != null)
        {
            StartCoroutine(destroyAfterTime());
        }
    }
    IEnumerator destroyAfterTime()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
