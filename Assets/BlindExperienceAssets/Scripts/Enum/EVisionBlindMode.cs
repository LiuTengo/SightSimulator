using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EVisionBlindMode
{
    None,
    Amblyopia, // 弱视
    TotalBlindHaveLight, // 全盲有光感
    TotalBlindNoLight, // 全盲无光感
    TubularVision, // 管状视野
    CentralVisionLoss // 中心视野缺失
}
