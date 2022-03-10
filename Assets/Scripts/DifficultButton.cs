using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultButton : MonoBehaviour
{
    public int difficult;
    private Button button;
    private Game game;
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SetDifficult);
        var go = GameObject.Find("Grid");
        game = go.GetComponent<Game>();
    }
    void SetDifficult()
    {
        game.SetUpDifficult(difficult);
    }
}
