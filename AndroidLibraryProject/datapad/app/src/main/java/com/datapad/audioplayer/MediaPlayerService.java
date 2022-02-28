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

    // Path to the audio file.
    private String mediaFile;

    // Used to pause/resume MediaPlayer.
    private int resumePosition;

    private AudioManager audioManager;

    // Handle Incoming phone calls
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
                // An audio file is passed to the service through putExtra().
                newMediaFile = intent.getExtras().getString(MEDIA_KEY);
            }
            catch (NullPointerException e)
            {
                stopSelf();
            }

            if (newMediaFile.equals(mediaFile) || newMediaFile.equals(""))
                return;

            //A PLAY_NEW_AUDIO action received
            //reset mediaPlayer to play the new Audio
            mediaFile = newMediaFile;
            stopMedia();
            mediaPlayer.reset();
            initMediaPlayer();
            //buildNotification(PlaybackStatus.PLAYER);
        }
    };

    private void register_playNewAudio()
    {
        IntentFilter filter = new IntentFilter(AudioPlayer.Broadcast_PLAY_NEW_AUDIO);
        registerReceiver(playNewAudio, filter);
    }

    private void callStateListener()
    {
        //Handle incoming phone calls
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
                        //if at least one call exists or the phone is ringing
                        //pause the MediaPlayer
                        if (mediaPlayer != null)
                        {
                            pauseMedia();
                            ongoingCall = true;
                        }
                        break;
                    case TelephonyManager.CALL_STATE_IDLE:
                        // Phone idle. Start playing.
                        if (mediaPlayer != null)
                        {
                            if (ongoingCall)
                            {
                                ongoingCall = false;
                                resumeMedia();
                            }
                        }
                        break;
                }
            }
        };
        // Register the listener with the telephony manager
        // Listen for changes to the device call state.
        telephonyManager.listen(phoneStateListener, PhoneStateListener.LISTEN_CALL_STATE);
    }

    private BroadcastReceiver becomingNoisyReceiver = new BroadcastReceiver()
    {
        @Override
        public void onReceive(Context context, Intent intent)
        {
            //pause audio on ACTION_AUDIO_BECOMING_NOISY
            pauseMedia();
            //buildNotification(PlaybackStatus.PAUSED);
        }
    };

    private void registerBecomingNoisyReceiver()
    {
        //register after getting audio focus
        IntentFilter intentFilter = new IntentFilter(AudioManager.ACTION_AUDIO_BECOMING_NOISY);
        registerReceiver(becomingNoisyReceiver, intentFilter);
    }

    @Override
    public void onCreate()
    {
        super.onCreate();
        // Perform one-time setup procedures

        // Manage incoming phone calls during playback.
        // Pause MediaPlayer on incoming call,
        // Resume on hangup.
        callStateListener();

        //ACTION_AUDIO_BECOMING_NOISY -- change in audio outputs -- BroadcastReceiver
        registerBecomingNoisyReceiver();

        //Listen for new Audio to play -- BroadcastReceiver
        register_playNewAudio();
    }

    // The system calls this method when an activity requests the service be started.
    @Override
    public int onStartCommand(Intent intent, int flags, int startId)
    {
        try
        {
            // An audio file is passed to the service through putExtra().
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

        //removeNotification();

        unregisterReceiver(becomingNoisyReceiver);
        unregisterReceiver(playNewAudio);
    }

    @Override
    public IBinder onBind(Intent intent)
    {
        return iBinder;
    }

    @Override
    public void onBufferingUpdate(MediaPlayer mediaPlayer, int percent)
    {
        // Invoked indicating buffering status of
        // a media resource being streamed over the network.
    }

    @Override
    public void onCompletion(MediaPlayer mediaPlayer)
    {
        // Invoked when playback of a media source has completed.
        stopMedia();

        // Stop the service.
        stopSelf();
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
        // Invoked to communicate some info.
        return false;
    }

    @Override
    public void onPrepared(MediaPlayer mediaPlayer)
    {
        // Invoked when the media source is ready for playback.
        playMedia();
    }

    @Override
    public void onSeekComplete(MediaPlayer mediaPlayer)
    {
        // Invoked indicating the completion of a seek operation.
    }

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

        // Reset so that the MediaPlayer is not pointer to another data source.
        mediaPlayer.reset();

        mediaPlayer.setAudioStreamType(AudioManager.STREAM_MUSIC);

        try
        {
            // Set the data source to the mediaFile location.
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

    private void pauseMedia()
    {
        if (mediaPlayer.isPlaying())
        {
            mediaPlayer.pause();
            resumePosition = mediaPlayer.getCurrentPosition();
        }
    }

    private void resumeMedia()
    {
        if (!mediaPlayer.isPlaying())
        {
            mediaPlayer.seekTo(resumePosition);
            mediaPlayer.start();
        }
    }
}
