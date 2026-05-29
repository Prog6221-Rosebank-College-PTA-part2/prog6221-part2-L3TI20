using CyberSecurityAwarenessBot.Helpers;
using CyberSecurityAwarenessBot.Memory;
using CyberSecurityAwarenessBot.Responses;
using CyberSecurityAwarenessBot.Sentiment;

namespace CyberSecurityAwarenessBot.Forms
{

    public class MainForm : Form
    {
        //  Layout 
        private Panel _sidebar = null!;
        private Panel _mainArea = null!;
        private Panel _topBar = null!;
        private Panel _inputBar = null!;
        private Panel _chatScroll = null!;
        private FlowLayoutPanel _messages = null!;

        // Sidebar controls 
        private Label _userCard = null!;
        private FlowLayoutPanel _topicBtns = null!;

        // Top bar controls 
        private Label _moodLabel = null!;

        //  Input controls
        private TextBox _inputBox = null!;
        private Button _sendBtn = null!;

        // Services
        private readonly ResponseEngine _engine;
        private readonly UserMemory _memory;
        private readonly SentimentDetector _detector;
        private readonly AudioHelper _audio;

        
        private bool _nameCollected = false;
        private string _lastTopic = string.Empty;

        // Colour palette 
        private static readonly Color C_PageBg = Color.FromArgb(14, 11, 28);
        private static readonly Color C_SidebarBg = Color.FromArgb(20, 16, 40);
        private static readonly Color C_TopBarBg = Color.FromArgb(26, 21, 50);
        private static readonly Color C_InputBg = Color.FromArgb(20, 16, 40);
        private static readonly Color C_BotBubble = Color.FromArgb(32, 26, 60);
        private static readonly Color C_UserBubble = Color.FromArgb(90, 46, 186);
        private static readonly Color C_Purple = Color.FromArgb(140, 80, 255);
        private static readonly Color C_Violet = Color.FromArgb(185, 130, 255);
        private static readonly Color C_Pink = Color.FromArgb(255, 75, 180);
        private static readonly Color C_Mint = Color.FromArgb(48, 220, 175);
        private static readonly Color C_TextHi = Color.FromArgb(242, 238, 255);
        private static readonly Color C_TextMid = Color.FromArgb(160, 150, 200);
        private static readonly Color C_TextLo = Color.FromArgb(95, 85, 135);
        private static readonly Color C_Border = Color.FromArgb(50, 42, 85);
        private static readonly Color C_BtnHover = Color.FromArgb(52, 44, 90);

        private static readonly string[] ShortcutTopics =
        {
            "Passwords", "Phishing", "Malware", "Ransomware",
            "2FA", "VPN", "Encryption", "Privacy",
            "Safe Browsing", "Data Breach", "Social Engineering", "Help"
        };

        public MainForm()
        {
            _engine = new ResponseEngine();
            _memory = new UserMemory();
            _detector = new SentimentDetector();
            _audio = new AudioHelper();

            InitForm();
            Task.Run(() => _audio.PlayVoiceGreeting());
            AddBotBubble(
                "Hey! I am CyberBot, your personal cybersecurity guide.\n\n" +
                "I can help you stay safe online.\n" +
                "Use the topic buttons on the left or just type below.\n\n" +
                "What is your name?");
        }


        private void InitForm()
        {
            Text = "CyberBot  |  Security Awareness Assistant";
            Size = new Size(1040, 700);
            MinimumSize = new Size(780, 560);
            BackColor = C_PageBg;
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
                BackColor = C_TopBarBg,
                Padding = new Padding(12, 0, 16, 0)
            };

            var avatar = new Label
            {
                Text = "CB",
                Size = new Size(34, 34),
                Location = new Point(14, 9),
                BackColor = C_Purple,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var name = new Label
            {
                Text = "CyberBot",
                Location = new Point(57, 7),
                Size = new Size(220, 20),
                ForeColor = C_TextHi,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                BackColor = Color.Transparent
            };

            var sub = new Label
            {
                Text = "Online  •  Cybersecurity ChatBot",
                Location = new Point(58, 28),
                Size = new Size(280, 16),
                ForeColor = C_Mint,
                Font = new Font("Segoe UI", 8f),
                BackColor = Color.Transparent
            };

            _moodLabel = new Label
            {
                Text = string.Empty,
                Dock = DockStyle.Right,
                Width = 240,
                ForeColor = C_Violet,
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 4, 0)
            };

            var border = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = C_Border
            };

