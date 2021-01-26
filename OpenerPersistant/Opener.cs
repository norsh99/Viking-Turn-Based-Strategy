using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opener : MonoBehaviour
{

    [SerializeField] private List<GameObject> allScenes;
    private void Start()
    {
        
    }

    //OTHER
    private void GoToScreen(List<GameObject> theList, int screenNum)
    {
        for (int i = 0; i < theList.Count; i++)
        {
            if (i == screenNum)
            {
                theList[i].SetActive(true);
            }
            else
            {
                theList[i].SetActive(false);

            }
        }
    }



    //BUTTONS
    public void StartGameButton()
    {
        //GameManager.instance.LoadGame();
        GoToScreen(allScenes, 1);
    }

}
