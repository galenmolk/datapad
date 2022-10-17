package com.datapad.audioplayer;

import android.app.Activity;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.Bundle;
import android.os.IBinder;

public class AudioPlayer extends Activity
{
    public static final String Broadcast_PLAY_NEW_AUDIO = "com.datapad.audioplayer.PlayNewAudio";
    public static final String Broadcast_STOP_AUDIO = "com.datapad.audioplayer.StopAudio";
    private final String SERVICE_STATE_KEY = "ServiceState";

    private static Activity unityActivity;
    private static MediaPlayerService player;
    private static boolean serviceBound = false;

    private static final ServiceConnection serviceConnection = new ServiceConnection()
    {
        @Override
        public void onServiceConnected(ComponentName componentName, IBinder service)
        {
            // We've bound to LocalService, cast the IBinder and get LocalService instance.
            MediaPlayerService.LocalBinder binder = (MediaPlayerService.LocalBinder) service;
            player = binder.getService();
            serviceBound = true;
        }

        @Override
        public void onServiceDisconnected(ComponentName componentName)
        {
            serviceBound = false;
        }
    };

    @Override
    protected void onDestroy()
    {
        super.onDestroy();

        if (serviceBound)
        {
            unbindService(serviceConnection);
            player.stopSelf();
        }
    }

    public static void setActivityInstance(Activity _unityActivity)
    {
        unityActivity = _unityActivity;
    }

    @Override
    public void onSaveInstanceState(Bundle savedInstanceState)
    {
        savedInstanceState.putBoolean(SERVICE_STATE_KEY, serviceBound);
        super.onSaveInstanceState(savedInstanceState);
    }

    @Override
    public void onRestoreInstanceState(Bundle savedInstanceState)
    {
        super.onRestoreInstanceState(savedInstanceState);
        serviceBound = savedInstanceState.getBoolean(SERVICE_STATE_KEY);
    }

    public static void StopAudio()
    {
        if (!serviceBound)
            return;

        Intent broadcastIntent = new Intent(Broadcast_STOP_AUDIO);
        unityActivity.sendBroadcast(broadcastIntent);
    }

    public static void PlayAudio(String path)
    {
        if (!serviceBound)
        {
            startMediaPlayerService(path);
            return;
        }

        broadcastPlayNewAudio(path);
    }

    private static void startMediaPlayerService(String path)
    {
        Intent playerIntent = new Intent(unityActivity, MediaPlayerService.class);
        playerIntent.putExtra(MediaPlayerService.MEDIA_KEY, path);
        unityActivity.startService(playerIntent);
        unityActivity.bindService(playerIntent, serviceConnection, Context.BIND_AUTO_CREATE);
    }

    private static void broadcastPlayNewAudio(String path)
    {
        Intent broadcastIntent = new Intent(Broadcast_PLAY_NEW_AUDIO);
        broadcastIntent.putExtra(MediaPlayerService.MEDIA_KEY, path);
        unityActivity.sendBroadcast(broadcastIntent);
    }
}
