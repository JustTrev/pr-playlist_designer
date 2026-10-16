using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms; 


namespace FrmReclaimerPlaylist
{
    public partial class FrmGameTypeEditor : Form
    {
        public FrmGameTypeEditor()
        {
            InitializeComponent();
        }

        private void FrmGameTypeEditor_Load(object sender, EventArgs e)
        {

        }


        private string currentFilePath = null;
        private JObject root;

        // Controls
        private ComboBox cmbTemplate;
        private TextBox txtName;
        private NumericUpDown numScoreToWin, numKillPoints, numDeathPoints, numSuicidePoints, numBetrayalPoints;
        private NumericUpDown numTeams, numMinutes, numVehicles, numPickup, numGrenades, numAmmo;
        private TextBox txtPrimaryWeapon, txtSecondaryWeapon, txtMapWeapons;
        private TextBox txtScript;          // Big multi-line editor for the entire "script" object
        private Button btnSave, btnSaveAs, btnClose;
        private Label lblStatus;

        private ListBox lstIntelliSense;
        private string[] intelliSenseWords;
        private int intelliSenseStart = -1;
        private bool suspendTemplateEvent = false;


        public FrmGameTypeEditor(string filePath = null, bool isDarkMode = false)
        {
            currentFilePath = filePath;

            this.Text = "Game Type Editor";
            this.Size = new Size(1100, 780);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(900, 650);

            BuildUI();               // ← creates the ComboBox + base items
            ApplyTheme(isDarkMode);

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                LoadFromFile(filePath);   // ← adds "Current File: xxx"
            else
                LoadTemplate("Blank");
        }

        public void ApplyTheme(bool isDarkMode)
        {
            if (isDarkMode)
            {
                this.BackColor = Color.FromArgb(45, 45, 48);
                this.ForeColor = Color.WhiteSmoke;
            }
            else
            {
                this.BackColor = SystemColors.Control;
                this.ForeColor = Color.Black;
            }

            foreach (Control control in this.Controls)
            {
                ApplyThemeToControl(control, isDarkMode);
            }
        }

        private void CmbTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suspendTemplateEvent || cmbTemplate.SelectedItem == null)
                return;

            string selected = cmbTemplate.SelectedItem.ToString();

            if (selected.StartsWith("Current File:"))
            {
                if (!string.IsNullOrEmpty(currentFilePath) && File.Exists(currentFilePath))
                    LoadFromFile(currentFilePath);
                return;
            }

