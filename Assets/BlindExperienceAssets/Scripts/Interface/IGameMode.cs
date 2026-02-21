using UnityEngine;

public interface IGameMode
{
    public void OnGameModeCreated();
    public void OnGameModeDestroyed();
    public void GameModeAwake();
    public void GameModeStarted();
}
