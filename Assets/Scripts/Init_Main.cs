using UnityEngine;
using CoffeeBean;

public class Init_Main : MonoBehaviour
{
    private void Start()
    {
        CMain.Init();

        CUI_Main.CreateUI();
    }
}

