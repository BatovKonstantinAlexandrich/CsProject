using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using NAudio.Wave;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViews.MediaModule;

namespace WinFormsApp.AppViewModules.MediaModule;

/// <summary>
/// Диктофон: запись с микрофона в WAV через NAudio (<see cref="WaveInEvent"/>).
/// </summary>
public class AudioRecorderControl : ControlBase
{
    private readonly Label _status = new()
    {
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleLeft,
        AutoEllipsis = true,
        Padding = new Padding(8, 0, 0, 0),
    };
    private readonly Label _timer = new()
    {
        AutoSize = true,
        Text = "00:00",
        Padding = new Padding(8, 6, 8, 6),
    };
    private readonly TextBox _pathBox = new() { Dock = DockStyle.Fill, PlaceholderText = "Путь к WAV-файлу (пусто — временный каталог)" };
    private readonly Button _record = new() { Text = "● Запись", AutoSize = true, Padding = new Padding(12, 4, 12, 4) };
    private readonly Button _play = new() { Text = "Воспроизвести", AutoSize = true };
    private readonly Button _stopPlay = new() { Text = "Стоп", AutoSize = true };

    private AudioRecorderView _recorderView;
    private bool _handlersAttached;
    private WaveInEvent _waveIn;
    private WaveFileWriter _writer;
    private string _activeRecordingPath;
    private DateTime _recordingStartedAt;
    private readonly System.Windows.Forms.Timer _elapsedTimer = new() { Interval = 200 };

    private WaveOutEvent _playback;
    private AudioFileReader _reader;

    public AudioRecorderControl()
    {
        Dock = DockStyle.Fill;
        MinimumSize = new Size(360, 96);

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(4),
        };
        toolbar.Controls.Add(_record);
        toolbar.Controls.Add(_play);
        toolbar.Controls.Add(_stopPlay);
        toolbar.Controls.Add(_timer);

        var statusPanel = new Panel { Dock = DockStyle.Top, Height = 28, Padding = new Padding(4, 0, 4, 0) };
        statusPanel.Controls.Add(_status);

        var pathPanel = new Panel { Dock = DockStyle.Top, Height = 32, Padding = new Padding(4) };
        pathPanel.Controls.Add(_pathBox);

        Controls.Add(toolbar);
        Controls.Add(statusPanel);
        Controls.Add(pathPanel);

        _record.Click += (_, _) => ToggleRecording();
        _play.Click += (_, _) => PlayRecordedFile();
        _stopPlay.Click += (_, _) => StopPlayback();
        _elapsedTimer.Tick += (_, _) => UpdateElapsed();

