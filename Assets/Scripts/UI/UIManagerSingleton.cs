using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerSingleton : MonoSingleton<UIManagerSingleton>
{
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Image timeTravelImage;
    [SerializeField] private Image healthbarImage;
    [SerializeField] private List<UIInventorySlot> uIInventorySlots;


    [SerializeField] public bool paused = false;
     
    Coroutine _flashRoutine = null;

    public override void Init()
    {
        base.Init();
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        paused = true;
        gamePanel.SetActive(false);
        pausePanel.SetActive(true);
    }
    public void UnpauseGame()
    {
        Time.timeScale = 1.0f;
        paused = false;
        gamePanel.SetActive(true);
        pausePanel.SetActive(false);
    }
    public void GameOver()
    {
        PauseGame();
        gameOverPanel.SetActive(true);
    }
    public void RetryGame()
    {
        UnpauseGame();
        LevelsManager.Instance.ResetGame();
    }

    public void ExitGame()
    {
        UnpauseGame();
        SceneManager.LoadScene("TitleScene");
    }

    public void SetHealthBar(float fillAmount)
    {
        healthbarImage.fillAmount = fillAmount;
    }

    public void UpdateItemSlotsUi(List<ItemList> itemLists)
    {
        for (int i = 0; i< itemLists.Count; i++)
        {
            uIInventorySlots[i].SetUILayout(itemLists[i]);
        }
    }

    public void TimeTravelFlashOnce(float secondForOneFlash)
    {
        if (_flashRoutine != null)
        {
            StopCoroutine(_flashRoutine);
        }
        _flashRoutine = StartCoroutine(TimeTravelFlashOnce(secondForOneFlash, 0, 1));
    }

    public void TimeTravelImageStart(float secondsForFullAplha)
    {
        if (_flashRoutine != null)
        {
            StopCoroutine(_flashRoutine);
        }
        _flashRoutine = StartCoroutine(TimeTravelStart(secondsForFullAplha));
    }

    public void TimeTravelImageStop(float secondsForEmptyAplha)
    {
        if (_flashRoutine != null)
        {
            StopCoroutine(_flashRoutine);
        }
        _flashRoutine = StartCoroutine(TimeTravelStop(secondsForEmptyAplha));
    }


    IEnumerator TimeTravelFlashOnce(float secondsForOneFlash, float minAlpha, float maxAlpha)
    {
        // half the flash time should be on flash in, the other half for flash out
        float flashInDuration = secondsForOneFlash / 2;
        float flashOutDuration = secondsForOneFlash / 2;


        for (float t = 0f; t <= flashInDuration; t += Time.deltaTime)
        {
            Color newColor = timeTravelImage.color;
            newColor.a = Mathf.Lerp(minAlpha, maxAlpha, t / flashInDuration);
            timeTravelImage.color = newColor;
            yield return null;
        }

        for (float t = 0f; t <= flashOutDuration; t += Time.deltaTime)
        {
            Color newColor = timeTravelImage.color;
            newColor.a = Mathf.Lerp(maxAlpha, minAlpha, t / flashOutDuration);
            if(newColor.a <= 0.1)
            {
                newColor.a = 0;
            }
            timeTravelImage.color = newColor;
            yield return null;
        }
    }
    IEnumerator TimeTravelStart (float secondsForFullAplha)
    {
        for (float t = 0f; t <= secondsForFullAplha; t += Time.deltaTime)
        {
            Color newColor = timeTravelImage.color;
            newColor.a = Mathf.Lerp(0, 1, t / secondsForFullAplha);
            if (newColor.a <= 0.1)
            {
                newColor.a = 0;
            }
            timeTravelImage.color = newColor;
            yield return null;
        }
    }

    IEnumerator TimeTravelStop(float secondsForEmptyAplha)
    {
        for (float t = 0f; t <= secondsForEmptyAplha; t += Time.deltaTime)
        {
            Color newColor = timeTravelImage.color;
            newColor.a = Mathf.Lerp(1, 0, t / secondsForEmptyAplha);
            if (newColor.a <= 0.1)
            {
                newColor.a = 0;
            }
            timeTravelImage.color = newColor;
            yield return null;
        }
    }
}