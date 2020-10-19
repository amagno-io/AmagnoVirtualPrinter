using NamedPipeWrapper;
using System;
using System.Windows.Forms;

using JetBrains.Annotations;

using VirtualPrinter.ProgressInfo.Core.Message;

namespace VirtualPrinter.ProgressInfo
{
    public partial class ProgressForm : Form
    {
        private const string PipeName = "vdpagent";

        public ProgressForm()
        {
            InitializeComponent();

            var client = new NamedPipeClient<Core.Message.Message>(PipeName)
            {
                AutoReconnect = true
            };

            client.ServerMessage += ServerMessage;
            client.Start(new TimeSpan(TimeSpan.TicksPerMinute));
        }

        private void ServerMessage(NamedPipeConnection<Core.Message.Message, Core.Message.Message> connection, [NotNull]Core.Message.Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            switch (message.Type) {
                case MessageType.Finalize:
                    Invoke((Action) Finish);
                    break;
                case MessageType.Initialize:
                    Invoke((Action) Initialize);
                    break;
                case MessageType.Step:
                    Invoke((Action) (() => Progress(message.Value)));
                    break;
                case MessageType.Close:
                    Invoke((Action) Dispose);
                    break;
                case MessageType.None:
                    throw new ArgumentException("'None' is not a valid MessageType");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Progress(uint val)
        {
            lbProgress.Text = $@"{val} pages converted";
        }

        private void Initialize()
        {
            PlaceToBottomRight();
            Show();
            BringToFront();
        }

        private void Finish()
        {
            Hide();
        }

        private void PlaceToBottomRight()
        {
            var desktopWorkingArea = Screen.PrimaryScreen.WorkingArea;
            Left = desktopWorkingArea.Right - Width - 12;
            Top = desktopWorkingArea.Bottom - Height - 12;
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            Initialize();
        }
    }
}