using System.Collections;
using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI demonText;       // single centered text for ALL lines
    public TextMeshProUGUI achievementText;

    [Header("Demon Dialogue")]
    public float demonDuration = 3f;        // how long the wobble stays after typing
    public float typeSpeed = 0.03f;
    public float shakeAmount = 1.5f;
    public float shakeSpeed = 20f;

    [Header("Demon Voice")]
    public AudioSource demonAudioSource;
    public AudioClip[] demonSyllables;      // short growls/grunts/bleeps
    public int voiceEveryNChars = 2;        // play sound every N characters
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;

    [Header("Achievement Display")]
    public float achievmentDuration = 3f;

    private Coroutine demonRoutine;
    private Coroutine achievementRoutine;

    // --- MONEY ---
    public void SetMoney(int amount)
    {
        if (moneyText != null)
            moneyText.text = amount.ToString() + " YEN";
    }

    // --- "ERRORS" (alias to demon speech) ---
    public void ShowError(string message)
    {
        // errors are also spoken by the demon
        ShowDemonLine(message);
    }

    // --- DEMON DIALOGUE ---
    public void ShowDemonLine(string message)
    {
        if (string.IsNullOrEmpty(message) || demonText == null) return;

        if (demonRoutine != null)
            StopCoroutine(demonRoutine);

        // Prefix is static, already printed, color white
        string prefix = "<color=#FFFFFF>STALL DEMON: </color>";
        demonRoutine = StartCoroutine(DemonDialogueRoutine(prefix, message));
    }

    private IEnumerator DemonDialogueRoutine(string prefix, string body)
{
    demonText.enabled = true;
    
    demonText.text = prefix;
    demonText.ForceMeshUpdate();

    // --- typewriter + "Banjo" chatter---
    for (int i = 0; i < body.Length; i++)
    {
        demonText.text = prefix + body.Substring(0, i + 1);
        demonText.ForceMeshUpdate();

        if (demonAudioSource != null && demonSyllables != null && demonSyllables.Length > 0)
        {
            if (i % voiceEveryNChars == 0)
            {
                demonAudioSource.pitch = Random.Range(minPitch, maxPitch);
                var clip = demonSyllables[Random.Range(0, demonSyllables.Length)];
                demonAudioSource.PlayOneShot(clip);
            }
        }

        yield return new WaitForSeconds(typeSpeed);
    }

    // --- wobble / shake over the full text (prefix + body) ---
    float timer = 0f;
    TMP_TextInfo textInfo = demonText.textInfo;

    while (timer < demonDuration)
    {
        demonText.ForceMeshUpdate();
        textInfo = demonText.textInfo;

        for (int c = 0; c < textInfo.characterCount; c++)
        {
            if (!textInfo.characterInfo[c].isVisible) continue;

            int meshIndex = textInfo.characterInfo[c].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[c].vertexIndex;

            Vector3[] verts = textInfo.meshInfo[meshIndex].vertices;

            Vector3 shake = new Vector3(
                Mathf.Sin(Time.time * shakeSpeed + c) * shakeAmount,
                Mathf.Cos(Time.time * shakeSpeed + c) * shakeAmount,
                0f
            );

            verts[vertexIndex + 0] += shake;
            verts[vertexIndex + 1] += shake;
            verts[vertexIndex + 2] += shake;
            verts[vertexIndex + 3] += shake;

            textInfo.meshInfo[meshIndex].mesh.vertices = verts;
            demonText.UpdateGeometry(textInfo.meshInfo[meshIndex].mesh, meshIndex);
        }

        timer += Time.deltaTime;
        yield return null;
    }

    demonText.enabled = false;
}

    // --- ACHIEVEMENTS ---
    public void ShowAchievement(string achievementName)
    {
        if (achievementText == null || string.IsNullOrEmpty(achievementName)) return;

        if (achievementRoutine != null)
            StopCoroutine(achievementRoutine);

        achievementRoutine = StartCoroutine(AchievementRoutine(achievementName));
    }

    private IEnumerator AchievementRoutine(string text)
    {
        achievementText.text = text;
        achievementText.enabled = true;

        Transform t = achievementText.transform;
        Vector3 originalScale = t.localScale;
        t.localScale = originalScale * 1.1f;

        float tLerp = 0f;
        while (tLerp < 0.15f)
        {
            tLerp += Time.deltaTime;
            float k = tLerp / 0.15f;
            t.localScale = Vector3.Lerp(originalScale * 1.1f, originalScale, k);
            yield return null;
        }

        yield return new WaitForSeconds(achievmentDuration);

        achievementText.enabled = false;
    }
}