            _topBar.Controls.AddRange(new Control[]
                { _moodLabel, border, sub, name, avatar });
        }



        private void BuildSidebar()
        {
            _sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = C_SidebarBg
            };

            // Logo for the Chatbot
            var logo = new Label
            {
                Text = "⬡ CYBER\n    BOT",
                Dock = DockStyle.Top,
                Height = 68,
                ForeColor = C_Purple,
                Font = new Font("Segoe UI Black", 15f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(16, 12, 32)
            };

            // User info card
            _userCard = new Label
            {
                Text = "Welcome!\nStart chatting below.",
                Dock = DockStyle.Top,
                Height = 54,
                ForeColor = C_TextMid,
                Font = new Font("Segoe UI", 8.5f),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(26, 20, 50),
                Padding = new Padding(6)
            };

            var sep1 = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = C_Border };

            var heading = new Label
            {
                Text = "  QUICK TOPICS",
                Dock = DockStyle.Top,
                Height = 26,
                ForeColor = C_TextLo,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

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
            {
                var btn = new Button
                {
                    Text = "  " + topic,
                    Size = new Size(182, 30),
                    BackColor = Color.FromArgb(30, 24, 56),
                    ForeColor = C_TextMid,
                    Font = new Font("Segoe UI", 9f),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Margin = new Padding(0, 2, 0, 2),
                    Tag = topic
                };
                btn.FlatAppearance.BorderColor = C_Border;
                btn.FlatAppearance.BorderSize = 1;
                btn.MouseEnter += (s, e) => { btn.BackColor = C_BtnHover; btn.ForeColor = C_Violet; };
                btn.MouseLeave += (s, e) => { btn.BackColor = Color.FromArgb(30, 24, 56); btn.ForeColor = C_TextMid; };
                btn.Click += TopicBtn_Click;
                _topicBtns.Controls.Add(btn);
            }

            _sidebar.Controls.Add(_topicBtns);
            _sidebar.Controls.Add(heading);
            _sidebar.Controls.Add(sep1);
            _sidebar.Controls.Add(_userCard);
            _sidebar.Controls.Add(logo);
        }

        

        private void BuildMainArea()
        {
            _mainArea = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = C_PageBg,
                Padding = new Padding(0)
            };

            _chatScroll = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = C_PageBg,
                AutoScroll = true,
                Padding = new Padding(14, 10, 14, 10)
            };

            _messages = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = C_PageBg,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0)
            };

            _chatScroll.Controls.Add(_messages);
            _mainArea.Controls.Add(_chatScroll);
        }

        //Input Bar 

        private void BuildInputBar()
        {
            _inputBar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = C_InputBg,
                Padding = new Padding(14, 10, 14, 10)
            };

            var topLine = new Panel
            { Dock = DockStyle.Top, Height = 1, BackColor = C_Border };

            _sendBtn = new Button
            {
                Text = "Send  ➤",
                Dock = DockStyle.Right,
                Width = 96,
                BackColor = C_Purple,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _sendBtn.FlatAppearance.BorderSize = 0;
            _sendBtn.FlatAppearance.MouseOverBackColor = C_Violet;
            _sendBtn.Click += SendBtn_Click;

            _inputBox = new TextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 24, 56),
                ForeColor = C_TextHi,
                Font = new Font("Segoe UI", 11f),
                BorderStyle = BorderStyle.None,
                PlaceholderText = "Type a message or pick a topic...",
            };
            _inputBox.KeyDown += InputBox_KeyDown;

            _inputBar.Controls.Add(_inputBox);
            _inputBar.Controls.Add(_sendBtn);
            _inputBar.Controls.Add(topLine);
        }

        // Event Handlers

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

        // Processing 

        private void ProcessInput()
        {
            string raw = _inputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(raw)) return;
            _inputBox.Clear();

            if (raw.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                raw.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                AddUserBubble(raw);
                AddBotBubble($"Take care, {_memory.Name}! Stay safe online.");
                return;
            }

            AddUserBubble(raw);

            // Name collection
            if (!_nameCollected)
            {
                _memory.SetName(raw);
                _nameCollected = true;
                RefreshUserCard();
                AddBotBubble(
                    $"Great to meet you, {_memory.Name}!\n\n" +
                    "Click any topic on the left to get started, or just type your question.\n" +
                    "I cover passwords, phishing, malware, ransomware, 2FA, encryption and more.");
                return;
            }

            // Follow-up
            if (IsFollowUp(raw)) { HandleFollowUp(); return; }

            // Sentiment
            SentimentResult mood = _detector.Detect(raw);
            RefreshMoodLabel(mood);
            string prefix = SentimentPrefix(mood);

            // Response
            string reply = _engine.GetResponse(raw, _memory.Name, _memory);

            string? topic = _engine.LastDetectedTopic;
            if (topic != null)
            {
                _lastTopic = topic;
                _memory.SetFavouriteTopic(topic);
                RefreshUserCard();
            }

            AddBotBubble(string.IsNullOrEmpty(prefix) ? reply : prefix + "\n\n" + reply);
        }

        // Conversation Flow 

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

        //  Sentiment 

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
                : $"Mood: {s.Label}";
        }

        // Sidebar refresh 

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
                ForeColor = Color.FromArgb(200, 170, 255),
                BackColor = Color.Transparent,
                AutoSize = true,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };

            var msgLabel = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = Color.White,
                BackColor = C_UserBubble,
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
                ForeColor = C_Mint,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(6, 0)
            };

            // Accent bar
            var accent = new Panel
            {
                BackColor = C_Purple,
                Width = 3,
                Location = new Point(0, 0)
            };

            var msgLabel = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = C_TextHi,
                BackColor = C_BotBubble,
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
