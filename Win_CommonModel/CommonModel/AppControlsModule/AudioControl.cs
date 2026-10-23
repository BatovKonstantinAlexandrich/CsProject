using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using NAudio.Wave;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViews.MediaModule;

namespace WinFormsApp.AppViewModules.MediaModule;

/// <summary>
/// Воспроизведение аудио по пути к файлу (NAudio).
/// </summary>
public class AudioControl : ControlBase
{
    private readonly TextBox _pathBox = new() { Dock = DockStyle.Fill, PlaceholderText = "Путь к файлу или URL" };
    private readonly Button _play = new() { Text = "Воспроизвести", AutoSize = true };
    private readonly Button _stop = new() { Text = "Стоп", AutoSize = true };
    private WaveOutEvent _output;
    private AudioFileReader _reader;

    public AudioControl()
    {
        Dock = DockStyle.Fill;
        MinimumSize = new Size(320, 72);
        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(4),
        };
        toolbar.Controls.Add(_play);
        toolbar.Controls.Add(_stop);
        var pathPanel = new Panel { Dock = DockStyle.Top, Height = 32, Padding = new Padding(4) };
        pathPanel.Controls.Add(_pathBox);
        Controls.Add(toolbar);
        Controls.Add(pathPanel);

        _play.Click += (_, _) => PlayFromPath(_pathBox.Text);
        _stop.Click += (_, _) => StopPlayback();

        this.HandleDestroyed += (_, _) => StopPlayback();
        this.Load += OnAudioControlLoaded;
    }

    private void OnAudioControlLoaded(object sender, EventArgs e)
    {
        if (viewModel is AudioView av)
        {
            _pathBox.Text = av.Source ?? "";
            av.AddPropertyChangeListener(nameof(AudioView.Source), evt =>
            {
                var next = evt.After?.ToString() ?? "";
                if (_pathBox.Text != next)
                    _pathBox.Text = next;
                if (av.AutoPlay && !string.IsNullOrWhiteSpace(next))
                    PlayFromPath(next);
            });
            av.AddPropertyChangeListener(nameof(AudioView.AutoPlay), _ => { });
            _pathBox.TextChanged += (_, _) =>
            {
                if (av.Source != _pathBox.Text)
                    av.Source = _pathBox.Text;
            };
        }
    }

    private void PlayFromPath(string path)
    {
        StopPlayback();
        if (string.IsNullOrWhiteSpace(path))
            return;
        if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("Потоковое аудио по HTTP в этом контроле не поддерживается — укажите локальный файл.", "Аудио", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            if (!File.Exists(path))
            {
                MessageBox.Show($"Файл не найден: {path}", "Аудио", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _reader = new AudioFileReader(path);
            _output = new WaveOutEvent();
            _output.Init(_reader);
            _output.PlaybackStopped += (_, _) => RunOnUiThreadSafe(StopPlayback);
            _output.Play();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка воспроизведения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            StopPlayback();
        }
    }

    private void RunOnUiThreadSafe(Action a)
    {
        try
        {
            if (!IsDisposed && IsHandleCreated)
                BeginInvoke(a);
            else
                a();
        }
        catch (ObjectDisposedException)
        {
        }
    }

    private void StopPlayback()
    {
        try
        {
            _output?.Stop();
        }
        catch
        {
            /* ignored */
        }

        _output?.Dispose();
        _output = null;
        _reader?.Dispose();
        _reader = null;
    }
}
