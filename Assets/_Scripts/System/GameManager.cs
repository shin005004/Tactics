using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    public GameState State { get; private set; }

    void Start() => ChangeState(GameState.Starting);

    public void ChangeState(GameState newState)
    {
        OnBeforeStateChanged?.Invoke(newState);
        
        State = newState;
        switch(newState)
        {
            case GameState.Starting:
                HandleStarting();
                break;

            case GameState.Planning:

                break;
            
            case GameState.Playing:
                
                break;

            case GameState.End:

                break;
        }

        OnAfterStateChanged?.Invoke(newState);
        Debug.Log($"New State : {newState}");
    }

    private void HandleStarting()
    {
        ChangeState(GameState.Playing);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(State == GameState.Playing)
            {
                ChangeState(GameState.Planning);
            }
            else if(State == GameState.Planning)
            {
                ChangeState(GameState.Playing);
            }
        }
    }
}

public enum GameState
{
    Starting,
    Planning,
    Playing,
    End
}