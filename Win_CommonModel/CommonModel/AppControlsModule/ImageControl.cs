using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WinFormsApp.AppComponents.AppControls;

namespace WinFormsApp.AppViewModules.ImageModule
{
    public partial class ImageControl : ControlBase
    {
        public ImageControl()
        {
            InitializeComponent();
            AttachPictureBoxInteraction(this.pictureBox);
            this.AutoSize = true;
            this.MinimumSize = new(320, 240);
        }

        /// <summary>
        /// Клик по <see cref="PictureBox"/> не всплывает на UserControl — дублируем на модель и стандартное событие <see cref="Control.Click"/>.
        /// </summary>
        private void AttachPictureBoxInteraction(PictureBox box)
        {
            this.BindMouseEvents(box);
            box.Click -= PictureBox_Click;
            box.Click += PictureBox_Click;
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            viewModel?.OnClick(this, e);
            OnClick(e);
        }

        public void SetData(byte[] data, bool keepAspectRatio = true)
        {
            var box = this.pictureBox;
            box.SizeMode = keepAspectRatio
                ? PictureBoxSizeMode.Zoom
                : PictureBoxSizeMode.StretchImage;

            if (data is null || data.Length == 0)
            {
                box.Image = null;
                this.LayoutFill(box);
                return;
            }

            using (var stream = new MemoryStream(data))
            {
                box.Image = Image.FromStream(stream);
            }

            if (!keepAspectRatio)
            {
                box.Size = box.Image.Size;
                var w = box.Width;
                var h = box.Height;
                box.Resize += (s, e) =>
                {
                    if (h > 0 && box.Width != box.Height * w / h)
                        box.Width = box.Height * w / h;
                };
            }

            this.LayoutFill(box);
        }
    }
}
