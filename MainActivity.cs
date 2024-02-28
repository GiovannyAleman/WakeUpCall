using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Runtime;
using System.Collections.Generic;
using Java.Util;


namespace WakeUpCall
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        // UI elements:Calls the interactions of the app
        Button setAlarmButton;
        TimePicker timePicker;
        ListView alarmListView;

        // List to store alarm strings
        List<string> alarms;

        // Flag to track time format
        bool is24HourFormat = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Initialize UI elements
            setAlarmButton = FindViewById<Button>(Resource.Id.setAlarmButton);
            timePicker = FindViewById<TimePicker>(Resource.Id.timePicker);
            alarmListView = FindViewById<ListView>(Resource.Id.alarmListView);

            // Load saved alarms that users has made
            LoadAlarms();

            // Handle button click event
            setAlarmButton.Click += SetAlarmButton_Click;

            // Handle long click event on ListView item to delete alarm
            alarmListView.ItemLongClick += AlarmListView_ItemLongClick;

            // Add settings button click event
            Button settingsButton = FindViewById<Button>(Resource.Id.settingsButton);
            settingsButton.Click += SettingsButton_Click;
        }

        // Handle button click to set the alarm
        private void SetAlarmButton_Click(object sender, System.EventArgs e)
        {
            int hour = timePicker.Hour;
            int minute = timePicker.Minute;

            // Format time string with AM/PM indication
            string newAlarm = FormatTimeWithAmPm(hour, minute);
            alarms.Add(newAlarm);
            SaveAlarms();
            // Schedule the alarm
            ScheduleAlarm(hour, minute);
            // Update the ListView
           UpdateListView();

        }
        private void ScheduleAlarm(int hour, int minute)
        {
            AlarmManager alarmMgr = (AlarmManager)GetSystemService(AlarmService);
            Intent intent = new Intent(this, typeof(AlarmReceiver));
            // Use FLAG_IMMUTABLE for PendingIntents that do not need to change after they're created.
            // For mutable PendingIntents, replace PendingIntentFlags.Immutable with PendingIntentFlags.Mutable
            PendingIntent alarmIntent = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.Immutable);

            Calendar calendar = Calendar.Instance;
            calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
            calendar.Set(CalendarField.HourOfDay, hour);
            calendar.Set(CalendarField.Minute, minute);
            calendar.Set(CalendarField.Second, 0);

            alarmMgr.SetExact(AlarmType.RtcWakeup, calendar.TimeInMillis, alarmIntent);

        }

        // Handle long click event to delete alarm
        private void AlarmListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            int position = e.Position;
            alarms.RemoveAt(position);
            SaveAlarms();
            UpdateListView();
        }

        // Handle settings button click event
        private void SettingsButton_Click(object sender, System.EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Time Format Settings");
            builder.SetMessage("Select time format:");

            // Set 12-hour format
            builder.SetPositiveButton("12-hour", (s, args) =>
            {
                is24HourFormat = false;
                timePicker.SetIs24HourView(Java.Lang.Boolean.ValueOf(false));
            });

            // Set 24-hour format
            builder.SetNegativeButton("24-hour", (s, args) =>
            {
                is24HourFormat = true;
                timePicker.SetIs24HourView(Java.Lang.Boolean.ValueOf(true));
            });

            builder.Show();
        }

        // Save alarms to SharedPreferences
        private void SaveAlarms()
        {
            alarms.Sort(); // Sort alarms
            ISharedPreferences prefs = GetSharedPreferences("MyAlarms", FileCreationMode.Private);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutStringSet("Alarms", alarms);
            editor.Apply();
        }

        // Load saved alarms from SharedPreferences
        private void LoadAlarms()
        {
            ISharedPreferences prefs = GetSharedPreferences("MyAlarms", FileCreationMode.Private);
            alarms = new List<string>(prefs.GetStringSet("Alarms", new List<string>()));
            UpdateListView();
        }

        // Update the ListView with alarms
        private void UpdateListView()
        {
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, alarms);
            alarmListView.Adapter = adapter;
        }

        // Format time string with AM/PM indication
        private string FormatTimeWithAmPm(int hour, int minute)
        {
            string amOrPm = "AM";
            if (is24HourFormat) // Use 24-hour format
            {
                // No AM/PM for 24-hour format
                return $"{hour}:{minute:D2}";
            }
            else
            {
                // Determine whether it's AM or PM
                if (hour >= 12)
                {
                    amOrPm = "PM";
                    if (hour > 12)
                    {
                        hour -= 12;
                    }
                }
                else if (hour == 0)
                {
                    hour = 12;
                }
                return $"{hour}:{minute:D2} {amOrPm}";
            }
        }
    }
}
