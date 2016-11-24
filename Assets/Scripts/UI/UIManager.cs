﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    LoginUIManager loginUIManager;
    WaitUIManager waitUIManager;
    BattleUIManager battleUIManager;

    public LoginUIManager LoginUIManager { get { return loginUIManager; } }
    public WaitUIManager WaitUIManager { get { return waitUIManager; } }
    public BattleUIManager BattleUIManager { get { return battleUIManager; } }
<<<<<<< HEAD

    public GameObject dialogPanel;
    public Text dialog;

    public void InitializeManager()
    {
        loginUIManager = new LoginUIManager();
        loginUIManager.ManagerInitialize();
        waitUIManager = new WaitUIManager();
        battleUIManager = new BattleUIManager();
        SetDialog();
    }

    public void SetDialog()
    {
        dialogPanel = GameObject.Find("DialogPanel");
        dialog = dialogPanel.transform.FindChild("Dialog").GetComponent<Text>();
        dialogPanel.SetActive(false);
    }

    public IEnumerator Dialog(float delay, string text)
    {
        dialogPanel.SetActive(true);

        dialog.text = text;

        yield return new WaitForSeconds(delay);

        dialog.text = "";

        dialogPanel.SetActive(false);
    }
}
=======

    void Start()
    {
        battleUIManager = new BattleUIManager();
        battleUIManager.SetUIObject();
    }

    void OnMouseOver()
    {
      

    }

    void OnMouseExit()
    {

    }
}
>>>>>>> 98dbc542fd4715da2799ac0dc5787834361477fa