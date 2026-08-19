using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotMachine : MonoBehaviour
{
    [Header("Reel Images")]
    public Image reel1;
    public Image reel2;
    public Image reel3;

    [Header("UI")]
    public TMP_Text resultText;
    public TMP_Text creditsText;
    public Button spinButton;

    [Header("Symbols")]
    public Sprite[] symbols;

    [Header("Settings")]
    public float spinTime = 2f;
    public float changeSpeed = 0.06f;

    [Header("Credits")]
    public int credits = 100;
    public int spinCost = 10;

    [Header("Payouts")]
    public int cherryPayout = 25;
    public int bellPayout = 50;
    public int barPayout = 75;
    public int jackpotPayout = 100;

    [Header("Win Effect")]
    public float winMessageTime = 1.5f;

    [Header("Sound Effects")]
    public AudioSource audioSource;

    public AudioClip spinSound;
    public AudioClip reelStopSound;
    public AudioClip winSound;
    public AudioClip jackpotSound;

    private bool isSpinning = false;

    // Store the actual symbol index
    private int reel1Index;
    private int reel2Index;
    private int reel3Index;


    // =========================
    // START
    // =========================

    void Start()
    {
        UpdateCredits();

        if (resultText != null)
        {
            resultText.text = "READY!";
        }

        if (spinButton != null)
        {
            spinButton.interactable = true;
        }
    }


    // =========================
    // SPIN BUTTON
    // =========================

    public void Spin()
    {
       
        if (isSpinning)
            return;

        // Check credits
        if (credits < spinCost)
        {
            if (resultText != null)
            {
                resultText.text = "NOT ENOUGH CREDITS!";
            }

            return;
        }

        // Check symbols
        if (symbols == null || symbols.Length < 4)
        {
            Debug.LogError(
                "You need at least 4 symbols in the Symbols array!"
            );

            return;
        }

        // Pay for the spin
        credits -= spinCost;

        UpdateCredits();

        // Show spinning message
        if (resultText != null)
        {
            resultText.text = "SPINNING...";
        }

        // Disable button
        if (spinButton != null)
        {
            spinButton.interactable = false;
        }

        // Play spinning sound
        if (audioSource != null && spinSound != null)
        {
            audioSource.clip = spinSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        StartCoroutine(SpinReels());
    }


    // =========================
    // UPDATE CREDITS
    // =========================

    void UpdateCredits()
    {
        if (creditsText != null)
        {
            creditsText.text = "CREDITS: " + credits;
        }
    }


    // =========================
    // SPIN ALL REELS
    // =========================

    IEnumerator SpinReels()
    {
        isSpinning = true;

        bool reel1Finished = false;
        bool reel2Finished = false;
        bool reel3Finished = false;


        // Reel 1
        StartCoroutine(
            SpinSingleReel(
                reel1,
                spinTime,
                result => reel1Index = result,
                () => reel1Finished = true
            )
        );


        // Reel 2
        StartCoroutine(
            SpinSingleReel(
                reel2,
                spinTime + 0.4f,
                result => reel2Index = result,
                () => reel2Finished = true
            )
        );


        // Reel 3
        StartCoroutine(
            SpinSingleReel(
                reel3,
                spinTime + 0.8f,
                result => reel3Index = result,
                () => reel3Finished = true
            )
        );


        // Wait until all reels finish
        while (
            !reel1Finished ||
            !reel2Finished ||
            !reel3Finished
        )
        {
            yield return null;
        }


        // Stop spinning sound
        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }


        // Check result
        CheckWin();


        // Finished spinning
        isSpinning = false;


        // Enable button again
        if (spinButton != null)
        {
            spinButton.interactable = true;
        }
    }


    // =========================
    // SPIN ONE REEL
    // =========================

    IEnumerator SpinSingleReel(
     Image reel,
     float duration,
     System.Action<int> saveResult,
     System.Action onFinished
 )
    {
        RectTransform rt = reel.GetComponent<RectTransform>();

        Vector2 originalPosition = rt.anchoredPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Pick a random symbol
            int randomIndex = Random.Range(0, symbols.Length);
            reel.sprite = symbols[randomIndex];

            // Move the symbol down
            float moveTime = 0.08f;
            float timer = 0f;

            while (timer < moveTime)
            {
                timer += Time.deltaTime;

                float t = timer / moveTime;

                
                rt.anchoredPosition =
                    originalPosition + Vector2.down * Mathf.Lerp(0, 70f, t);

                yield return null;
            }

           
            rt.anchoredPosition =
                originalPosition + Vector2.up * 70f;

            // Smoothly move back to center
            timer = 0f;

            while (timer < moveTime)
            {
                timer += Time.deltaTime;

                float t = timer / moveTime;

                rt.anchoredPosition =
                    Vector2.Lerp(
                        originalPosition + Vector2.up * 70f,
                        originalPosition,
                        t
                    );

                yield return null;
            }

            elapsed += moveTime;
        }

        // Final symbol
        int finalIndex = Random.Range(0, symbols.Length);

        reel.sprite = symbols[finalIndex];

        
        rt.anchoredPosition = originalPosition;

       
        saveResult(finalIndex);

       
        onFinished();
    }


    // =========================
    // RANDOM SYMBOL
    // =========================

    Sprite GetRandomSymbol()
    {
        return symbols[
            Random.Range(
                0,
                symbols.Length
            )
        ];
    }


    // =========================
    // CHECK WIN
    // =========================

    void CheckWin()
    {
        Debug.Log(
            "Final Result: " +
            reel1Index +
            " | " +
            reel2Index +
            " | " +
            reel3Index
        );


        // =========================
        // JACKPOT - 777
        // =========================

        if (
            reel1Index == 0 &&
            reel2Index == 0 &&
            reel3Index == 0
        )
        {
            credits += jackpotPayout;

            UpdateCredits();


            if (
                audioSource != null &&
                jackpotSound != null
            )
            {
                audioSource.PlayOneShot(
                    jackpotSound
                );
            }


            StartCoroutine(
                ShowWinMessage(
                    "JACKPOT! 777! +" +
                    jackpotPayout
                )
            );


            Debug.Log(
                "JACKPOT! 777!"
            );


            return;
        }


        // =========================
        // THREE CHERRIES
        // =========================

        if (
            reel1Index == 1 &&
            reel2Index == 1 &&
            reel3Index == 1
        )
        {
            credits += cherryPayout;

            UpdateCredits();


            if (
                audioSource != null &&
                winSound != null
            )
            {
                audioSource.PlayOneShot(
                    winSound
                );
            }


            StartCoroutine(
                ShowWinMessage(
                    "THREE CHERRIES! +" +
                    cherryPayout
                )
            );


            Debug.Log(
                "THREE CHERRIES!"
            );


            return;
        }


        // =========================
        // THREE BELLS
        // =========================

        if (
            reel1Index == 2 &&
            reel2Index == 2 &&
            reel3Index == 2
        )
        {
            credits += bellPayout;

            UpdateCredits();


            if (
                audioSource != null &&
                winSound != null
            )
            {
                audioSource.PlayOneShot(
                    winSound
                );
            }


            StartCoroutine(
                ShowWinMessage(
                    "THREE BELLS! +" +
                    bellPayout
                )
            );


            Debug.Log(
                "THREE BELLS!"
            );


            return;
        }


        // =========================
        // THREE BAR
        // =========================

        if (
            reel1Index == 3 &&
            reel2Index == 3 &&
            reel3Index == 3
        )
        {
            credits += barPayout;

            UpdateCredits();


            if (
                audioSource != null &&
                winSound != null
            )
            {
                audioSource.PlayOneShot(
                    winSound
                );
            }


            StartCoroutine(
                ShowWinMessage(
                    "BAR BAR BAR! +" +
                    barPayout
                )
            );


            Debug.Log(
                "BAR BAR BAR!"
            );


            return;
        }


        // =========================
        // NO WIN
        // =========================

        if (resultText != null)
        {
            resultText.text = "TRY AGAIN!";
        }


        Debug.Log(
            "Try Again!"
        );
    }


    // =========================
    // WIN MESSAGE EFFECT
    // =========================

    IEnumerator ShowWinMessage(
        string message
    )
    {
        if (resultText == null)
            yield break;


        resultText.text = message;


        // Store original scale
        Vector3 originalScale =
            resultText.transform.localScale;


        // Make message bigger
        resultText.transform.localScale =
            originalScale * 1.3f;


        // Wait
        yield return new WaitForSeconds(
            winMessageTime
        );


        // Return to original size
        resultText.transform.localScale =
            originalScale;
    }
}