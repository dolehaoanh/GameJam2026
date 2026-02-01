using UnityEngine;
using UnityEngine.UI; // Vẫn cần để dùng Image (Portrait)
using TMPro; // <--- THƯ VIỆN QUAN TRỌNG NHẤT
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    [TextArea(3, 10)]
    public string sentence;
    public Sprite portrait;
}

public class VisualNovelManager : MonoBehaviour
{
    public static VisualNovelManager Instance;

    [Header("--- UI COMPONENTS ---")]
    public GameObject dialoguePanel;
    public Image portraitImage;

    // --- THAY ĐỔI Ở ĐÂY ---
    // Thay vì Text (Legacy), ta dùng TextMeshProUGUI
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contentText;
    // ---------------------

    private Queue<DialogueLine> linesQueue = new Queue<DialogueLine>();
    private bool isTyping = false;

    public bool IsDialogueActive { get; private set; } = false;

    void Awake()
    {
        Instance = this;
        // Đảm bảo lúc đầu tắt panel đi
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public void StartConversation(DialogueLine[] lines)
    {
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);
        linesQueue.Clear();

        foreach (DialogueLine line in lines)
        {
            linesQueue.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (isTyping) return;

        if (linesQueue.Count == 0)
        {
            EndConversation();
            return;
        }

        DialogueLine currentLine = linesQueue.Dequeue();

        // Gán text cho TMP y hệt như Text thường
        nameText.text = currentLine.characterName;

        if (currentLine.portrait != null)
        {
            portraitImage.sprite = currentLine.portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine.sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        contentText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            contentText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
        isTyping = false;
    }

    void EndConversation()
    {
        IsDialogueActive = false;
        dialoguePanel.SetActive(false);
    }
}