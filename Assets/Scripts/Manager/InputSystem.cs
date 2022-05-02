using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public sealed class InputSystem : SingleSubject<InputSystem>
{
    private InputSystem () {}
    private List<Diamond> diamondSelected = new List<Diamond>();
    public List<Diamond> DiamondSelected => diamondSelected;

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (TimeController.IsPaused) return;

            if (GameManager.Instance.GameState != GameStates.OnWaitInput) return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray raySelection = Camera.main.ScreenPointToRay(Input.mousePosition);
                // RaycastHit hitSelection;
                Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hitSelection = Physics2D.Raycast(ray, Vector2.zero);
                if (hitSelection.collider != null)
                
                {
                    Diamond diamond;
                    if (!hitSelection.collider.TryGetComponent<Diamond>(out diamond)) return;
                    
                    Vector2Int position = new Vector2Int((int)diamond.transform.position.x, (int)diamond.transform.position.y);

                    diamondSelected.Add(diamond);

                    if (diamondSelected.Count == 2)
                    {
                        SendMessage(InputEvent.OnInvert, diamondSelected);
                        SendMessage (InputEvent.DeleteChoose);
                        diamondSelected.Clear();
                    }
                    else if (diamondSelected.Count == 1)
                    {
                        SendMessage(InputEvent.OnChoose, diamondSelected);
                    }
                }
                else
                {
                    if (diamondSelected.Count > 0)
                    {
                        SendMessage (InputEvent.DeleteChoose);
                        diamondSelected.Clear();
                    }
                }
            }
        }
        catch (System.Exception)
        {
            GameManager.Instance.SendMessage (GameStates.OnError);
        }
    }
}

public enum InputEvent
{
    OnChoose,
    DeleteChoose,
    OnInvert
}
