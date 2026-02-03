using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    [Header("UI Elemanlarý")]
    public TextMeshProUGUI missionText;

    [Header("Görev Listesi")]
    public List<string> dailyMissions = new List<string>();

    private int currentMissionIndex = 0;
    private bool isProcessingMission = false;

    void Start()
    {
        UpdateMissionUI();
    }

    // Görevi tamamlamak için bu fonksiyonu çaðýrýyoruz
    public void CompleteMission()
    {
        if (!isProcessingMission && currentMissionIndex < dailyMissions.Count)
        {
            StartCoroutine(MissionTransitionRoutine());
        }
    }

    IEnumerator MissionTransitionRoutine()
    {
        isProcessingMission = true;

        // 1. Görevi "Tamamlandý" olarak iþaretle
        missionText.text = "MISSION COMPLETED!";
        missionText.color = Color.green;

        // 2. 3 saniye bekle (Oyuncu baþarýyý görsün)
        yield return new WaitForSeconds(3f);

        // 3. Bir sonraki göreve geç
        currentMissionIndex++;

        // 4. Eðer listede baþka görev varsa onu göster
        if (currentMissionIndex < dailyMissions.Count)
        {
            UpdateMissionUI();
        }
        else
        {
            missionText.text = "TÜM GÖREVLER BÝTTÝ!";
            missionText.color = Color.yellow;
        }

        isProcessingMission = false;
    }

    void UpdateMissionUI()
    {
        if (currentMissionIndex < dailyMissions.Count)
        {
            missionText.text = "NEW MISSION: " + dailyMissions[currentMissionIndex];
            missionText.color = Color.black;
        }
    }
}