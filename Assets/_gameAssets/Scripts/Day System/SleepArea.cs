using UnityEngine;

public class SleepArea : MonoBehaviour
{
    private bool isInside = false;
    private DaySystem daySystem;
    public GameObject interactionUI;

    void Start()
    {
        // DaySystem'i sahnede bul
        daySystem = Object.FindAnyObjectByType<DaySystem>();
        if (interactionUI != null) interactionUI.SetActive(false);
    }

    void Update()
    {
        // Eðer içerdeyse ve E'ye basarsa
        if (isInside && Input.GetKeyDown(KeyCode.E))
        {
            if (daySystem != null)
            {
                daySystem.StartNextDay();
                // Etkileþim yazýsýný uykudayken kapat
                if (interactionUI != null) interactionUI.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInside = true;
            if (interactionUI != null) interactionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInside = false;
            if (interactionUI != null) interactionUI.SetActive(false);
        }
    }
}