using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;

[BroadcastReceiver(Enabled = true)]
public class AlarmReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
     

        // Intent to start the AlarmScreenActivity
        Intent alarmScreenIntent = new Intent(context, typeof(AlarmScreenActivity));

        // If the app is not running, the activity needs to be started as a new task
        alarmScreenIntent.AddFlags(ActivityFlags.NewTask);

        // Start the AlarmScreenActivity
        context.StartActivity(alarmScreenIntent);

        // If you're handling the MediaPlayer in the AlarmScreenActivity,
        // you don't need to start it here in the receiver.
    }
}
