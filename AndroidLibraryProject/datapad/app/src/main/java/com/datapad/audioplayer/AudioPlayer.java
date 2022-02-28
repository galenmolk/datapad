package com.datapad.audioplayer;

import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.Bundle;
import android.os.IBinder;

import androidx.appcompat.app.AppCompatActivity;

public class AudioPlayer extends AppCompatActivity
{
    public static final String Broadcast_PLAY_NEW_AUDIO = "com.datapad.audioplayer.PlayNewAudio";

    private final String SERVICE_STATE_KEY = "ServiceState";

    //private Activity unityActivity;

    private MediaPlayerService player;
    private boolean serviceBound = false;

    private final ServiceConnection serviceConnection = new ServiceConnection()
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

//    // Called From C# to get the Activity Instance
//    public void setActivityInstance(Activity _unityActivity)
//    {
//        unityActivity = _unityActivity;
//    }

    public void PlayAudio(String path)
    {
        if (!serviceBound)
        {
            Intent playerIntent = new Intent(this, MediaPlayerService.class);
            playerIntent.putExtra(MediaPlayerService.MEDIA_KEY, path);
            startService(playerIntent);
            bindService(playerIntent, serviceConnection, Context.BIND_AUTO_CREATE);
        }
        else
        {
            //Service is active
            //Send media with BroadcastReceiver
            Intent broadcastIntent = new Intent(Broadcast_PLAY_NEW_AUDIO);
            broadcastIntent.putExtra(MediaPlayerService.MEDIA_KEY, path);
            sendBroadcast(broadcastIntent);
        }
    }
}
