
using WakeUpCall;
using Android.Content;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Media;

[Activity(Label = "Alarm")]
public class AlarmScreenActivity : Activity
{
    MediaPlayer mediaPlayer;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "alarm_screen" layout resource
        SetContentView(Resource.Layout.alarm_screen);


        mediaPlayer = MediaPlayer.Create(this, 2131558400);//This is the ID# for the iphone alarm found in Resource.designer.cs
        mediaPlayer.Start();
        mediaPlayer.Looping = true; // Keep looping the sound until dismissed

        // Get a reference to the "Dismiss" button
        Button stopAlarmButton = FindViewById<Button>(Resource.Id.stopAlarmButton);
      //  Button snoozeButton = FindViewById<Button>(Resource.Id.snooze_button); // Adding the snooze function 2/27 ----------------

        // Set up the button click event
        stopAlarmButton.Click += (sender, e) =>
        {
            // Stop the alarm sound
            mediaPlayer.Stop();
            mediaPlayer.Release();
            mediaPlayer = null;

            // Close the activity
            Finish();
        };
    }
}