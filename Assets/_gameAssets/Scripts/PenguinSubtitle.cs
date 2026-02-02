using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PenguinSoundController : MonoBehaviour
{
    [Header("UI Bileşenleri")]
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private GameObject subtitlePanel; // Alt yazı panelini açıp kapatmak için

    [Header("Hız Ayarları")]
    [SerializeField] private float typingSpeed = 0.05f;  // Yazma hızı
    [SerializeField] private float erasingSpeed = 0.02f; // Geriye doğru silme hızı

    [Header("Ses Ayarları")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> penguinSounds;

    [Header("Animasyon (Sallanma)")]
    [SerializeField] private float shakeAmount = 2.0f;
    [SerializeField] private float shakeSpeed = 10.0f;

    [Header("Senaryo Testi")]
    [TextArea(3, 10)]
    [SerializeField] private string[] testScenario = { 
        "Vak! Merhaba buzulların efendisi!", 
        "Geriye doğru silinme efektini gördün mü?", 
        "Bu sistem tam istediğin gibi çalışıyor!" 
    };

    private Queue<string> sentenceQueue = new Queue<string>();
    private Coroutine activeRoutine;
    private bool isTextAnimating = false;

    void Start()
    {
        // Başlangıç temizliği
        if (subtitleText != null) subtitleText.text = "";
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
    }

    void Update()
    {
        // TEST TUŞLARI
        // P tuşuna basınca senaryoyu baştan yükler ve başlatır
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartScenario(testScenario);
        }

        // Space veya Enter tuşuna basınca sıradaki cümleye geçer
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            DisplayNextSentence();
        }

        // Metin varsa sallanma efektini uygula
        if (isTextAnimating && subtitleText.text.Length > 0)
        {
            ApplyVertexAnimation();
        }
    }

    // --- SENARYO YÖNETİMİ ---

    public void StartScenario(string[] lines)
    {
        sentenceQueue.Clear();
        foreach (string line in lines)
        {
            sentenceQueue.Enqueue(line);
        }
        
        if (subtitlePanel != null) subtitlePanel.SetActive(true);
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentenceQueue.Count == 0 && subtitleText.text == "")
        {
            if (subtitlePanel != null) subtitlePanel.SetActive(false);
            return;
        }

        // Eğer hala bir cümle varsa onu al, yoksa boşluk (silme işlemi için)
        string nextSentence = sentenceQueue.Count > 0 ? sentenceQueue.Dequeue() : "";

        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(TransitionSequence(nextSentence));
    }

    // --- ANA ANİMASYON DÖNGÜSÜ (SİL VE YAZ) ---

    IEnumerator TransitionSequence(string newText)
    {
        isTextAnimating = true;

        // 1. ADIM: Mevcut metni harf harf geriye doğru sil
        string currentText = subtitleText.text;
        while (currentText.Length > 0)
        {
            currentText = currentText.Substring(0, currentText.Length - 1);
            subtitleText.text = currentText;

            // Silerken daha hızlı ve farklı bir ses (opsiyonel)
            PlayPenguinSound(1.2f); // Biraz daha tiz ses

            yield return new WaitForSeconds(erasingSpeed);
        }

        // Eğer yeni metin boşsa (senaryo bittiyse) paneli kapat ve çık
        if (string.IsNullOrEmpty(newText))
        {
            if (subtitlePanel != null) subtitlePanel.SetActive(false);
            isTextAnimating = false;
            yield break;
        }

        // 2. ADIM: Yeni metni harf harf yaz
        foreach (char letter in newText.ToCharArray())
        {
            subtitleText.text += letter;

            if (letter != ' ')
            {
                PlayPenguinSound(1.0f); // Normal ses
            }

            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void PlayPenguinSound(float pitch)
    {
        if (penguinSounds != null && penguinSounds.Count > 0 && audioSource != null)
        {
            audioSource.pitch = pitch + Random.Range(-0.1f, 0.1f);
            int index = Random.Range(0, penguinSounds.Count);
            audioSource.PlayOneShot(penguinSounds[index]);
        }
    }

    // --- VERTEX ANİMASYONU (SALLANMA) ---

    void ApplyVertexAnimation()
    {
        subtitleText.ForceMeshUpdate();
        TMP_TextInfo textInfo = subtitleText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                Vector3 orig = sourceVertices[vertexIndex + j];
                // Her harf için farklı bir zamanlama (orig.x) ile sallanma
                float yOffset = Mathf.Sin(Time.time * shakeSpeed + (orig.x * 0.05f)) * shakeAmount;
                sourceVertices[vertexIndex + j] = orig + new Vector3(0, yOffset, 0);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            subtitleText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}