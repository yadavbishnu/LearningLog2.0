// Author:  Kyle Chapman
// Created: October 31, 2024
// Updated: November 12, 2024
// Updated by: Bishnu Yadav
//            : November 13, 2024
// Description:
// Form code for a form that allows a user to make an audio recording
// as a log entry. Then it does nothing with the log entry, so that's
// cool.

using System.Windows;
using System.IO;
using System.Media;

namespace LearningLog2024
{
    /// <summary>
    /// Interaction logic for EntryWindow.xaml
    /// </summary>
    public partial class EntryWindow : Window
    {
        bool isRecording = false;
        FileInfo recordingFile;

        /// <summary>
        /// Constructor for the window.
        /// </summary>
        public EntryWindow()
        {
            InitializeComponent();
            // Populate the ComboBoxes.
            for (int counter = 1; counter <= 5; counter++)
            {
                comboQuality.Items.Add(counter);
                comboWellness.Items.Add(counter);
            }
            ResetForm();

            // Bind the ListView to the static LogEntry entries list.
            listViewEntries.ItemsSource = LogEntry.Entries;
        }

        /// <summary>
        /// If not recording, start a recording and allow the user to end the recording.
        /// If recording, end the recording and allow the user to save and delete the recording.
        /// </summary>
        private void RecordClick(object sender, RoutedEventArgs e)
        {
            if (!isRecording)
            {
                labelRecordText.Content = "Stop";
                isRecording = true;
                RecordWav.StartRecording();
                buttonSave.IsEnabled = false;
                buttonPlay.IsEnabled = false;
                UpdateStatus("Recording started.");
            }
            else
            {
                labelRecordText.Content = "_Record";
                isRecording = false;
                recordingFile = RecordWav.EndRecording();
                buttonSave.IsEnabled = true;
                buttonPlay.IsEnabled = true;
                buttonDelete.IsEnabled = true;
                UpdateStatus("Recording completed and saved to " + recordingFile.FullName);
            }
        }

        /// <summary>
        /// Save an entry; creates a LogEntry object.
        /// </summary>
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tabController.SelectedItem == tabEntry)
                {
                    // Create an AudioLogEntry
                    var newEntry = new AudioLogEntry(
                        (int)comboWellness.SelectedItem,
                        (int)comboQuality.SelectedItem,
                        textNotes.Text,
                        recordingFile
                    );
                    UpdateStatus("Audio entry saved: " + newEntry.GetDetails());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving audio entry: {ex.Message}", "Error");
            }

            ResetForm();
        }

        /// <summary>
        /// Write a message to the status bar.
        /// </summary>
        /// <param name="status">Message to write to the status bar</param>
        private void UpdateStatus(string status)
        {
            statusState.Content = DateTime.Now.ToString("MM/dd/yy H:mm:ss") + ": " + status;
        }

        /// <summary>
        /// Play button attempts to play the file.
        /// </summary>
        private void PlayClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(recordingFile.FullName))
                {
                    var player = new SoundPlayer(recordingFile.FullName);
                    player.Play();
                    UpdateStatus("Playing " + recordingFile.FullName);
                }
                else
                {
                    throw new FileNotFoundException("The file does not exist.", recordingFile.FullName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing file: {ex.Message}", "Error");
            }
        }

        /// <summary>
        /// When the tab changes, update the status bar.
        /// </summary>
        private void TabChanged(object sender, RoutedEventArgs e)
        {
            if (tabController.SelectedItem == tabSummary)
            {
                // Update the summary tab fields.
                textNumberOfEntries.Text = LogEntry.Count.ToString();
                textFirstEntry.Text = LogEntry.FirstEntry.ToString();
                textNewestEntry.Text = LogEntry.NewestEntry.ToString();

                UpdateStatus("Viewing summary");
            }
        }

        /// <summary>
        /// Reset the form to its default state.
        /// </summary>
        private void ResetForm()
        {
            comboQuality.SelectedIndex = 2;
            comboWellness.SelectedIndex = 2;
            labelRecordText.Content = "_Record";
            isRecording = false;
            buttonSave.IsEnabled = false;
            buttonPlay.IsEnabled = false;
            buttonDelete.IsEnabled = false;
            textNotes.Text = String.Empty;
        }

        /// <summary>
        /// Clear the current saved file.
        /// </summary>
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(recordingFile.FullName))
                {
                    File.Delete(recordingFile.FullName);
                    UpdateStatus("Deleted " + recordingFile.FullName);
                }
                else
                {
                    throw new FileNotFoundException("The file does not exist.", recordingFile.FullName);
                }

                buttonSave.IsEnabled = false;
                buttonPlay.IsEnabled = false;
                buttonDelete.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting file: {ex.Message}", "Error");
            }
        }

        private void SaveTextClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tabController.SelectedItem == tabTextEntry)
                {
                    // Create a TextLogEntry
                    var newTextEntry = new TextLogEntry(
                        (int)comboWellness.SelectedItem,
                        (int)comboQuality.SelectedItem,
                        textNotes.Text,
                        textTextEntry.Text
                    );
                    UpdateStatus("Text entry saved: " + newTextEntry.GetDetails());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving text entry: {ex.Message}", "Error");
            }

            ResetForm();
        }

        private void ListViewSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (listViewEntries.SelectedItem is LogEntry selectedEntry)
            {
                MessageBox.Show($"Selected Entry Details:\n{selectedEntry.Details}", "Entry Details");
            }

        }
    }
}