using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    public GameObject loadingScreen;
    public ProgressBar progressBar;
    private float totalSceneProgress;
    public TextMeshProUGUI loadingText;


    public static GameManager instance;


    public List<Ship> pShip1;
    public List<Ship> pShip2;


    private void Awake()
    {
        instance = this;


        SceneManager.LoadSceneAsync((int)SceneIndexes.OPENER, LoadSceneMode.Additive);
    }

    public void LoadGame(List<Ship> p1, List<Ship> p2)
    {
        pShip1 = p1;
        pShip2 = p2;

        loadingScreen.SetActive(true);



        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.OPENER));

        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.GAME, LoadSceneMode.Additive));
        StartCoroutine(GetSceneLoadProgress());
        StartCoroutine(GetTotalProgress());

    }

    public IEnumerator GetSceneLoadProgress()
    {
        for (int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
                totalSceneProgress = 0;

                foreach (AsyncOperation operation in scenesLoading)
                {
                    totalSceneProgress += operation.progress;
                }
                totalSceneProgress = (totalSceneProgress / scenesLoading.Count) * 100f;


                yield return null;    
            }
        }
    }


    public IEnumerator GetTotalProgress()
    {
        float totalProgress = 0;
        while (GameMaster.current == null || !GameMaster.current.setupComplete)
        {
            if (GameMaster.current == null)
            {

            }
            else
            {
                loadingText.text = GameMaster.current.loadingStatus.ToString();

            }

            totalProgress = Mathf.Round(totalSceneProgress / 2f);

            progressBar.currentPercent = totalProgress;

            yield return null;

        }

        if (GameMaster.current.setupComplete)
        {
            progressBar.currentPercent = progressBar.maxValue;
            loadingText.text = GameMaster.current.loadingStatus.ToString();

        }

        yield return new WaitForSeconds(1f);
        SceneManager.SetActiveScene(SceneManager.GetSceneAt((int)SceneIndexes.GAME - 1));
        loadingScreen.SetActive(false);
    }
}

public enum SceneIndexes
{
    MANAGER = 0,
    OPENER = 1,
    GAME = 2
}