            LoadTemplate(selected);
        }

        private void ApplyThemeToControl(Control control, bool isDarkMode)
        {
            if (isDarkMode)
            {
                control.BackColor = Color.FromArgb(45, 45, 48);
                control.ForeColor = Color.WhiteSmoke;

                if (control is ListBox || control is TextBox || control is NumericUpDown || control is ComboBox)
                {
                    control.BackColor = Color.FromArgb(30, 30, 30);
                    control.ForeColor = Color.WhiteSmoke;
                }
            }
            else
            {
                control.BackColor = SystemColors.Control;
                control.ForeColor = Color.Black;

                if (control is ListBox || control is TextBox || control is NumericUpDown || control is ComboBox)
                {
                    control.BackColor = SystemColors.Window;
                    control.ForeColor = SystemColors.WindowText;
                }
            }

            if (control.HasChildren)
            {
                foreach (Control child in control.Controls)
                {
                    ApplyThemeToControl(child, isDarkMode);
                }
            }
        }

        private void BuildUI()
        {
            this.SuspendLayout();

            // ========== TOP BAR ==========
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10, 10, 10, 5)
            };

            topPanel.Controls.Add(new Label { Text = "Template:", Location = new Point(10, 14), AutoSize = true });

            cmbTemplate = new ComboBox
            {
                Location = new Point(80, 11),
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Always add the base templates
            cmbTemplate.Items.AddRange(new object[]
            {
    "Blank",
    "Zone Control",
    "Gun Game",
    "Infection (Flood)",
    "Growth",
    "One in the Chamber",
    "Invasion (UNSC vs Covenant)"
            });

            cmbTemplate.SelectedIndexChanged += CmbTemplate_SelectedIndexChanged;
            topPanel.Controls.Add(cmbTemplate);

            btnSave = new Button { Text = "Save", Location = new Point(380, 9), Size = new Size(85, 30) };
            btnSaveAs = new Button { Text = "Save As…", Location = new Point(475, 9), Size = new Size(90, 30) };
            btnClose = new Button { Text = "Close", Location = new Point(575, 9), Size = new Size(85, 30) };

            btnSave.Click += BtnSave_Click;
            btnSaveAs.Click += BtnSaveAs_Click;
            btnClose.Click += (s, e) => this.Close();

            topPanel.Controls.Add(btnSave);
            topPanel.Controls.Add(btnSaveAs);
            topPanel.Controls.Add(btnClose);

            // ========== STATUS BAR ==========
            lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 24,
                Text = "Ready",
                BorderStyle = BorderStyle.Fixed3D,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };

            // ========== MAIN CONTENT PANEL ==========
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12, 8, 12, 8)
            };

            // --- Fixed top section (Name + Rules + Options) ---
            var topContent = new Panel
            {
                Dock = DockStyle.Top,
                Height = 210
            };

            int y = 6;
            int lx = 8;
            int fx = 140;

            // Name
            topContent.Controls.Add(new Label { Text = "Name:", Location = new Point(lx, y + 3), AutoSize = true });
            txtName = new TextBox { Location = new Point(fx, y), Width = 520 };
            topContent.Controls.Add(txtName);
            y += 34;

            // Rules header
            topContent.Controls.Add(new Label { Text = "Rules", Location = new Point(lx, y), Font = new Font(Font, FontStyle.Bold), AutoSize = true });
            y += 24;

            topContent.Controls.Add(new Label { Text = "Score to Win:", Location = new Point(lx, y + 3), AutoSize = true });
            numScoreToWin = new NumericUpDown { Location = new Point(110, y), Width = 65, Minimum = -1, Maximum = 999 };
            topContent.Controls.Add(numScoreToWin);

            topContent.Controls.Add(new Label { Text = "Kill Pts:", Location = new Point(195, y + 3), AutoSize = true });
            numKillPoints = new NumericUpDown { Location = new Point(250, y), Width = 55, Minimum = -10, Maximum = 10 };
            topContent.Controls.Add(numKillPoints);

            topContent.Controls.Add(new Label { Text = "Death Pts:", Location = new Point(325, y + 3), AutoSize = true });
            numDeathPoints = new NumericUpDown { Location = new Point(395, y), Width = 55, Minimum = -10, Maximum = 10 };
            topContent.Controls.Add(numDeathPoints);

            topContent.Controls.Add(new Label { Text = "Suicide:", Location = new Point(470, y + 3), AutoSize = true });
            numSuicidePoints = new NumericUpDown { Location = new Point(530, y), Width = 55, Minimum = -10, Maximum = 10 };
            topContent.Controls.Add(numSuicidePoints);

            topContent.Controls.Add(new Label { Text = "Betrayal:", Location = new Point(605, y + 3), AutoSize = true });
            numBetrayalPoints = new NumericUpDown { Location = new Point(665, y), Width = 55, Minimum = -10, Maximum = 10 };
            topContent.Controls.Add(numBetrayalPoints);
            y += 36;

            // Common Options header
            topContent.Controls.Add(new Label { Text = "Common Options", Location = new Point(lx, y), Font = new Font(Font, FontStyle.Bold), AutoSize = true });
            y += 24;

            topContent.Controls.Add(new Label { Text = "Teams (0/1):", Location = new Point(lx, y + 3), AutoSize = true });
            numTeams = new NumericUpDown { Location = new Point(110, y), Width = 50, Minimum = 0, Maximum = 1 };
            topContent.Controls.Add(numTeams);

            topContent.Controls.Add(new Label { Text = "Minutes:", Location = new Point(180, y + 3), AutoSize = true });
            numMinutes = new NumericUpDown { Location = new Point(245, y), Width = 55, Minimum = 0, Maximum = 60 };
            topContent.Controls.Add(numMinutes);

            topContent.Controls.Add(new Label { Text = "Vehicles:", Location = new Point(320, y + 3), AutoSize = true });
            numVehicles = new NumericUpDown { Location = new Point(385, y), Width = 50, Minimum = 0, Maximum = 2 };
            topContent.Controls.Add(numVehicles);
            y += 32;

            topContent.Controls.Add(new Label { Text = "Primary Weapon:", Location = new Point(lx, y + 3), AutoSize = true });
            txtPrimaryWeapon = new TextBox { Location = new Point(130, y), Width = 180 };
            topContent.Controls.Add(txtPrimaryWeapon);

            topContent.Controls.Add(new Label { Text = "Secondary:", Location = new Point(330, y + 3), AutoSize = true });
            txtSecondaryWeapon = new TextBox { Location = new Point(410, y), Width = 160 };
            topContent.Controls.Add(txtSecondaryWeapon);
            y += 32;

            topContent.Controls.Add(new Label { Text = "Map Weapons:", Location = new Point(lx, y + 3), AutoSize = true });
            txtMapWeapons = new TextBox { Location = new Point(130, y), Width = 180 };
            topContent.Controls.Add(txtMapWeapons);

            topContent.Controls.Add(new Label { Text = "Pickup / Grenades / Ammo:", Location = new Point(330, y + 3), AutoSize = true });
            numPickup = new NumericUpDown { Location = new Point(500, y), Width = 45, Minimum = 0, Maximum = 5 };
            numGrenades = new NumericUpDown { Location = new Point(555, y), Width = 45, Minimum = 0, Maximum = 5 };
            numAmmo = new NumericUpDown { Location = new Point(610, y), Width = 45, Minimum = 0, Maximum = 5 };
            topContent.Controls.Add(numPickup);
            topContent.Controls.Add(numGrenades);
            topContent.Controls.Add(numAmmo);

            // --- Script section (fills remaining space) ---
            var scriptLabel = new Label
            {
                Text = "Script (full JSON – Megalo, Traits, Deployment, Objects, etc.)",
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font(Font, FontStyle.Bold),
                Padding = new Padding(0, 4, 0, 0)
            };

            txtScript = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 9.5f),
                WordWrap = false,
                AcceptsTab = true,
                AcceptsReturn = true
            };

            // Assemble main panel (order is important for docking)
            mainPanel.Controls.Add(txtScript);      // Fill
            mainPanel.Controls.Add(scriptLabel);    // Top of remaining
            mainPanel.Controls.Add(topContent);     // Top

            // Final form layout
            this.Controls.Add(mainPanel);
            this.Controls.Add(topPanel);
            this.Controls.Add(lblStatus);

            this.ResumeLayout(true);

            SetupIntelliSense();
        }


        private void SetupIntelliSense()
        {
            // Common keywords collected from all your templates
            intelliSenseWords = new[]
            {
        // Top-level
        "version", "name", "mode", "rules", "options", "script", "map",

        // Rules
        "score_to_win", "kill_points", "death_points", "suicide_points", "betrayal_points",

        // Options
        "rounds.teams", "rounds.limit", "rounds.minutes", "rounds.reset_players",
        "map.weapons", "map.vehicles",
        "player.primary_weapon", "player.secondary_weapon", "player.pickup",
        "player.grenades", "player.ammo", "player.shields",
        "respawn.lives", "respawn.seconds",

        // Script structure
        "initial", "on_spawn", "on_kill", "on_death", "on_tick", "logic",

        // Logic / Ops
        "op", "condition", "then", "actions", "value", "slot", "index", "choices",
        "when", "enabled",

        // Common ops
        "weapon", "ammo", "lethal", "add_ammo", "score", "set", "scale",
        "health", "damage", "if", "for_each_player", "with_object",
        "set_global", "set_object", "move_object", "spawn_object",

        // Megalo / Traits
        "megalo_traits", "megalo", "resistance", "shields", "speed", "gravity",
        "pickup", "vehicles", "radar", "headshots",
        "apply_traits", "script_traits", "script_widget", "current_player",

        // Deployment
        "deployment", "title", "teams", "classes", "script_menu",
        "name", "description", "team", "cost",

        // Objects
        "objects", "source", "kind", "zone", "weapon", "label", "shape",
        "sphere", "radius", "hill", "supply",

        // Values / Misc
        "global", "self", "true", "false", "null"
    }.Distinct().OrderBy(w => w).ToArray();

            lstIntelliSense = new ListBox
            {
                Visible = false,
                Font = new Font("Consolas", 9.5f),
                Width = 280,
                Height = 180,
                BorderStyle = BorderStyle.FixedSingle
            };
            lstIntelliSense.Click += LstIntelliSense_Click;
            lstIntelliSense.KeyDown += LstIntelliSense_KeyDown;

            // Important: add to the form so it can float above everything
            this.Controls.Add(lstIntelliSense);
            lstIntelliSense.BringToFront();

            txtScript.KeyDown += TxtScript_KeyDown;
            txtScript.KeyUp += TxtScript_KeyUp;
        }

        private void TxtScript_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Space)
            {
                ShowIntelliSense();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (lstIntelliSense.Visible)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    HideIntelliSense();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    InsertSelectedIntelliSense();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    // Let the ListBox handle up/down
                    if (e.KeyCode == Keys.Up && lstIntelliSense.SelectedIndex > 0)
                        lstIntelliSense.SelectedIndex--;
                    else if (e.KeyCode == Keys.Down && lstIntelliSense.SelectedIndex < lstIntelliSense.Items.Count - 1)
                        lstIntelliSense.SelectedIndex++;
                    e.Handled = true;
                }
            }
        }

        private void TxtScript_KeyUp(object sender, KeyEventArgs e)
        {
            // Auto-show after typing 2+ characters of a word
            if (!lstIntelliSense.Visible && e.KeyCode != Keys.Space && e.KeyCode != Keys.Return)
            {
                string currentWord = GetCurrentWord();
                if (currentWord.Length >= 2)
                    ShowIntelliSense(currentWord);
            }
            else if (lstIntelliSense.Visible)
            {
                string currentWord = GetCurrentWord();
                FilterIntelliSense(currentWord);

                if (lstIntelliSense.Items.Count == 0)
                    HideIntelliSense();
            }
        }

        private void LstIntelliSense_Click(object sender, EventArgs e)
        {
            InsertSelectedIntelliSense();
        }

        private void LstIntelliSense_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                InsertSelectedIntelliSense();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                HideIntelliSense();
                e.Handled = true;
            }
        }
        private string GetCurrentWord()
        {
            int pos = txtScript.SelectionStart;
            if (pos == 0) return "";

            int start = pos - 1;
            while (start >= 0)
            {
                char c = txtScript.Text[start];
                if (!char.IsLetterOrDigit(c) && c != '.' && c != '_')
                    break;
                start--;
            }
            start++;

            int length = pos - start;
            if (length <= 0) return "";

            return txtScript.Text.Substring(start, length);
        }

        private void ShowIntelliSense(string filter = null)
        {
            if (filter == null)
                filter = GetCurrentWord();

            FilterIntelliSense(filter);

            if (lstIntelliSense.Items.Count == 0)
            {
                HideIntelliSense();
                return;
            }

            // Position the list near the caret
            Point p = txtScript.GetPositionFromCharIndex(txtScript.SelectionStart);
            p = txtScript.PointToScreen(p);
            p = this.PointToClient(p);

            lstIntelliSense.Location = new Point(p.X + 10, p.Y + 20);
            lstIntelliSense.Visible = true;
            lstIntelliSense.BringToFront();

            if (lstIntelliSense.Items.Count > 0)
                lstIntelliSense.SelectedIndex = 0;

            intelliSenseStart = txtScript.SelectionStart - filter.Length;
        }

        private void FilterIntelliSense(string filter)
        {
            lstIntelliSense.Items.Clear();

            if (string.IsNullOrEmpty(filter))
            {
                lstIntelliSense.Items.AddRange(intelliSenseWords);
            }
            else
            {
                var matches = intelliSenseWords
                    .Where(w => w.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                lstIntelliSense.Items.AddRange(matches);
            }
        }

        private void InsertSelectedIntelliSense()
        {
            if (lstIntelliSense.SelectedItem == null) return;

            string word = lstIntelliSense.SelectedItem.ToString();
            string current = GetCurrentWord();

            int start = txtScript.SelectionStart - current.Length;
            txtScript.Select(start, current.Length);
            txtScript.SelectedText = word;

            HideIntelliSense();
            txtScript.Focus();
        }

        private void HideIntelliSense()
        {
            lstIntelliSense.Visible = false;
            intelliSenseStart = -1;
        }



        private void LoadTemplate(string name)
        {
            string json = GetTemplateJson(name);
            root = JObject.Parse(json);
            PopulateFromRoot();
            currentFilePath = null;
            lblStatus.Text = $"Loaded template: {name}";
        }

        private void LoadFromFile(string path)
        {
            try
            {
                string json = File.ReadAllText(path);
                root = JObject.Parse(json);
                PopulateFromRoot();

                currentFilePath = path;
                this.Text = "Game Type Editor – " + Path.GetFileName(path);
                lblStatus.Text = "Loaded: " + Path.GetFileName(path);

                string currentItem = "Current File: " + Path.GetFileName(path);

                suspendTemplateEvent = true;

                // Remove any existing "Current File:" entry
                for (int i = cmbTemplate.Items.Count - 1; i >= 0; i--)
                {
                    if (cmbTemplate.Items[i].ToString().StartsWith("Current File:"))
                        cmbTemplate.Items.RemoveAt(i);
                }

                // Make sure base templates still exist (safety)
                if (cmbTemplate.Items.Count == 0)
                {
                    cmbTemplate.Items.AddRange(new object[]
                    {
                "Blank",
                "Zone Control",
                "Gun Game",
                "Infection (Flood)",
                "Growth",
                "One in the Chamber",
                "Invasion (UNSC vs Covenant)"
                    });
                }

                // Insert Current File at the top
                cmbTemplate.Items.Insert(0, currentItem);
                cmbTemplate.SelectedItem = currentItem;

                suspendTemplateEvent = false;
            }
            catch (Exception ex)
            {
                suspendTemplateEvent = false;
                MessageBox.Show("Failed to load file:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFromRoot()
        {
            txtName.Text = root["name"]?.ToString() ?? "";

            var rules = root["rules"] as JObject;
            if (rules != null)
            {
                numScoreToWin.Value = rules["score_to_win"]?.Value<int>() ?? 0;
                numKillPoints.Value = rules["kill_points"]?.Value<int>() ?? 0;
                numDeathPoints.Value = rules["death_points"]?.Value<int>() ?? 0;
                numSuicidePoints.Value = rules["suicide_points"]?.Value<int>() ?? 0;
                numBetrayalPoints.Value = rules["betrayal_points"]?.Value<int>() ?? 0;
            }

            var options = root["options"] as JObject;
            if (options != null)
            {
                numTeams.Value = options["rounds.teams"]?.Value<int>() ?? 0;
                numMinutes.Value = options["rounds.minutes"]?.Value<int>() ?? 0;
                numVehicles.Value = options["map.vehicles"]?.Value<int>() ?? 0;
                numPickup.Value = options["player.pickup"]?.Value<int>() ?? 0;
                numGrenades.Value = options["player.grenades"]?.Value<int>() ?? 0;
                numAmmo.Value = options["player.ammo"]?.Value<int>() ?? 0;

                txtPrimaryWeapon.Text = options["player.primary_weapon"]?.ToString() ?? "";
                txtSecondaryWeapon.Text = options["player.secondary_weapon"]?.ToString() ?? "";
                txtMapWeapons.Text = options["map.weapons"]?.ToString() ?? "";
            }

            // Full script section
            var script = root["script"];
            if (script != null)
                txtScript.Text = script.ToString(Formatting.Indented);
            else
                txtScript.Text = "{\n  \"version\": 1,\n  \"name\": \"\",\n  \"initial\": []\n}";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                BtnSaveAs_Click(sender, e);
                return;
            }
            SaveToFile(currentFilePath);
        }

        private void BtnSaveAs_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Game Type JSON (*.json)|*.json";
                sfd.FileName = (txtName.Text.Trim() + ".json").Replace(" ", "_");
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    currentFilePath = sfd.FileName;
                    SaveToFile(currentFilePath);
                    this.Text = "Game Type Editor – " + Path.GetFileName(currentFilePath);
                }
            }
        }

        private void SaveToFile(string path)
        {
            try
            {
                // Update common fields
                root["name"] = txtName.Text.Trim();
                root["version"] = 1;
                root["mode"] = "Slayer";

                var rules = new JObject
                {
                    ["score_to_win"] = (int)numScoreToWin.Value,
                    ["kill_points"] = (int)numKillPoints.Value,
                    ["death_points"] = (int)numDeathPoints.Value,
                    ["suicide_points"] = (int)numSuicidePoints.Value,
                    ["betrayal_points"] = (int)numBetrayalPoints.Value
                };
                root["rules"] = rules;

                var options = root["options"] as JObject ?? new JObject();
                options["rounds.teams"] = (int)numTeams.Value;
                options["rounds.minutes"] = (int)numMinutes.Value;
                options["map.vehicles"] = (int)numVehicles.Value;
                options["player.pickup"] = (int)numPickup.Value;
                options["player.grenades"] = (int)numGrenades.Value;
                options["player.ammo"] = (int)numAmmo.Value;

                if (!string.IsNullOrWhiteSpace(txtPrimaryWeapon.Text))
                    options["player.primary_weapon"] = TryParseIntOrString(txtPrimaryWeapon.Text);
                if (!string.IsNullOrWhiteSpace(txtSecondaryWeapon.Text))
                    options["player.secondary_weapon"] = TryParseIntOrString(txtSecondaryWeapon.Text);
                if (!string.IsNullOrWhiteSpace(txtMapWeapons.Text))
                    options["map.weapons"] = TryParseIntOrString(txtMapWeapons.Text);

                root["options"] = options;

                // Script – parse what the user typed
                try
                {
                    root["script"] = JToken.Parse(txtScript.Text);
                }
                catch
                {
                    MessageBox.Show("The Script JSON is invalid. Please fix it before saving.", "Invalid Script", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                File.WriteAllText(path, root.ToString(Formatting.Indented));
                lblStatus.Text = "Saved: " + Path.GetFileName(path);
                MessageBox.Show("Game type saved successfully!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private JToken TryParseIntOrString(string text)
        {
            if (int.TryParse(text.Trim(), out int n))
                return n;
            return text.Trim();
        }

        // ==================== TEMPLATES ====================
        private string GetTemplateJson(string name)
        {
            switch (name)
            {
                case "Zone Control":
                    return @"{
  ""version"": 1,
  ""name"": ""Zone Control"",
  ""mode"": ""Slayer"",
  ""rules"": {""score_to_win"": 30, ""kill_points"": 0, ""death_points"": 0, ""suicide_points"": 0, ""betrayal_points"": 0},
  ""options"": {""rounds.teams"": 0, ""rounds.limit"": 1, ""rounds.minutes"": 10},
  ""script"": {
    ""version"": 2,
    ""name"": ""Zone Control"",
    ""initial"": [0],
    ""on_spawn"": [],
    ""logic"": {
      ""options"": {""points_per_second"": {""value"": 1, ""min"": 1, ""max"": 10}},
      ""objects"": {
        ""hill"": {""source"": {""kind"": ""zone""}, ""label"": ""objective"", ""shape"": {""kind"": ""sphere"", ""radius"": 8}}
      }
    }
  }
}";

                case "Gun Game":
                    return @"{
  ""version"": 1,
  ""name"": ""Gun Game"",
  ""mode"": ""Slayer"",
  ""rules"": {""score_to_win"": 24, ""kill_points"": 0, ""death_points"": 0, ""suicide_points"": 0, ""betrayal_points"": 0},
  ""options"": {
    ""rounds.teams"": 0, ""rounds.reset_players"": 1, ""rounds.limit"": 3, ""rounds.minutes"": 0,
    ""map.weapons"": 11, ""map.vehicles"": 1,
    ""player.primary_weapon"": 16, ""player.secondary_weapon"": -1,
    ""player.pickup"": 2, ""player.grenades"": 2, ""player.ammo"": 2
  },
  ""script"": {
    ""version"": 1,
    ""name"": ""Gun Game"",
    ""initial"": [0],
    ""on_spawn"": [{""op"": ""weapon"", ""index"": [""self.0""], ""choices"": [""spartan_laser"", ""rocket_launcher"", ""shotgun"", ""sniper_rifle"", ""battle_rifle"", ""assault_rifle"", ""magnum"", ""energy_sword""]}],
    ""on_kill"": [
      {""op"": ""score"", ""when"": [""kill.melee"", 0, ""==""], ""value"": [1]},
      {""op"": ""set"", ""slot"": 0, ""when"": [""kill.melee"", 0, ""==""], ""value"": [""self.0"", 1, ""+"", 23, ""min""]},
      {""op"": ""weapon"", ""index"": [""self.0""], ""choices"": [""spartan_laser"", ""rocket_launcher"", ""shotgun"", ""sniper_rifle"", ""battle_rifle"", ""assault_rifle"", ""magnum"", ""energy_sword""]}
    ]
  }
}";

                case "Infection (Flood)":
                    // Using a shortened but valid structure – user can paste full Megalo later
                    return @"{
  ""version"": 1,
  ""name"": ""Flood Infection"",
  ""mode"": ""Slayer"",
  ""rules"": {""score_to_win"": 0, ""kill_points"": 1, ""death_points"": 0, ""suicide_points"": 0, ""betrayal_points"": -1},
  ""options"": {
    ""rounds.teams"": 1, ""rounds.limit"": 3, ""rounds.minutes"": 0,
    ""player.primary_weapon"": 11, ""player.secondary_weapon"": 7, ""player.grenades"": 2
  },
  ""script"": {
    ""version"": 2,
    ""name"": ""Flood Infection"",
    ""initial"": [],
    ""logic"": {
      ""options"": {
        ""initial_infected"": {""value"": 1, ""min"": 1, ""max"": 8},
        ""round_seconds"": {""value"": 180, ""min"": 30, ""max"": 900},
        ""warmup_seconds"": {""value"": 10, ""min"": 0, ""max"": 60}
      },
      ""megalo_traits"": [
        {""resistance"": 4, ""shields"": 1, ""speed"": 5, ""gravity"": 3, ""damage"": 6, ""pickup"": 1, ""vehicles"": 1, ""radar"": 2},
        {""resistance"": 4, ""shields"": 1, ""speed"": 9, ""gravity"": 3, ""damage"": 6, ""pickup"": 2, ""vehicles"": 1, ""radar"": 3, ""headshots"": 2}
      ],
      ""megalo"": ""-- Paste full Megalo source here""
    }
  }
}";

                case "Growth":
                    return @"{
  ""version"": 1,
  ""name"": ""Growth"",
  ""mode"": ""Slayer"",
  ""rules"": {""score_to_win"": 0, ""kill_points"": 1, ""death_points"": 0, ""suicide_points"": 0, ""betrayal_points"": 0},
  ""options"": {
    ""rounds.teams"": 0, ""rounds.reset_players"": 1, ""rounds.limit"": 2, ""rounds.minutes"": 10,
    ""map.weapons"": 0, ""map.vehicles"": 1,
    ""player.primary_weapon"": -3, ""player.secondary_weapon"": -3,
    ""player.pickup"": 1, ""player.grenades"": 1, ""player.ammo"": 1
  },
  ""script"": {
    ""version"": 1,
    ""name"": ""Growth"",
    ""initial"": [0.1],
    ""on_spawn"": [
      {""op"": ""scale"", ""value"": [""self.0""]},
      {""op"": ""health"", ""value"": [""self.0""]},
      {""op"": ""damage"", ""value"": [""self.0""]}
    ],
    ""on_kill"": [
      {""op"": ""set"", ""slot"": 0, ""value"": [""self.0"", ""victim.0"", ""+"", 3, ""min""]},
      {""op"": ""scale"", ""value"": [""self.0""]},
      {""op"": ""health"", ""value"": [""self.0""]},
      {""op"": ""damage"", ""value"": [""self.0""]}
    ],
    ""on_death"": [
      {""op"": ""set"", ""slot"": 0, ""value"": [0.1]},
      {""op"": ""scale"", ""value"": [0.1]},
      {""op"": ""health"", ""value"": [0.1]},
      {""op"": ""damage"", ""value"": [0.1]}
    ]
  }
}";

                case "One in the Chamber":
                    return @"{
  ""version"": 1,
  ""name"": ""One in the Chamber"",
  ""mode"": ""Slayer"",
  ""rules"": {""score_to_win"": 25, ""kill_points"": 1, ""death_points"": 0, ""suicide_points"": 0, ""betrayal_points"": 0},
  ""options"": {
    ""rounds.teams"": 0, ""rounds.limit"": 1, ""rounds.minutes"": 10,
    ""map.weapons"": 11, ""map.vehicles"": 1,
    ""player.primary_weapon"": 7, ""player.secondary_weapon"": -1,
    ""player.pickup"": 2, ""player.grenades"": 2, ""player.ammo"": 1,
    ""respawn.lives"": 3, ""player.shields"": 1
  },
  ""script"": {
    ""version"": 1,
    ""name"": ""One in the Chamber"",
    ""initial"": [],
    ""on_spawn"": [
      {""op"": ""weapon"", ""index"": [0], ""choices"": [""magnum""]},
      {""op"": ""ammo"", ""value"": [1]},
      {""op"": ""lethal"", ""enabled"": true}
    ],
    ""on_kill"": [
      {""op"": ""add_ammo"", ""value"": [1]}
    ]
  }
}";

                case "Invasion (UNSC vs Covenant)":
                    return @"{
  ""version"": 1,
  ""name"": ""UNSC vs Covenant"",
  ""mode"": ""Slayer"",
  ""rules"": {""score_to_win"": 0, ""kill_points"": 1, ""death_points"": 0, ""suicide_points"": 0, ""betrayal_points"": -1},
  ""options"": {
    ""rounds.teams"": 1, ""rounds.limit"": 1, ""rounds.minutes"": 0,
    ""player.grenades"": 2, ""player.primary_weapon"": 1, ""player.secondary_weapon"": -1,
    ""respawn.seconds"": 5
  },
  ""script"": {
    ""version"": 2,
    ""name"": ""UNSC vs Covenant"",
    ""initial"": [],
    ""logic"": {
      ""deployment"": {
        ""title"": ""UNSC vs Covenant"",
        ""teams"": [""UNSC"", ""Covenant""],
        ""classes"": [],
        ""script_menu"": true
      },
      ""options"": {
        ""points_per_kill"": {""value"": 50, ""min"": 1, ""max"": 1000},
        ""reinforcements"": {""value"": 150, ""min"": 10, ""max"": 1000},
        ""round_seconds"": {""value"": 600, ""min"": 60, ""max"": 3600}
      },
      ""megalo"": ""-- Paste full Invasion Megalo here""
    }
  }
}";

                default: // Blank
                    return @"{
  ""version"": 1,
  ""name"": ""New Game Type"",
  ""mode"": ""Slayer"",
  ""rules"": {""score_to_win"": 50, ""kill_points"": 1, ""death_points"": 0, ""suicide_points"": -1, ""betrayal_points"": -1},
  ""options"": {
    ""rounds.teams"": 1, ""rounds.limit"": 1, ""rounds.minutes"": 10,
    ""player.primary_weapon"": ""Assault Rifle"", ""player.secondary_weapon"": ""Magnum"",
    ""player.pickup"": 1, ""player.grenades"": 2, ""player.ammo"": 2
  },
  ""script"": {
    ""version"": 1,
    ""name"": ""New Game Type"",
    ""initial"": []
  }
}";
            }
        }
    }
}