        HandleDestroyed += (_, _) =>
        {
            _elapsedTimer.Stop();
            StopRecordingInternal(setViewState: false);
            StopPlayback();
        };
        Load += OnRecorderControlLoaded;
    }

    /// <summary>Привязка модели сразу после <see cref="ComponentsFactory.CreateAudioRecorderPanel"/>.</summary>
    public void BindViewModel(AudioRecorderView view)
    {
        if (view is null)
            return;
        viewModel = view;
        view.control = this;
        BindRecorderView(view);
    }

    private void OnRecorderControlLoaded(object sender, EventArgs e)
    {
        if (viewModel is AudioRecorderView view)
            BindViewModel(view);
    }

    private void BindRecorderView(AudioRecorderView view)
    {
        if (_handlersAttached && ReferenceEquals(_recorderView, view))
            return;

        _recorderView = view;
        if (_handlersAttached)
            return;

        _handlersAttached = true;
        _pathBox.Text = view.OutputPath ?? "";
        SetStatus(view.StatusMessage);
        UpdateTimerLabel(view.ElapsedSeconds);
        UpdateRecordButton(view.IsRecording);

        view.AddPropertyChangeListener(nameof(AudioRecorderView.OutputPath), evt =>
        {
            var next = evt.After?.ToString() ?? "";
            if (_pathBox.Text != next)
                _pathBox.Text = next;
        });
        view.AddPropertyChangeListener(nameof(AudioRecorderView.IsRecording), evt =>
        {
            if (evt.After is bool recording)
                UpdateRecordButton(recording);
        });
        view.AddPropertyChangeListener(nameof(AudioRecorderView.ElapsedSeconds), evt =>
        {
            if (evt.After is double seconds)
                UpdateTimerLabel(seconds);
        });
        view.AddPropertyChangeListener(nameof(AudioRecorderView.StatusMessage), evt =>
            SetStatus(evt.After?.ToString() ?? ""));
        view.AddPropertyChangeListener(nameof(AudioRecorderView.AutoPlayAfterRecord), _ => { });

        _pathBox.TextChanged += (_, _) =>
        {
            if (view.OutputPath != _pathBox.Text)
                view.OutputPath = _pathBox.Text;
        };
    }

    private void ToggleRecording()
    {
        if (_recorderView?.IsRecording == true)
            StopRecording();
        else
            StartRecording();
    }

    private void StartRecording()
    {
        if (_recorderView is null)
            return;

        StopPlayback();
        _recorderView.LastError = "";

        try
        {
            var path = ResolveOutputPath(_recorderView.OutputPath);
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            _activeRecordingPath = path;
            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(44100, 16, 1),
            };
            _writer = new WaveFileWriter(path, _waveIn.WaveFormat);
            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.RecordingStopped += OnRecordingStopped;

            _recordingStartedAt = DateTime.UtcNow;
            _waveIn.StartRecording();

            _recorderView.IsRecording = true;
            _recorderView.ElapsedSeconds = 0;
            _recorderView.StatusMessage = "Запись…";
            _elapsedTimer.Start();
            UpdateRecordButton(true);
        }
        catch (Exception ex)
        {
            _recorderView.LastError = ex.Message;
            _recorderView.StatusMessage = "Ошибка записи";
            StopRecordingInternal(setViewState: true);
            MessageBox.Show(ex.Message, "Диктофон", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void StopRecording()
    {
        try
        {
            _waveIn?.StopRecording();
        }
        catch (Exception ex)
        {
            if (_recorderView is not null)
            {
                _recorderView.LastError = ex.Message;
                _recorderView.StatusMessage = "Ошибка остановки записи";
            }
        }
    }

    private void OnDataAvailable(object sender, WaveInEventArgs e)
    {
        _writer?.Write(e.Buffer, 0, e.BytesRecorded);
    }

    private void OnRecordingStopped(object sender, StoppedEventArgs e)
    {
        RunOnUiThreadSafe(() =>
        {
            if (e.Exception is not null && _recorderView is not null)
            {
                _recorderView.LastError = e.Exception.Message;
                _recorderView.StatusMessage = "Ошибка записи";
            }

            var savedPath = _activeRecordingPath;
            StopRecordingInternal(setViewState: true);

            if (_recorderView is null)
                return;

            if (!string.IsNullOrWhiteSpace(savedPath) && File.Exists(savedPath))
            {
                _recorderView.OutputPath = savedPath;
                _pathBox.Text = savedPath;
                _recorderView.StatusMessage = $"Сохранено: {savedPath}";
                if (_recorderView.AutoPlayAfterRecord)
                    PlayFromPath(savedPath);
            }
            else if (string.IsNullOrEmpty(_recorderView.LastError))
            {
                _recorderView.StatusMessage = "Запись остановлена";
            }
        });
    }

    private void StopRecordingInternal(bool setViewState)
    {
        _elapsedTimer.Stop();

        try
        {
            _waveIn?.StopRecording();
        }
        catch
        {
            /* ignored */
        }

        _waveIn?.Dispose();
        _waveIn = null;

        try
        {
            _writer?.Dispose();
        }
        catch
        {
            /* ignored */
        }

        _writer = null;

        if (setViewState && _recorderView is not null)
        {
            _recorderView.IsRecording = false;
            UpdateRecordButton(false);
        }
    }

    private void UpdateElapsed()
    {
        if (_recorderView is null || !_recorderView.IsRecording)
            return;

        var elapsed = (DateTime.UtcNow - _recordingStartedAt).TotalSeconds;
        _recorderView.ElapsedSeconds = elapsed;
        UpdateTimerLabel(elapsed);
    }

    private void PlayRecordedFile()
    {
        var path = _pathBox.Text;
        if (string.IsNullOrWhiteSpace(path) && _recorderView is not null)
            path = _recorderView.OutputPath;
        PlayFromPath(path);
    }

    private void PlayFromPath(string path)
    {
        StopPlayback();
        if (string.IsNullOrWhiteSpace(path))
            return;

        try
        {
            if (!File.Exists(path))
            {
                MessageBox.Show($"Файл не найден: {path}", "Диктофон", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _reader = new AudioFileReader(path);
            _playback = new WaveOutEvent();
            _playback.Init(_reader);
            _playback.PlaybackStopped += (_, _) => RunOnUiThreadSafe(StopPlayback);
            _playback.Play();
            if (_recorderView is not null)
                _recorderView.StatusMessage = "Воспроизведение…";
        }
        catch (Exception ex)
        {
            if (_recorderView is not null)
            {
                _recorderView.LastError = ex.Message;
                _recorderView.StatusMessage = "Ошибка воспроизведения";
            }
            MessageBox.Show(ex.Message, "Диктофон", MessageBoxButtons.OK, MessageBoxIcon.Error);
            StopPlayback();
        }
    }

    private void StopPlayback()
    {
        try
        {
            _playback?.Stop();
        }
        catch
        {
            /* ignored */
        }

        _playback?.Dispose();
        _playback = null;
        _reader?.Dispose();
        _reader = null;

        if (_recorderView is not null && !_recorderView.IsRecording)
            _recorderView.StatusMessage = string.IsNullOrWhiteSpace(_recorderView.OutputPath)
                ? "Готов к записи"
                : $"Файл: {_recorderView.OutputPath}";
    }

    private static string ResolveOutputPath(string configuredPath)
    {
        if (!string.IsNullOrWhiteSpace(configuredPath))
            return configuredPath.Trim();

        return Path.Combine(
            Path.GetTempPath(),
            $"recording_{DateTime.Now:yyyyMMdd_HHmmss}.wav");
    }

    private void SetStatus(string text) => _status.Text = text ?? "";

    private void UpdateTimerLabel(double seconds)
    {
        var total = Math.Max(0, (int)Math.Floor(seconds));
        _timer.Text = $"{total / 60:00}:{total % 60:00}";
    }

    private void UpdateRecordButton(bool recording)
    {
        _record.Text = recording ? "■ Стоп" : "● Запись";
        _record.ForeColor = recording ? Color.Red : SystemColors.ControlText;
    }

    private void RunOnUiThreadSafe(Action action)
    {
        try
        {
            if (!IsDisposed && IsHandleCreated)
                BeginInvoke(action);
            else
                action();
        }
        catch (ObjectDisposedException)
        {
        }
    }
}
