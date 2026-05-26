using CyberSecurityAwarenessBot.Helpers;
using CyberSecurityAwarenessBot.Memory;
using CyberSecurityAwarenessBot.Responses;
using CyberSecurityAwarenessBot.Sentiment;
using CyberSecurityAwarenessBot.Themes;

namespace CyberSecurityAwarenessBot.Forms
{
    public class MainForm : Form
    {
        private Panel _sidebar = null!;
        private Panel _mainArea = null!;
        private Panel _topBar = null!;
        private Panel _inputBar = null!;
        private Panel _chatScroll = null!;
        private FlowLayoutPanel _messages = null!;
        private Panel _toolbar = null!;
        private Label _userCard = null!;
        private FlowLayoutPanel _topicBtns = null!;
        private Label _moodLabel = null!;
        private TextBox _inputBox = null!;
        private Button _sendBtn = null!;

        private readonly ResponseEngine _engine;
        private readonly UserMemory _memory;
        private readonly SentimentDetector _detector;
        private readonly AudioHelper _audio;

        private bool _nameCollected = false;
        private string _lastTopic = string.Empty;
        private ChatTheme _theme = ThemeManager.Themes[0];

        private static readonly string[] ShortcutTopics =
        {
            "Passwords", "Phishing", "Malware", "Ransomware",
            "2FA", "VPN", "Encryption", "Privacy",
            "Safe Browsing", "Data Breach", "Social Engineering", "Help"
        };

        private static readonly string[] TipsList =
        {
            "Use a password manager",
            "Enable 2FA on all accounts",
            "Update software regularly",
            "Use HTTPS sites only",
            "Back up your data (3-2-1)",
            "Use a VPN on public Wi-Fi",
            "Use unique passwords per site",
            "Review app permissions",
            "Verify emails before clicking",
        };
        private readonly Dictionary<string, CheckBox> _tipChecks = new();

        public MainForm()
        {
            _engine = new ResponseEngine();
            _memory = new UserMemory();
            _detector = new SentimentDetector();
            _audio = new AudioHelper();

            InitForm();
            Task.Run(() => _audio.PlayVoiceGreeting());
            AddBotBubble(
                "Hey! I am CyberBot — your personal cybersecurity guide.\n\n" +
                "Use the topic buttons on the left, or just type below.\n\n" +
                "What is your name?");
        }

        private void InitForm()
        {
            Text = "CyberBot  |  Security Awareness Assistant";
            Size = new Size(1080, 720);
            MinimumSize = new Size(820, 580);
            BackColor = _theme.PageBg;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10f);

            BuildTopBar();
            BuildSidebar();
            BuildMainArea();
            BuildInputBar();

            Controls.Add(_mainArea);
            Controls.Add(_sidebar);
            Controls.Add(_inputBar);
            Controls.Add(_topBar);
            _inputBox.Select();
        }

        private void BuildTopBar()
        {
            _topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = _theme.TopBarBg,
                Padding = new Padding(12, 0, 16, 0)
            };

            var avatar = new Label
            {
                Text = "CB",
                Size = new Size(34, 34),
                Location = new Point(14, 9),
                BackColor = _theme.Accent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var nameLabel = new Label
            {
                Text = "CyberBot",
                Location = new Point(57, 7),
                Size = new Size(220, 20),
                ForeColor = _theme.TextHi,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                BackColor = Color.Transparent
            };

            var sub = new Label
            {
                Text = "Online  •  Cybersecurity Assistant",
                Location = new Point(58, 28),
                Size = new Size(280, 16),
                ForeColor = _theme.Highlight,
                Font = new Font("Segoe UI", 8f),
                BackColor = Color.Transparent
            };

            _moodLabel = new Label
            {
                Text = string.Empty,
                Dock = DockStyle.Right,
                Width = 260,
                ForeColor = _theme.AccentLight,
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 4, 0)
            };

