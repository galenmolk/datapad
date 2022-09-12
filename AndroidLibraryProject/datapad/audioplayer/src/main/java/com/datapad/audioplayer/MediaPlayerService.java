package com.datapad.audioplayer;

import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.os.Binder;
import android.os.IBinder;
import android.telephony.PhoneStateListener;
import android.telephony.TelephonyManager;
import android.util.Log;

import java.io.IOException;

public class MediaPlayerService extends Service implements MediaPlayer.OnCompletionListener, MediaPlayer.OnPreparedListener, MediaPlayer.OnErrorListener, MediaPlayer.OnSeekCompleteListener, MediaPlayer.OnInfoListener, MediaPlayer.OnBufferingUpdateListener, AudioManager.OnAudioFocusChangeListener
{
    public static String MEDIA_KEY = "media";

    // Binder given to clients.
    private final IBinder iBinder = new LocalBinder();

    private MediaPlayer mediaPlayer;
    private String mediaFile;
    private int resumePosition;
    private AudioManager audioManager;

    // Handle Incoming phone calls.
    private boolean ongoingCall = false;
    private PhoneStateListener phoneStateListener;
    private TelephonyManager telephonyManager;

    private BroadcastReceiver playNewAudio = new BroadcastReceiver()
    {
        @Override
        public void onReceive(Context context, Intent intent)
        {
            String newMediaFile = "";

            try
            {
                newMediaFile = intent.getExtras().getString(MEDIA_KEY);
            }
            catch (NullPointerException e)
            {
                stopSelf();
            }

            if (newMediaFile.equals(mediaFile) || newMediaFile.equals(""))
                return;

            // A PLAY_NEW_AUDIO action received
            // Reset mediaPlayer to play the new Audio.
            PlayAudio(newMediaFile);
        }
    };

    private void PlayAudio(String audioPath)
    {
        Log.d("USM Test", "???");
        UnityMessenger.SendMessage("PlayerHandler", "MessageTest", "Fuck You");
        mediaFile = audioPath;
        stopMedia();
        mediaPlayer.reset();
        initMediaPlayer();
    }

    private String GetNextTrack(String currentTrack)
    {
        AudioAsset[] library = AudioPlayer.AudioLibrary.Assets;
        int libraryLength = library.length;
        int currentTrackIndex = -1;

        for (int i = 0; i < libraryLength; i++)
        {
            if ((currentTrack.equals(library[i].LocalPath)))
            {
                currentTrackIndex = i;
                break;
            }
        }

        if (currentTrackIndex == -1)
            return null;

        if (currentTrackIndex < libraryLength - 1)
            return library[currentTrackIndex + 1].LocalPath;
        else
            return library[0].LocalPath;
    }

    private BroadcastReceiver stopAudio = new BroadcastReceiver()
    {
        @Override
        public void onReceive(Context context, Intent intent)
        {
            mediaFile = "";
            stopMedia();
            mediaPlayer.reset();
        }
    };

    private BroadcastReceiver becomingNoisyReceiver = new BroadcastReceiver()
    {
        @Override
        public void onReceive(Context context, Intent intent)
        {
            pauseMedia();
        }
    };

    @Override
    public void onCreate()
    {
        super.onCreate();

        // Manage incoming phone calls during playback.
        // Pause MediaPlayer on incoming call,
        // Resume on hangup.
        callStateListener();

        //ACTION_AUDIO_BECOMING_NOISY -- change in audio outputs.
        registerBecomingNoisyReceiver();

        register_playNewAudio();
        register_stopAudio();
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId)
    {
        try
        {
            mediaFile = intent.getExtras().getString(MEDIA_KEY);
        }
        catch (NullPointerException e)
        {
            stopSelf();
        }

        // Request audio focus.
        if (!requestAudioFocus())
            stopSelf();

        if (mediaFile != null && !mediaFile.equals(""))
            initMediaPlayer();

        return super.onStartCommand(intent, flags, startId);
    }

    @Override
    public void onDestroy()
    {
        super.onDestroy();

        if (mediaPlayer != null)
        {
            stopMedia();
            mediaPlayer.release();
        }

        removeAudioFocus();

        if (phoneStateListener != null)
        {
            telephonyManager.listen(phoneStateListener, PhoneStateListener.LISTEN_NONE);
        }

        unregisterReceiver(becomingNoisyReceiver);
        unregisterReceiver(playNewAudio);
        unregisterReceiver(stopAudio);
    }

    @Override
    public IBinder onBind(Intent intent)
    {
        return iBinder;
    }

    @Override
    public void onBufferingUpdate(MediaPlayer mediaPlayer, int percent) { }

    @Override
    public void onCompletion(MediaPlayer mediaPlayer)
    {
        Log.d("MediaPlayer Log", "onCompletion");
        PlayAudio(GetNextTrack(mediaFile));
//        stopMedia();
//        stopSelf();
    }

    @Override
    public boolean onError(MediaPlayer mediaPlayer, int i, int i1)
    {
        // Invoked when there has been an error during an asynchronous operation.
        switch (i)
        {
            case MediaPlayer.MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK:
                Log.d("MediaPlayer Error", "MEDIA ERROR NOT VALID FOR PROGRESSIVE PLAYBACK " + i1);
                break;
            case MediaPlayer.MEDIA_ERROR_SERVER_DIED:
                Log.d("MediaPlayer Error", "MEDIA ERROR SERVER DIED" + i1);
                break;
            case MediaPlayer.MEDIA_ERROR_UNKNOWN:
                Log.d("MediaPlayer Error", "MEDIA ERROR UNKNOWN" + i1);
                break;
        }

        return false;
    }

    @Override
    public boolean onInfo(MediaPlayer mediaPlayer, int i, int i1)
    {
        return false;
    }

