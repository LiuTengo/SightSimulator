using System;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    //PlayerSpawner
    [SerializeField]protected ControllerFactory ControllerFactory;
    [SerializeField]protected Transform SpawnPoint;
    //GameMode
    protected IGameMode CurGameMode;
    protected EVisionBlindMode CurVisionMode;
    
    #region PlayerFactory Function
    
    
    public ControllerFactory GetControllerFactory() 
    { return ControllerFactory; }
    
    #endregion

    #region GameMode Function

    public EVisionBlindMode GetVisionMode() {
        return CurVisionMode;
    }

    public void SetupGameMode(EVisionBlindMode VisionBlindMode)
    {
        if (CurGameMode != null)
        {
            if (CurVisionMode == VisionBlindMode) {return;}
            
            CurGameMode.OnGameModeDestroyed();
            CurGameMode = null;
            CurVisionMode = EVisionBlindMode.None;
        }
        CurGameMode = GenerateGameMode(VisionBlindMode);
    }

    public void ResetGameMode()
    {
        if (CurGameMode != null)
        {
            CurGameMode.OnGameModeDestroyed();
            CurGameMode = null;
        }    
    }
    
    private IGameMode GenerateGameMode(EVisionBlindMode VisionBlindMode)
    {
        CurVisionMode = VisionBlindMode;
        IGameMode GameModeProduct;
        switch (VisionBlindMode)
        {
            case EVisionBlindMode.Amblyopia:// 弱视
                GameModeProduct = new AmblyopiaGameMode();
                break;
            case EVisionBlindMode.TotalBlindHaveLight:// 全盲有光感
                GameModeProduct = new TotalBlindHaveLightGameMode();
                break;
            case EVisionBlindMode.TotalBlindNoLight:// 全盲无光感
                GameModeProduct = new TotalBlindNoLightGameMode();
                break;
            case EVisionBlindMode.TubularVision:// 管状视野
                GameModeProduct = new TubularVisionGameMode();
                break;
            case EVisionBlindMode.CentralVisionLoss:// 中心视野缺失
                GameModeProduct = new CentralVisionLossGameMode();
                break;
            default:
                return null;
        }
        
        GameModeProduct.OnGameModeCreated();
        GameModeProduct.GameModeAwake();
        GameModeProduct.GameModeStarted();
        
        return GameModeProduct;
    }
    
    #endregion

    #region MonoFunction

    private void Start()
    {
        ControllerFactory.CreatePlayerController(SpawnPoint);
    }

    #endregion
}
