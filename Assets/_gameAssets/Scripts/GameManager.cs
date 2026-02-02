using UnityEngine;
using UnityEngine.SceneManagement; // Sahne iþlemleri için

public class GameManager : MonoBehaviour
{
    // Singleton Yapýsý: Diðer scriptlerden GameManager.Instance diyerek buna ulaþabilirsin.
    public static GameManager Instance { get; private set; }

    [Header("Cursor Settings")]
    public bool isCursorLocked = true;

    private void Awake()
    {
        // Singleton kontrolü
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahneler deðiþse bile GameManager silinmez
        }
    }

    private void Start()
    {
        SetCursorState(isCursorLocked);
    }

    private void Update()
    {
        // ESC tuþuna basýnca mouse imlecini serbest býrak (Test yaparken lazým olur)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursor();
        }
    }

    public void SetCursorState(bool lockCursor)
    {
        isCursorLocked = lockCursor;
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }

    public void ToggleCursor()
    {
        SetCursorState(!isCursorLocked);
    }

    // Ýleride lazým olur: Oyunu yeniden baþlatma fonksiyonu
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}