    @Override
    public void onPrepared(MediaPlayer mediaPlayer)
    {
        playMedia();
    }

    @Override
    public void onSeekComplete(MediaPlayer mediaPlayer) { }

    @Override
    public void onAudioFocusChange(int focusChange)
    {
        // Invoked when the audio focus of the system is updated.
        switch (focusChange)
        {
            case AudioManager.AUDIOFOCUS_GAIN:
                // Resume Playback.
                if (mediaPlayer == null)
                    initMediaPlayer();
                else if (!mediaPlayer.isPlaying())
                    mediaPlayer.start();

                mediaPlayer.setVolume(1.0f, 1.0f);
                break;

            case AudioManager.AUDIOFOCUS_LOSS:
                // Lost focus for an unbounded amount of time: stop playback and release media player.
                if (mediaPlayer.isPlaying())
                    mediaPlayer.stop();

                mediaPlayer.release();
                mediaPlayer = null;
                break;
            case AudioManager.AUDIOFOCUS_LOSS_TRANSIENT:
                // Lost focus for a short time, but we have to stop playback.
                // We don't release the media player because playback is likely to resume.
                if (mediaPlayer.isPlaying())
                    mediaPlayer.pause();

                break;
            case AudioManager.AUDIOFOCUS_LOSS_TRANSIENT_CAN_DUCK:
                // Lost focus for a short time, but it's okay to keep playing
                // at an attenuated level.
                if (mediaPlayer.isPlaying())
                    mediaPlayer.setVolume(0.1f, 0.1f);

                break;
        }
    }

    private void register_playNewAudio()
    {
        IntentFilter filter = new IntentFilter(AudioPlayer.Broadcast_PLAY_NEW_AUDIO);
        registerReceiver(playNewAudio, filter);
    }

    private void register_stopAudio()
    {
        IntentFilter filter = new IntentFilter(AudioPlayer.Broadcast_STOP_AUDIO);
        registerReceiver(stopAudio, filter);
    }

    private void registerBecomingNoisyReceiver()
    {
        // Register after getting audio focus.
        IntentFilter intentFilter = new IntentFilter(AudioManager.ACTION_AUDIO_BECOMING_NOISY);
        registerReceiver(becomingNoisyReceiver, intentFilter);
    }

    private void callStateListener()
    {
        //Handle incoming phone calls.
        telephonyManager = (TelephonyManager) getSystemService(Context.TELEPHONY_SERVICE);
        //Starting listening for PhoneState changes
        phoneStateListener = new PhoneStateListener()
        {
            @Override
            public void onCallStateChanged(int state, String incomingNumber)
            {
                switch (state)
                {
                    case TelephonyManager.CALL_STATE_OFFHOOK:
                    case TelephonyManager.CALL_STATE_RINGING:
                        // If at least one call exists or the phone is ringing, pause the MediaPlayer.
                        if (mediaPlayer == null)
                            return;

                        pauseMedia();
                        ongoingCall = true;
                        break;
                    case TelephonyManager.CALL_STATE_IDLE:
                        // Phone idle. Start playing.
                        if (mediaPlayer == null)
                            return;

                        if (ongoingCall)
                        {
                            ongoingCall = false;
                            resumeMedia();
                        }
                        break;
                }
            }
        };

        // Register the listener with the telephony manager to listen for changes to the device call state.
        telephonyManager.listen(phoneStateListener, PhoneStateListener.LISTEN_CALL_STATE);
    }

    private boolean requestAudioFocus()
    {
        audioManager = (AudioManager) getSystemService(Context.AUDIO_SERVICE);
        int result = audioManager.requestAudioFocus(this, AudioManager.STREAM_MUSIC, AudioManager.AUDIOFOCUS_GAIN);

        // Return true if focus is gained.
        return result == AudioManager.AUDIOFOCUS_REQUEST_GRANTED;
    }

    private boolean removeAudioFocus()
    {
        return AudioManager.AUDIOFOCUS_REQUEST_GRANTED == audioManager.abandonAudioFocus(this);
    }

    public class LocalBinder extends Binder
    {
        public MediaPlayerService getService()
        {
            return MediaPlayerService.this;
        }
    }

    private void initMediaPlayer()
    {
        mediaPlayer = new MediaPlayer();

        // Set up MediaPlayer event listeners.
        mediaPlayer.setOnCompletionListener(this);
        mediaPlayer.setOnErrorListener(this);
        mediaPlayer.setOnPreparedListener(this);
        mediaPlayer.setOnBufferingUpdateListener(this);
        mediaPlayer.setOnSeekCompleteListener(this);
        mediaPlayer.setOnInfoListener(this);

        // Reset so that the MediaPlayer is not pointing to another data source.
        mediaPlayer.reset();

        mediaPlayer.setAudioStreamType(AudioManager.STREAM_MUSIC);

        try
        {
            mediaPlayer.setDataSource(mediaFile);
        }
        catch (IOException e)
        {
            e.printStackTrace();
            stopSelf();
        }

        mediaPlayer.prepareAsync();
    }

    private void playMedia()
    {
        if (!mediaPlayer.isPlaying())
            mediaPlayer.start();
    }

    private void stopMedia()
    {
        if (mediaPlayer == null)
            return;

        if (mediaPlayer.isPlaying())
            mediaPlayer.stop();
    }

    private void resumeMedia()
    {
        if (!mediaPlayer.isPlaying())
        {
            mediaPlayer.seekTo(resumePosition);
            mediaPlayer.start();
        }
    }

    private void pauseMedia()
    {
        if (mediaPlayer.isPlaying())
        {
            mediaPlayer.pause();
            resumePosition = mediaPlayer.getCurrentPosition();
        }
    }
}
