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
        /// Start or stop recording.
        /// </summary>
        private void RecordClick(object sender, RoutedEventArgs e)
        {
            try
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

                    if (recordingFile == null || !recordingFile.Exists)
                        throw new IOException("Recording failed to save.");

                    buttonSave.IsEnabled = true;
                    buttonPlay.IsEnabled = true;
                    buttonDelete.IsEnabled = true;
                    UpdateStatus($"Recording completed and saved to {recordingFile.FullName}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during recording: {ex.Message}", "Recording Error");
                ResetForm();
            }
        }

        /// <summary>
        /// Save an audio entry.
        /// </summary>
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure required fields are filled.
                ValidateComboBoxSelection();

                // Create an AudioLogEntry.
                if (tabController.SelectedItem == tabEntry)
                {
                    var newEntry = new AudioLogEntry(
                        (int)comboWellness.SelectedItem,
                        (int)comboQuality.SelectedItem,
                        textNotes.Text,
                        recordingFile
                    );
                    UpdateStatus($"Audio entry saved: {newEntry.GetDetails()}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving audio entry: {ex.Message}", "Error");
            }
            ResetForm();
        }

        /// <summary>
        /// Save a text entry.
        /// </summary>
        private void SaveTextClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure required fields are filled.
                ValidateComboBoxSelection();
                if (string.IsNullOrWhiteSpace(textTextEntry.Text))
                    throw new ArgumentException("Text entry content cannot be empty.");

                // Create a TextLogEntry.
                if (tabController.SelectedItem == tabTextEntry)
                {
                    var newTextEntry = new TextLogEntry(
                        (int)comboWellness.SelectedItem,
                        (int)comboQuality.SelectedItem,
                        textNotes.Text,
                        textTextEntry.Text
                    );
                    UpdateStatus($"Text entry saved: {newTextEntry.GetDetails()}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving text entry: {ex.Message}", "Error");
            }
            ResetForm();
        }

        /// <summary>
        /// Play the saved recording.
        /// </summary>
        private void PlayClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (recordingFile == null || !recordingFile.Exists)
                    throw new FileNotFoundException("Recording file not found.");

                var player = new SoundPlayer(recordingFile.FullName);
                player.Play();
                UpdateStatus($"Playing {recordingFile.FullName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing file: {ex.Message}", "Error");
            }
        }

        /// <summary>
        /// Delete the saved recording.
        /// </summary>
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (recordingFile == null || !recordingFile.Exists)
                    throw new FileNotFoundException("Recording file not found.");

                File.Delete(recordingFile.FullName);
                UpdateStatus($"Deleted {recordingFile.FullName}");

                buttonSave.IsEnabled = false;
                buttonPlay.IsEnabled = false;
                buttonDelete.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting file: {ex.Message}", "Error");
            }
        }

        /// <summary>
        /// Handle tab selection changes.
        /// </summary>
        private void TabChanged(object sender, RoutedEventArgs e)
        {
            if (tabController.SelectedItem == tabSummary)
            {
                textNumberOfEntries.Text = LogEntry.Count.ToString();
                textFirstEntry.Text = LogEntry.FirstEntry != DateTime.MinValue ? LogEntry.FirstEntry.ToShortDateString() : "N/A";
                textNewestEntry.Text = LogEntry.NewestEntry != DateTime.MinValue ? LogEntry.NewestEntry.ToShortDateString() : "N/A";
                UpdateStatus("Viewing summary tab.");
            }
        }

        /// <summary>
        /// Validate ComboBox selections.
        /// </summary>
        private void ValidateComboBoxSelection()
        {
            if (comboWellness.SelectedItem == null || comboQuality.SelectedItem == null)
                throw new InvalidOperationException("Wellness and Quality must be selected.");
        }

        /// <summary>
        /// Update the status bar with a message.
        /// </summary>
        private void UpdateStatus(string status)
        {
            statusState.Content = $"{DateTime.Now:MM/dd/yy H:mm:ss}: {status}";
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
            textNotes.Text = string.Empty;
        }

        /// <summary>
        /// Handle ListView selection changes.
        /// </summary>
        private void ListViewSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (listViewEntries.SelectedItem is LogEntry selectedEntry)
            {
                MessageBox.Show($"Selected Entry Details:\n{selectedEntry.Details}", "Entry Details");
            }
        }
    }
}
