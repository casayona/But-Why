using UnityEngine;
using Unity.Cinemachine; // Cinemachine 3 kütüphanesi

public class CameraZoom : MonoBehaviour
{
    private PenguinInputs inputActions;
    private CinemachineOrbitalFollow orbitalFollow;

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minRadiusScale = 0.5f;
    public float maxRadiusScale = 3f;

    private float currentZoomMultiplier = 1f;

    // Orijinal deðerleri saklamak için
    private float originalTopRadius;
    private float originalCenterRadius;
    private float originalBottomRadius;

    private void Awake()
    {
        inputActions = new PenguinInputs();
        orbitalFollow = GetComponent<CinemachineOrbitalFollow>();

        // DOÐRU YOL: Direkt .Orbits üzerinden ulaþýlýr
        originalTopRadius = orbitalFollow.Orbits.Top.Radius;
        originalCenterRadius = orbitalFollow.Orbits.Center.Radius;
        originalBottomRadius = orbitalFollow.Orbits.Bottom.Radius;
    }

    private void OnEnable() => inputActions.PenguinActions.Enable();
    private void OnDisable() => inputActions.PenguinActions.Disable();

    private void Update()
    {
        // ÖNEMLÝ: Eðer burada hala hata alýrsan, inputActions.PenguinActions.Zoom 
        // kýsmýndaki "Zoom" isminin Input Action penceresindekiyle AYNI olduðundan emin ol.
        // Ve "Save Asset" butonuna bastýðýndan emin ol.

        var zoomValue = inputActions.PenguinActions.Zoom.ReadValue<Vector2>();
        float scrollDelta = zoomValue.y;

        if (Mathf.Abs(scrollDelta) > 0.01f)
        {
            currentZoomMultiplier -= (scrollDelta / 120f) * zoomSpeed * Time.deltaTime;
            currentZoomMultiplier = Mathf.Clamp(currentZoomMultiplier, minRadiusScale, maxRadiusScale);

            // GÜNCELLEME: Orbits üzerinden deðerleri atýyoruz
            orbitalFollow.Orbits.Top.Radius = originalTopRadius * currentZoomMultiplier;
            orbitalFollow.Orbits.Center.Radius = originalCenterRadius * currentZoomMultiplier;
            orbitalFollow.Orbits.Bottom.Radius = originalBottomRadius * currentZoomMultiplier;
        }
    }
}