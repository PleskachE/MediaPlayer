using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace MediaPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer Timer;
        private ObservableCollection<KeyValuePair<string, string>> InternalPlayList;
        private int CounterElem;
        public MainWindow()
        {
            InitializeComponent();
            CounterElem = 0;
            Timer = new DispatcherTimer();
            InternalPlayList = new ObservableCollection<KeyValuePair<string, string>>();
            Playlist.ItemsSource = InternalPlayList;
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += Timer_tick;
        }
        private void Timer_tick(object sender, EventArgs e)
        {
            TimeSpan _time = Player.Position;
            Time.Text = String.Format("{0:00}:{1:00}:{2:00}", _time.Hours, _time.Minutes, _time.Seconds);
            Position.Value++;
        }
        private void PlayButt_Click(object sender, RoutedEventArgs e)
        {
            if (InternalPlayList.Count > 0)
            {
                Player.Play();
                Timer.Start();
            }
        }
        private void StopButt_Click(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
            Player.Stop();
            Position.Value = 0;
        }
        private void PauseButt_Click(object sender, RoutedEventArgs e)
        {
            Player.Pause();
            Timer.Stop();
        }
        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            Position.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
            Position.Value = 0;
            Timer.Start();
        }
        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
            CounterElem++;
            if (CounterElem >= InternalPlayList.Count)
                CounterElem = 0;
            Player.Source = new Uri(InternalPlayList[CounterElem].Value);
            Player.Play();
            Position.Value = 0;
            Timer.Start();
        }
        private void Position_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TimeSpan _time = new TimeSpan(0, 0, Convert.ToInt32(Math.Round(Position.Value)));
            Player.Position = _time;
        }
        private void PlaylistButt_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.Visibility == Visibility.Visible)
                Playlist.Visibility = Visibility.Hidden;
            else
                Playlist.Visibility = Visibility.Visible;
        }
        private void Playlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string _fileName = Path.GetFileNameWithoutExtension((sender as ListBox).SelectedItem.ToString());
            string _path = InternalPlayList.First(f => f.Key == _fileName).Value;
            CounterElem = InternalPlayList.IndexOf(new KeyValuePair<string, string>(_fileName, _path));
            Player.Source = new Uri(_path);
            Player.Play();
        }
        private void OpenFileButt_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _openFileDialog = new OpenFileDialog();
            string path;
            _openFileDialog.Filter = "Movies (*.mp4)|*.mp4";
            if (_openFileDialog.ShowDialog() == true)
                path = _openFileDialog.FileName;
            else
                return;
            InternalPlayList.Add(new KeyValuePair<string, string>(Path.GetFileNameWithoutExtension(path), path));
            if (InternalPlayList.Count == 1)
            {
                Player.Source = new Uri(path);
                Player.Play();
            }
        }
    }
}
