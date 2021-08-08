using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalNowInStackTextChange : MonoBehaviour
{
    public void SignalNowInStackTextChange_()
    {
        AnimatedRewardClaims.GetInstance().NowInStackTextChanged();
    }
}