            var border = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = _theme.Border
            };

            _topBar.Controls.AddRange(new Control[] { _moodLabel, border, sub, nameLabel, avatar });
        }

        private void BuildSidebar()
        {
            _sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 210,
                BackColor = _theme.SidebarBg
            };

            var logo = new Label
            {
                Text = "⬡ CYBER\n    BOT",
                Dock = DockStyle.Top,
                Height = 68,
                ForeColor = _theme.Accent,
                Font = new Font("Segoe UI Black", 15f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(
                    Math.Max(0, _theme.SidebarBg.R - 6),
                    Math.Max(0, _theme.SidebarBg.G - 6),
                    Math.Max(0, _theme.SidebarBg.B - 6))
            };

            _userCard = new Label
            {
                Text = "Welcome!\nStart chatting below.",
                Dock = DockStyle.Top,
                Height = 58,
                ForeColor = _theme.TextMid,
                Font = new Font("Segoe UI", 8.5f),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(
                    Math.Min(255, _theme.SidebarBg.R + 6),
                    Math.Min(255, _theme.SidebarBg.G + 4),
                    Math.Min(255, _theme.SidebarBg.B + 10)),
                Padding = new Padding(6)
            };

            var sep1 = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = _theme.Border };

            var topicsHeading = new Label
            {
                Text = "  QUICK TOPICS",
                Dock = DockStyle.Top,
                Height = 24,
                ForeColor = _theme.TextLo,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            var tipsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 200,
                BackColor = Color.FromArgb(
                    Math.Max(0, _theme.SidebarBg.R - 3),
                    Math.Max(0, _theme.SidebarBg.G - 3),
                    Math.Max(0, _theme.SidebarBg.B - 3)),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(6, 4, 6, 4)
            };

            var sep2 = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = _theme.Border };

            _topicBtns = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(6, 4, 6, 4)
            };

            foreach (string topic in ShortcutTopics)
                _topicBtns.Controls.Add(MakeTopicButton(topic));

            _sidebar.Controls.Add(_topicBtns);
            _sidebar.Controls.Add(topicsHeading);
            _sidebar.Controls.Add(sep1);
            _sidebar.Controls.Add(_userCard);
            _sidebar.Controls.Add(logo);
            _sidebar.Controls.Add(tipsPanel);
            _sidebar.Controls.Add(sep2);
        }

        private Button MakeTopicButton(string topic)
        {
            var btn = new Button
            {
                Text = "  " + topic,
                Size = new Size(192, 30),
                BackColor = Color.FromArgb(
                    Math.Min(255, _theme.SidebarBg.R + 10),
                    Math.Min(255, _theme.SidebarBg.G + 8),
                    Math.Min(255, _theme.SidebarBg.B + 16)),
                ForeColor = _theme.TextMid,
                Font = new Font("Segoe UI", 9f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 2, 0, 2),
                Tag = topic
            };
            btn.FlatAppearance.BorderColor = _theme.Border;
            btn.FlatAppearance.BorderSize = 1;
            btn.MouseEnter += (s, e) => { btn.BackColor = _theme.BtnHover; btn.ForeColor = _theme.AccentLight; };
            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(
                    Math.Min(255, _theme.SidebarBg.R + 10),
                    Math.Min(255, _theme.SidebarBg.G + 8),
                    Math.Min(255, _theme.SidebarBg.B + 16));
                btn.ForeColor = _theme.TextMid;
            };
            btn.Click += TopicBtn_Click;
            return btn;
        }

        private void BuildMainArea()
        {
            _mainArea = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _theme.PageBg,
                Padding = new Padding(0)
            };

            _chatScroll = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _theme.PageBg,
                AutoScroll = true,
                Padding = new Padding(14, 10, 14, 10)
            };

            _messages = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = _theme.PageBg,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0)
            };

            _chatScroll.Controls.Add(_messages);
            _mainArea.Controls.Add(_chatScroll);
        }

        private void BuildInputBar()
        {
            _inputBar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 98,
                BackColor = _theme.InputBg,
                Padding = new Padding(14, 8, 14, 8)
            };

            var topLine = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = _theme.Border };

            _toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 32,
                BackColor = Color.Transparent
            };

            var themeLabel = new Label
            {
                Text = "Theme:",
                Location = new Point(0, 7),
                Size = new Size(52, 18),
                ForeColor = _theme.TextLo,
                Font = new Font("Segoe UI", 8.5f),
                BackColor = Color.Transparent
            };

            int xPos = 56;
            for (int i = 0; i < ThemeManager.Themes.Count; i++)
            {
                int capturedIndex = i;
                ChatTheme t = ThemeManager.Themes[i];

                var dot = new Button
                {
                    Size = new Size(22, 22),
                    Location = new Point(xPos, 5),
                    BackColor = t.Accent,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Tag = capturedIndex,
                    Text = string.Empty
                };
                dot.FlatAppearance.BorderSize = 1;
                dot.FlatAppearance.BorderColor = t.AccentLight;
                dot.Click += ThemeBtn_Click;

                var tip = new ToolTip();
                tip.SetToolTip(dot, t.Name);

                _toolbar.Controls.Add(dot);
                xPos += 28;
            }

            // ── Background colour picker button ──────────────────────────────
            

            var clearBtn = new Button
            {
                Text = "🗑 Clear Chat",
                Size = new Size(110, 24),
                Location = new Point(xPos + 126, 4),
                BackColor = _theme.BotBubble,
                ForeColor = _theme.TextMid,
                Font = new Font("Segoe UI", 8.5f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            clearBtn.FlatAppearance.BorderColor = _theme.Border;
            clearBtn.FlatAppearance.BorderSize = 1;
            clearBtn.Click += (s, e) =>
            {
                _messages.Controls.Clear();
                AddBotBubble("Chat cleared! How can I help you, " + _memory.Name + "?");
            };
            _toolbar.Controls.Add(clearBtn);
            _toolbar.Controls.Add(themeLabel);

            var inputRow = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            _sendBtn = new Button
            {
                Text = "Send  ➤",
                Dock = DockStyle.Right,
                Width = 96,
                BackColor = _theme.Accent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _sendBtn.FlatAppearance.BorderSize = 0;
            _sendBtn.FlatAppearance.MouseOverBackColor = _theme.AccentLight;
            _sendBtn.Click += SendBtn_Click;

            _inputBox = new TextBox
            {
                Dock = DockStyle.Fill,
                BackColor = _theme.InputText,
                ForeColor = _theme.TextHi,
                Font = new Font("Segoe UI", 11f),
                BorderStyle = BorderStyle.None,
                PlaceholderText = "Type a message or pick a topic...",
            };
            _inputBox.KeyDown += InputBox_KeyDown;

            inputRow.Controls.Add(_inputBox);
            inputRow.Controls.Add(_sendBtn);

            _inputBar.Controls.Add(inputRow);
            _inputBar.Controls.Add(_toolbar);
            _inputBar.Controls.Add(topLine);
        }

        // ── Background colour picker ──────────────────────────────────────────
        private void BgBtn_Click(object? sender, EventArgs e)
        {
            using var dlg = new ColorDialog
            {
                Color = _theme.PageBg,
                FullOpen = true,
                AnyColor = true,
                SolidColorOnly = false
            };

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                Color chosen = dlg.Color;
                // Apply the chosen colour to every chat background surface
                BackColor = chosen;
                _mainArea.BackColor = chosen;
                _chatScroll.BackColor = chosen;
                _messages.BackColor = chosen;
                AddBotBubble($"Background updated! Looks great, {_memory.Name}!");
            }
        }

        private void ThemeBtn_Click(object? sender, EventArgs e)
        {
            if (sender is Button b && b.Tag is int idx)
            {
                _theme = ThemeManager.Themes[idx];
                ApplyTheme();
                AddBotBubble($"Theme switched to \"{_theme.Name}\"! Looking good, {_memory.Name}!");
            }
        }

        private void ApplyTheme()
        {
            BackColor = _theme.PageBg;
            _topBar.BackColor = _theme.TopBarBg;
            _sidebar.BackColor = _theme.SidebarBg;
            _mainArea.BackColor = _theme.PageBg;
            _chatScroll.BackColor = _theme.PageBg;
            _messages.BackColor = _theme.PageBg;
            _inputBar.BackColor = _theme.InputBg;
            _moodLabel.ForeColor = _theme.AccentLight;
            _inputBox.BackColor = _theme.InputText;
            _inputBox.ForeColor = _theme.TextHi;
            _sendBtn.BackColor = _theme.Accent;
            _sendBtn.FlatAppearance.MouseOverBackColor = _theme.AccentLight;

            _topicBtns.Controls.Clear();
            foreach (string topic in ShortcutTopics)
                _topicBtns.Controls.Add(MakeTopicButton(topic));

            foreach (var cb in _tipChecks.Values)
                cb.ForeColor = cb.Checked ? _theme.Highlight : _theme.TextMid;

            foreach (Control c in _toolbar.Controls)
                if (c is Label lbl) lbl.ForeColor = _theme.TextLo;

            Refresh();
        }

        private void InputBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; ProcessInput(); }
        }

        private void SendBtn_Click(object? sender, EventArgs e) => ProcessInput();

        private void TopicBtn_Click(object? sender, EventArgs e)
        {
            if (sender is Button b && b.Tag is string t)
            { _inputBox.Text = t; ProcessInput(); }
        }

        // ── BUG FIX 1: AddUserBubble called FIRST, then name check, then reply
        private void ProcessInput()
        {
            string raw = _inputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(raw)) return;
            _inputBox.Clear();

            // Always show what the user typed straight away
            AddUserBubble(raw);

            if (raw.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                raw.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                AddBotBubble($"Take care, {_memory.Name}! Stay safe online.");
                return;
            }

            // BUG FIX 2: collect name BEFORE anything else
            if (!_nameCollected)
            {
                _memory.SetName(raw);          // store the name properly
                _nameCollected = true;
                RefreshUserCard();             // update sidebar card immediately
                AddBotBubble(
                    $"Great to meet you, {_memory.Name}!\n\n" +
                    "Click any topic on the left to get started, or just type your question.\n\n" +
                    "I cover passwords, phishing, malware, ransomware, 2FA, encryption and more.");
                return;
            }

            if (IsFollowUp(raw)) { HandleFollowUp(); return; }

            SentimentResult mood = _detector.Detect(raw);
            RefreshMoodLabel(mood);
            string prefix = SentimentPrefix(mood);

            string reply = _engine.GetResponse(raw, _memory.Name, _memory);

            // BUG FIX 3: store topic AND refresh sidebar so favourite is never lost
            string? topic = _engine.LastDetectedTopic;
            if (topic != null)
            {
                _lastTopic = topic;
                _memory.SetFavouriteTopic(topic);   // persist favourite topic
                RefreshUserCard();                  // immediately show it in sidebar
            }

            AddBotBubble(string.IsNullOrEmpty(prefix) ? reply : prefix + "\n\n" + reply);
        }

        private static bool IsFollowUp(string s)
        {
            string l = s.ToLower();
            return l.Contains("tell me more") || l.Contains("explain more") ||
                   l.Contains("more info") || l.Contains("another tip") ||
                   l.Contains("go on") || l.Contains("continue") ||
                   l.Contains("expand") || l.Contains("keep going");
        }

        private void HandleFollowUp()
        {
            if (string.IsNullOrEmpty(_lastTopic))
            {
                AddBotBubble(
                    $"Not sure which topic to expand on, {_memory.Name}.\n" +
                    "Click a topic on the left or ask about something specific.");
                return;
            }
            AddBotBubble(
                $"More on {_lastTopic}:\n\n" +
                _engine.GetFollowUp(_lastTopic, _memory.Name));
        }

        private string SentimentPrefix(SentimentResult s) => s.Type switch
        {
            SentimentType.Worried =>
                $"It is completely understandable to feel worried, {_memory.Name}. " +
                "You are taking the right step by learning about this.",
            SentimentType.Frustrated =>
                $"I hear you, {_memory.Name} — cybersecurity can feel overwhelming. " +
                "Let us break this down together.",
            SentimentType.Curious =>
                $"Love the curiosity, {_memory.Name}! Here is what you need to know.",
            SentimentType.Happy => $"Great energy, {_memory.Name}!",
            SentimentType.Confused =>
                $"No problem at all, {_memory.Name}. " +
                "Let me explain this as clearly as possible.",
            _ => string.Empty
        };

        private void RefreshMoodLabel(SentimentResult s)
        {
            _moodLabel.Text = s.Type == SentimentType.Neutral
                ? string.Empty
                : $"Mood detected: {s.Label}";
        }

        private void RefreshUserCard()
        {
            _userCard.Text =
                $"{_memory.Name}\n" +
                $"Interest: {_memory.FavouriteTopic ?? "None yet"}\n" +
                $"Messages: {_memory.MessageCount}";
        }

        private void AddUserBubble(string text)
        {
            string time = DateTime.Now.ToString("HH:mm");
            var container = new Panel
            {
                BackColor = Color.Transparent,
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4),
                Width = _messages.ClientSize.Width - 10
            };

            var timeLabel = new Label
            {
                Text = $"{_memory.Name}  {time}",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = _theme.AccentLight,
                BackColor = Color.Transparent,
                AutoSize = true,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };

            var msgLabel = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = Color.White,
                BackColor = _theme.UserBubble,
                AutoSize = true,
                MaximumSize = new Size(520, 0),
                Padding = new Padding(14, 10, 14, 10),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Margin = new Padding(0, 2, 0, 0)
            };

            container.Controls.Add(msgLabel);
            container.Controls.Add(timeLabel);

            int w = container.Width;
            container.PerformLayout();
            int h = timeLabel.Height + msgLabel.Height + 6;
            container.Height = h;
            timeLabel.Location = new Point(w - timeLabel.Width - 2, 0);
            msgLabel.Location = new Point(w - msgLabel.Width - 2, timeLabel.Height + 3);

            _messages.Controls.Add(container);
            ScrollToBottom();
        }

        private void AddBotBubble(string text)
        {
            string time = DateTime.Now.ToString("HH:mm");
            var container = new Panel
            {
                BackColor = Color.Transparent,
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4),
                Width = _messages.ClientSize.Width - 10
            };

            var timeLabel = new Label
            {
                Text = $"CyberBot  {time}",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = _theme.Highlight,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(6, 0)
            };

            var accent = new Panel
            {
                BackColor = _theme.Accent,
                Width = 3,
                Location = new Point(0, 0)
            };

            var msgLabel = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = _theme.TextHi,
                BackColor = _theme.BotBubble,
                AutoSize = true,
                MaximumSize = new Size(580, 0),
                Padding = new Padding(14, 10, 14, 10),
                Location = new Point(6, 0),
                Margin = new Padding(0, 2, 0, 0)
            };

            container.Controls.Add(msgLabel);
            container.Controls.Add(accent);
            container.Controls.Add(timeLabel);

            container.PerformLayout();
            int msgTop = timeLabel.Height + 3;
            int totalH = msgTop + msgLabel.Height + 4;
            container.Height = totalH;
            timeLabel.Location = new Point(6, 0);
            accent.Location = new Point(0, msgTop);
            accent.Height = msgLabel.Height;
            msgLabel.Location = new Point(6, msgTop);

            _messages.Controls.Add(container);
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            _chatScroll.ScrollControlIntoView(
                _messages.Controls.Count > 0
                    ? _messages.Controls[_messages.Controls.Count - 1]
                    : _messages);
        }
    }
}