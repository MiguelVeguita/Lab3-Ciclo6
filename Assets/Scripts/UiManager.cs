using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI; 

public class UIGameManager : NetworkBehaviour
{
    public TMP_InputField inputField;
    public Button submitButton;

    public GameObject LoginPanel;

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmitName);
        LoginPanel.SetActive(false);

       GameManager.Instance.OnConnection += () =>
        {
            LoginPanel.SetActive(true);
            inputField.text = "";
            submitButton.interactable = true;
            inputField.interactable = true;
        };
    }

    public void OnSubmitName()
    {
        string accountID = inputField.text;
        if (!string.IsNullOrEmpty(accountID))
        {
            GameManager.Instance.RegisterPlayerServerRpc(accountID, NetworkManager.Singleton.LocalClientId);
            submitButton.interactable = false;
            inputField.interactable = false;

            LoginPanel.SetActive(false);
        }
    }
}