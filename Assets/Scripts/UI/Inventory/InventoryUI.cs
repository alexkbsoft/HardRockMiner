using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryUI : MonoBehaviour
{
    public void OnDigClicked()
    {
        SceneManager.LoadScene("MapScreen");
    }
}
