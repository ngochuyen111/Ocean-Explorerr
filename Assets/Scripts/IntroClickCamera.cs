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
        if (cameraPoints.Length > 0)
        {
            currentIndex = 0;
            transform.position = cameraPoints[0].position;
            targetPosition = cameraPoints[0].position;
            currentSpeed = moveSpeed;
        }

        ShowCurrentText();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isMoving)
            {
                transform.position = targetPosition;
                isMoving = false;
                currentSpeed = moveSpeed;
                ShowCurrentText();
                return;
            }

            if (isTyping)
            {
                FinishTypingImmediately();
                return;
            }

            NextPoint();
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            currentSpeed * Time.deltaTime
        );

            if (Vector3.Distance(transform.position, targetPosition) <= 0.01f)
            {
                transform.position = targetPosition;

                isMoving = false;

                currentSpeed = moveSpeed;

                ShowCurrentText();
            }
        }
    }

    void NextPoint()
    {
        currentIndex++;

        if (currentIndex >= cameraPoints.Length)
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        if (storyText != null)
        {
            storyText.text = "";
        }

        targetPosition = cameraPoints[currentIndex].position;
        isMoving = true;
    }

    void ShowCurrentText()
    {
        if (storyText == null) return;
        if (currentIndex < 0 || currentIndex >= storyLines.Length) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(storyLines[currentIndex]));
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        storyText.text = "";

        foreach (char c in line)
        {
            storyText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void FinishTypingImmediately()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        storyText.text = storyLines[currentIndex];
        isTyping = false;
    }

    public void SkipIntro()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}