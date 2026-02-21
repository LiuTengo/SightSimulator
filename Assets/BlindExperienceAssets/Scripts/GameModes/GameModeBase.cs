public class GameModeBase : IGameMode
{
    public PlayerController PlayerController;
    
    public virtual void OnGameModeCreated()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnGameModeDestroyed()
    {
        throw new System.NotImplementedException();
    }

    public virtual void GameModeAwake()
    {
        throw new System.NotImplementedException();
    }

    public virtual void GameModeStarted()
    {
        throw new System.NotImplementedException();
    }
}