using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI; // Image bileþeni için gerekli

public class DaySystem : MonoBehaviour
{
    [Header("UI Elemanlarý")]
    public TextMeshProUGUI dayText;
    public CanvasGroup dayTextCanvasGroup;
    public CanvasGroup blackScreenCanvasGroup; // Uyurken ekranýn kararmasý için

    [Header("Iþýk Ayarlarý")]
    public Light sunLight;
    public Gradient sunColorGradient;
    public AnimationCurve intensityCurve;

    [Header("Zaman Ayarlarý")]
    public float dayDurationInSeconds = 120f;
    [Range(0, 1)]
    public float timeOfDay = 0.25f; // 0.25 Sabah baþlangýcýdýr

    private int currentDay = 1;
    private bool isSleeping = false;

    void Start()
    {
        if (sunLight == null) sunLight = RenderSettings.sun;
        if (blackScreenCanvasGroup != null) blackScreenCanvasGroup.alpha = 0;

        UpdateDayUI();
        StartCoroutine(FadeInDayText());
    }

    void Update()
    {
        if (!isSleeping)
        {
            // Zamaný ilerlet
            timeOfDay += Time.deltaTime / dayDurationInSeconds;

            if (timeOfDay >= 1f)
            {
                timeOfDay = 0f;
                currentDay++;
                UpdateDayUI();
                StartCoroutine(FadeInDayText());
            }
        }

        UpdateLighting();
    }

    void UpdateLighting()
    {
        float t = timeOfDay;
        sunLight.color = sunColorGradient.Evaluate(t);
        sunLight.intensity = intensityCurve.Evaluate(t);

        // Güneþ rotasyonu
        float sunAngle = t * 360f - 90f;
        sunLight.transform.localRotation = Quaternion.Euler(sunAngle, 170f, 0f);

        // Atmosfer
        RenderSettings.fogColor = sunLight.color;
        RenderSettings.ambientLight = sunLight.color * 0.4f;
    }

    // UYUMA FONKSÝYONU - SleepArea'dan bu çaðrýlacak
    public void StartNextDay()
    {
        if (!isSleeping)
        {
            StartCoroutine(SleepTransition());
        }
    }

    IEnumerator SleepTransition()
    {
        isSleeping = true;

        // 1. Ekraný Karart
        float timer = 0;
        while (timer < 1f)
        {
            blackScreenCanvasGroup.alpha = Mathf.Lerp(0, 1, timer);
            timer += Time.deltaTime;
            yield return null;
        }

        // 2. Arka Planda Zamaný ve Günü Ayarla
        currentDay++;
        timeOfDay = 0.25f; // Sabah 06:00 gibi düþün
        UpdateDayUI();
        yield return new WaitForSeconds(1f); // Karanlýkta biraz bekle (horlama sesi eklenebilir :)

        // 3. Ekraný Tekrar Aç
        timer = 0;
        while (timer < 1f)
        {
            blackScreenCanvasGroup.alpha = Mathf.Lerp(1, 0, timer);
            timer += Time.deltaTime;
            yield return null;
        }

        isSleeping = false;
        StartCoroutine(FadeInDayText());
    }

    public void UpdateDayUI()
    {
        if (dayText != null) dayText.text = "DAY " + currentDay;
    }

    IEnumerator FadeInDayText()
    {
        dayTextCanvasGroup.alpha = 0;
        while (dayTextCanvasGroup.alpha < 1)
        {
            dayTextCanvasGroup.alpha += Time.deltaTime * 2f;
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        while (dayTextCanvasGroup.alpha > 0)
        {
            dayTextCanvasGroup.alpha -= Time.deltaTime * 1.5f;
            yield return null;
        }
    }
}