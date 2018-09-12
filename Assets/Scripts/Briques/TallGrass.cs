using System.Collections;
using UnityEngine;

public class TallGrass : MonoBehaviour, IDamageable
{
    private bool m_IsAlive = true;
    [SerializeField] Material CutSprites;
    BoxCollider m_Collider;
    [SerializeField] Vector3 sizeOfScale = new Vector3(0.1f, 0.1f, 0.0f);

    private void Start()
    {
        m_Collider = GetComponent<BoxCollider>();
    }

    public void TakeDamage(AttackStats attackStats)
    {
        if (!m_IsAlive)
            return;

        Death();
    }
    
    private void Death()
    {
        m_IsAlive = false;
        //GetComponent<SpriteRenderer>().sprite = CutSprites;
        GetComponent<MeshRenderer>().material = CutSprites;
        m_Collider.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!m_IsAlive)
            return;
        ICanBeSlowed entity = other.gameObject.GetComponent<ICanBeSlowed>();

        if (entity != null)
            entity.TryToSlow();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            transform.localScale = transform.localScale + sizeOfScale;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            transform.localScale = transform.localScale - sizeOfScale;
        }
    }

}
