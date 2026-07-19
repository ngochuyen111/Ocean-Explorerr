using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroClickCamera : MonoBehaviour
{
    [Header("Camera Points")]
    public Transform[] cameraPoints;

    [Header("Story Text")]
    public TextMeshProUGUI storyText;

    [TextArea(2, 4)]
    public string[] storyLines;

    [Header("Story Voice")]
    public AudioSource voiceSource;
    public AudioClip[] storyVoices;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Typing")]
    public float typingSpeed = 0.05f;

    [Header("Scene")]
    public string nextSceneName = "Level1";

    private int currentIndex = 0;
    private bool isMoving = false;
    private bool isTyping = false;

    private Vector3 targetPosition;

    private Coroutine typingCoroutine;

    public float fastMultiplier = 2f;
    private float currentSpeed;

    void Start()
    {
        if (cameraPoints != null && cameraPoints.Length > 0)
        {
            currentIndex = 0;

            transform.position =
                cameraPoints[0].position;

            targetPosition =
                cameraPoints[0].position;

            currentSpeed = moveSpeed;
        }

        ShowCurrentText();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Camera đang di chuyển:
            // click để đi ngay tới điểm tiếp theo
            if (isMoving)
            {
                transform.position =
                    targetPosition;

                isMoving = false;
                currentSpeed = moveSpeed;

                ShowCurrentText();
                return;
            }

            // Chữ đang chạy:
            // click để hiện toàn bộ câu ngay
            if (isTyping)
            {
                FinishTypingImmediately();
                return;
            }

            // Chữ đã hiện xong:
            // click để chuyển sang đoạn tiếp theo
            NextPoint();
        }

        if (isMoving)
        {
            transform.position =
                Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    currentSpeed * Time.deltaTime
                );

            if (Vector3.Distance(
                    transform.position,
                    targetPosition
                ) <= 0.01f)
            {
                transform.position =
                    targetPosition;

                isMoving = false;
                currentSpeed = moveSpeed;

                ShowCurrentText();
            }
        }
    }

    void NextPoint()
    {
        // Dừng giọng đọc của đoạn cũ
        StopCurrentVoice();

        currentIndex++;

        if (currentIndex >= cameraPoints.Length)
        {
            SceneManager.LoadScene(
                nextSceneName
            );

            return;
        }

        if (storyText != null)
        {
            storyText.text = "";
        }

        targetPosition =
            cameraPoints[currentIndex].position;

        isMoving = true;
    }

    void ShowCurrentText()
    {
        if (storyText == null)
            return;

        if (currentIndex < 0 ||
            currentIndex >= storyLines.Length)
            return;

        if (typingCoroutine != null)
        {
            StopCoroutine(
                typingCoroutine
            );
        }

        // Phát đúng giọng đọc theo index
        PlayCurrentVoice();

        typingCoroutine =
            StartCoroutine(
                TypeText(
                    storyLines[currentIndex]
                )
            );
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        storyText.text = "";

        foreach (char c in line)
        {
            storyText.text += c;

            yield return
                new WaitForSeconds(
                    typingSpeed
                );
        }

        isTyping = false;
        typingCoroutine = null;
    }

    void FinishTypingImmediately()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(
                typingCoroutine
            );

            typingCoroutine = null;
        }

        if (currentIndex >= 0 &&
            currentIndex < storyLines.Length)
        {
            storyText.text =
                storyLines[currentIndex];
        }

        isTyping = false;
    }

    void PlayCurrentVoice()
    {
        if (voiceSource == null)
        {
            Debug.LogWarning(
                "Intro chưa gắn Voice Source!"
            );

            return;
        }

        if (storyVoices == null ||
            currentIndex < 0 ||
            currentIndex >= storyVoices.Length)
        {
            Debug.LogWarning(
                "Story Voices không khớp với Story Lines!"
            );

            return;
        }

        AudioClip currentClip =
            storyVoices[currentIndex];

        if (currentClip == null)
        {
            Debug.LogWarning(
                "Story Voice ở Element " +
                currentIndex +
                " đang để None!"
            );

            return;
        }

        // Tránh hai đoạn giọng đọc chồng lên nhau
        voiceSource.Stop();

        voiceSource.clip =
            currentClip;

        voiceSource.Play();
    }

    void StopCurrentVoice()
    {
        if (voiceSource != null &&
            voiceSource.isPlaying)
        {
            voiceSource.Stop();
        }
    }

    public void SkipIntro()
    {
        StopCurrentVoice();

        SceneManager.LoadScene(
            nextSceneName
        );
    }

    void OnDisable()
    {
        StopCurrentVoice();
    }
}