using System;
using System.IO;
using System.Collections.Generic;

namespace LearningLog2024
{
    // Abstract base class for all types of log entries.
    internal abstract class LogEntry
    {
        #region "Variable Declarations"

        // Static variables for shared data across all log entries.
        protected internal static int count = 0;
        protected internal static DateTime firstEntry;
        protected internal static DateTime newestEntry;
        protected internal static List<LogEntry> entries = new List<LogEntry>();

        // Instance variables.
        protected int logId;
        protected DateTime logDate = DateTime.Now;
        protected int logWellness;
        protected int logQuality;
        protected string logNotes = string.Empty;

        #endregion

        #region "Constructors"

        // Base constructor for a log entry.
        protected LogEntry()
        {
            count++;
            logId = count;
            if (count == 1)
            {
                firstEntry = logDate;
            }
            newestEntry = logDate;
        }

        #endregion

        #region "Properties"

        // Unique ID for the log entry.
        public int Id => logId;

        // Date of the log entry.
        public DateTime EntryDate => logDate;

        // Wellness property with validation.
        public int Wellness
        {
            get => logWellness;
            set
            {
                if (value < 1 || value > 5)
                    throw new ArgumentOutOfRangeException(nameof(Wellness), "Wellness must be between 1 and 5.");
                logWellness = value;
            }
        }

        // Quality property with validation.
        public int Quality
        {
            get => logQuality;
            set
            {
                if (value < 1 || value > 5)
                    throw new ArgumentOutOfRangeException(nameof(Quality), "Quality must be between 1 and 5.");
                logQuality = value;
            }
        }

        // Notes property with validation.
        public string Notes
        {
            get => logNotes;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Notes cannot be empty.", nameof(Notes));
                logNotes = value;
            }
        }

        // Static list of entries.
        public static List<LogEntry> Entries => entries;

        // Static properties for log summary data.
        public static int Count => count;
        public static DateTime FirstEntry => firstEntry;
        public static DateTime NewestEntry => newestEntry;

        // New: Entry type property (to display the type of entry in the ListView).
        public virtual string EntryType => "Base Entry";

        // New: Details property (to display specific details in the ListView).
        public virtual string Details => ToString();

        #endregion

        #region "Methods"

        // Abstract method to display specific entry details.
        public abstract string GetDetails();

        // Override ToString to provide a summary.
        public override string ToString()
        {
            return $"Entry {Id} on {EntryDate.ToShortDateString()}: Wellness {Wellness}, Quality {Quality}";
        }

        #endregion
    }

    // AUDIOLOGENTRY
    internal class AudioLogEntry : LogEntry
    {
        // Property for the recording file.
        public FileInfo RecordingFile { get; set; }

        public AudioLogEntry(int wellness, int quality, string notes, FileInfo recordingFile)
        {
            Wellness = wellness;
            Quality = quality;
            Notes = notes;
            RecordingFile = recordingFile ?? throw new ArgumentNullException(nameof(recordingFile), "Recording file cannot be null.");

            // Add the entry to the static list.
            Entries.Add(this);
        }

        // Override GetDetails to provide specific details for audio entries.
        public override string GetDetails()
        {
            return $"{ToString()}, Recording: {RecordingFile.Name}";
        }

        // Override EntryType for AudioLogEntry.
        public override string EntryType => "Audio";

        // Override Details for AudioLogEntry.
        public override string Details => $"Recording: {RecordingFile?.Name}";
    }

    // TEXTLOGENTRY
    internal class TextLogEntry : LogEntry
    {
        // Property for additional text.
        public string Text { get; set; }

        public TextLogEntry(int wellness, int quality, string notes, string text)
        {
            Wellness = wellness;
            Quality = quality;
            Notes = notes;
            Text = !string.IsNullOrWhiteSpace(text) ? text : throw new ArgumentException("Text cannot be empty.", nameof(text));

            // Add the entry to the static list.
            Entries.Add(this);
        }

        // Override GetDetails to provide specific details for text entries.
        public override string GetDetails()
        {
            return $"{ToString()}, Text: {Text}";
        }

        // Override EntryType for TextLogEntry.
        public override string EntryType => "Text";

        // Override Details for TextLogEntry.
        public override string Details => $"Text: {Text}";
    }
}
