using UnityEngine;
using CoffeeBean;

public class Init_Game : MonoBehaviour
{
    private void Awake()
    {
        GameObject.Find ( "SampleGrid" ).SetActive ( false );
    }

    private void Start()
    {
        CMain.Init();

        CGame.Instance.StartGame();

        CGame.Instance.StartGridClose();
    }
}

