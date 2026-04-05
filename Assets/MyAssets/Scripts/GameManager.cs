using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject retryPanel, winPanel, startPanel, gamePanel;
    public Text levelText, sliderValueText, maxValueText;
    public Material selectedGroundMat, groundMat;
    public Slider slider;
    private int levelNo;
    public Color[] bgColors;
    public Material bgMat;
    public float incrementAmount; // Amount to increment the slider each time
    public float sliderIncreaseTime;
    private Coroutine smoothIncreaseCoroutine;
    private bool win;

    void Awake()
    {
        Application.targetFrameRate = 60;
        levelNo = PlayerPrefs.GetInt("Level", 1);

        SetBgColor();
        SetSliderMaxValue();

        levelText.text = "Level " + levelNo.ToString();
    }

    private void SetBgColor()
    {
        int colorIndex = (levelNo - 1) / 5; // Calculate the color index based on level number
        int maxColorIndex = bgColors.Length - 1; // Maximum color index

        // Ensure colorIndex wraps around when it exceeds the maximum color index
        colorIndex = colorIndex % (maxColorIndex + 1);

        bgMat.color = bgColors[colorIndex];
    }

    private void SetSliderMaxValue()
    {
        int maxSliderValue = 100 + (levelNo - 1) * 10;
        slider.maxValue = maxSliderValue;

        maxValueText.text = slider.maxValue.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Time.timeScale = 1;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.DeleteAll();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerPrefs.SetInt("Level", ++levelNo);
        }
    }

    public void Reload()
    {
        AudioManager.Instance.Play("Click");
        AdsManager.Instance.ShowInterstitialAd();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartGame()
    {
        AudioManager.Instance.Play("Click");
        startPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void Win()
    {
        if (!win)
        {
            win = true;
            Invoke("InvokeWinGame", 1);
        }
    }

    public void Retry()
    {
        if (!win)
        {
            Invoke("InvokeRetryGame", 1);
        }
    }

    private void InvokeRetryGame()
    {
        Debug.Log("Retry");
        AudioManager.Instance.Play("Retry");
        retryPanel.SetActive(true);
        retryPanel.transform.DOScale(new Vector3(1, 1, 1), .5f);
    }

    private void InvokeWinGame()
    {
        Debug.Log("Win");
        AudioManager.Instance.Play("Win");
        winPanel.SetActive(true);
        winPanel.transform.DOScale(new Vector3(1, 1, 1), .5f);
    }

    public void Next()
    {
        AudioManager.Instance.Play("Click");

        AdsManager.Instance.ShowInterstitialAd();

        PlayerPrefs.SetInt("Level", ++levelNo);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SoundToggle()
    {
        AudioManager.Instance.Play("Click");
        AudioManager.Instance.SoundToggle();
    }

    public void Home()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }

    public void OpenScene(int index)
    {
        AudioManager.Instance.Play("Click");
        SceneManager.LoadScene(index);
    }

    public void IncreaseLevelSlider()
    {
        // If there's a previous coroutine running, stop it
        if (smoothIncreaseCoroutine != null)
            StopCoroutine(smoothIncreaseCoroutine);

        // Determine the target value, ensuring it doesn't exceed the maximum value of 100
        float targetValue = Mathf.Min(slider.value + incrementAmount, slider.maxValue);

        // Start a new coroutine to smoothly increase the slider value
        smoothIncreaseCoroutine = StartCoroutine(SmoothIncreaseSlider(targetValue));
    }

    IEnumerator SmoothIncreaseSlider(float targetValue)
    {
        float timer = 0f;
        float startValue = slider.value;

        // Gradually increase the value of the slider over time
        while (timer < sliderIncreaseTime)
        {
            timer += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, targetValue, timer / sliderIncreaseTime);

            int value = (int)slider.value;
            sliderValueText.text = value.ToString();

            yield return null;
        }

        // Ensure the slider value reaches the exact target value
        slider.value = targetValue;

        if (slider.value == slider.maxValue)
              Win();

            // Reset the coroutine reference
            smoothIncreaseCoroutine = null;
    }
}
