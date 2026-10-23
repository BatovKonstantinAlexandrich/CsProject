using System;
using System.Drawing;
using System.Windows.Forms;

using WinFormsApp.AppComponents.AppControls;

namespace WinFormsApp.AppViewModules.ButtonModule
{
    /// <summary>
    /// Современный элемент управления кнопкой с улучшенным дизайном
    /// </summary>
    public partial class ButtonControl : ControlBase
    {
        private Color _hoverColor = Color.FromArgb(245, 245, 245);
        private Color _pressedColor = Color.FromArgb(235, 235, 235);
        private bool _isHovered = false;
        private bool _isPressed = false;

        public override Font Font { get => this.button.Font; set => this.button.Font = value; }


        public event EventHandler? Click
        {
            add => this.button.Click += value;
            remove => this.button.Click -= value;
        }

        /// <summary>
        /// Цвет при наведении
        /// </summary>
        public Color HoverColor
        {
            get => _hoverColor;
            set
            {
                _hoverColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Цвет при нажатии
        /// </summary>
        public Color PressedColor
        {
            get => _pressedColor;
            set
            {
                _pressedColor = value;
                Invalidate();
            }
        }

        public ButtonControl()
        {            
            InitializeComponent();
            this.BindMouseEvents(this.button);
            this.button.Dock = DockStyle.Fill;
            this.button.AutoSize = false;
            this.AutoSize = false;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            
            // Применяем современные стили к контейнеру
            this.BorderRadius = 6;
            this.BorderWidth = 0;
            this.ShowShadow = true;
            this.BackColor = Color.Transparent;
            this.Padding = new Padding(2);
            
            // Улучшаем внешний вид MaterialButton
            this.button.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.button.HighEmphasis = true;

            // Добавляем обработчики для эффектов наведения на контейнер
            this.MouseEnter += (s, e) => {
                _isHovered = true;
                this.Cursor = Cursors.Hand;
                Invalidate();
            };
            
            this.MouseLeave += (s, e) => {
                _isHovered = false;
                _isPressed = false;
                this.Cursor = Cursors.Default;
                Invalidate();
            };
            
            this.MouseDown += (s, e) => {
                _isPressed = true;
                Invalidate();
            };
            
            this.MouseUp += (s, e) => {
                _isPressed = false;
                Invalidate();
            };
        }

        public override string Text { get => button.Text; set => button.Text = value; }  

        public void SetIcon(string tag)
        {
            button.Icon = Image.FromFile($"Icons\\{tag}");
        }
         
        public void AddOnClickEvent(Action todo)
        {
            this.button.Click += (s, e) => {
                if (todo != null)
                {
                    todo();
                }
            };
        } 
    }
}
