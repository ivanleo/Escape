
using CoffeeBean;
using UnityEngine;

public class Init_Logo: MonoBehaviour
{
    private void Start()
    {
        CMain.Init();
        CUI_LOGO.CreateUI();
    }
}

