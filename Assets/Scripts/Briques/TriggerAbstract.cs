using UnityEngine;

abstract public class TriggerAbstract : MonoBehaviour
{
    BoxCollider m_Collider;

    private void Awake()
    {
        m_Collider = GetComponent<BoxCollider>();
    }

    public void ActivateTrigger()
    {
        m_Collider.enabled = true;
    }

    protected void DesactivateTrigger()
    {
        m_Collider.enabled = false;
    }

    public bool IsActive()
    {
        return m_Collider.enabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Player"))
            TriggerEnter();
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.tag == "Player"))
            TriggerExit();
    }

    protected abstract void TriggerEnter();

    protected virtual void TriggerExit()
    {
        //Optionnal
    }
}
