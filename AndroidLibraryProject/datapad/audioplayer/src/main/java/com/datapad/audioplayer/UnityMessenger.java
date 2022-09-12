package com.datapad.audioplayer;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class UnityMessenger extends UnityPlayerActivity {
    public static void SendMessage(String gameObjectName, String methodName, String msg)
    {
        UnityPlayer.UnitySendMessage(gameObjectName, methodName, msg);
    }

}
