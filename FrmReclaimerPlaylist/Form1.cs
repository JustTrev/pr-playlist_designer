using System.Xml;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FrmReclaimerPlaylist
{
    public partial class FrmReclaimerPlaylist : Form
    {
        private Playlist currentPlaylist = new Playlist();
        private PlaylistGame editingGame = null;
        private bool isNewGame = false;
        private bool isDirty = false;

        // Controls
        private TextBox txtPlaylistName;
        private NumericUpDown numVoteSeconds;
        private Button btnNew, btnLoad, btnSave, btnAbout;
        private ListBox lstGames;
        private Button btnAddGame, btnRemoveGame, btnDuplicateGame;
        private GroupBox grpEditor;
        private TextBox txtMap, txtGameName, txtGameType, txtPreviewImage;
        private Button btnBrowseMap, btnBrowseGameType, btnBrowsePreviewImage;
        private CheckBox chkUseWeight;
        private NumericUpDown numWeight;
        private ComboBox cmbSprint;
        private NumericUpDown numScoreToWin, numKillPoints, numDeathPoints, numSuicidePoints, numBetrayalPoints;
        private TextBox txtPrimaryWeapon, txtSecondaryWeapon, txtMapWeapons;
        private NumericUpDown numTeams, numMinutes, numVehicles, numPickup, numGrenades, numAmmo;
        private Button btnSaveGame, btnCancelEdit;
        private Label lblStatus;
        private CheckBox cbxDarkMode;
        private Button btnEditGameType;
        private ToolTip toolTip;

        public FrmReclaimerPlaylist()
        {
            InitializeComponent();
        }

        private void BuildUI()
        {
            this.SuspendLayout();

            // ========== TOP BAR ==========
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                Padding = new Padding(8, 8, 8, 5)
            };

            var lblName = new Label { Text = "Playlist Name:", AutoSize = true, Location = new Point(10, 14) };
            txtPlaylistName = new TextBox { Location = new Point(115, 11), Width = 260, Text = currentPlaylist.Name };

            var lblVote = new Label { Text = "Vote Seconds:", AutoSize = true, Location = new Point(395, 14) };
            numVoteSeconds = new NumericUpDown
            {
                Location = new Point(495, 11),
                Width = 70,
                Minimum = -1,
                Maximum = 1000,
                Value = 20
            };

            btnNew = new Button { Text = "New", Location = new Point(590, 9), Size = new Size(75, 28) };
            btnLoad = new Button { Text = "Load", Location = new Point(675, 9), Size = new Size(75, 28) };
            btnSave = new Button { Text = "Save Playlist", Location = new Point(760, 9), Size = new Size(110, 28) };

            btnNew.Click += BtnNew_Click;
            btnLoad.Click += BtnLoad_Click;
            btnSave.Click += BtnSave_Click;

            btnAbout = new Button
            {
                Text = "About",
                Location = new Point(880, 9),
                Size = new Size(75, 28)
            };
            btnAbout.Click += BtnAbout_Click;
            topPanel.Controls.Add(btnAbout);

            cbxDarkMode = new CheckBox
            {
                Text = "Dark Mode",
                AutoSize = true,
                Location = new Point(topPanel.Width - 110, 14),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            cbxDarkMode.CheckedChanged += cbxDarkMode_CheckedChanged;

            topPanel.Controls.Add(lblName);
            topPanel.Controls.Add(txtPlaylistName);
            topPanel.Controls.Add(lblVote);
            topPanel.Controls.Add(numVoteSeconds);
            topPanel.Controls.Add(btnNew);
            topPanel.Controls.Add(btnLoad);
            topPanel.Controls.Add(btnSave);
            topPanel.Controls.Add(cbxDarkMode);

            // ========== STATUS BAR ==========
            lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 26,
                Text = "Ready – Add at least 3 games before saving.",
                BorderStyle = BorderStyle.Fixed3D,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };

            // ========== LEFT PANEL ==========
            var leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 310,
                Padding = new Padding(8, 8, 4, 8)
            };

            var lblGames = new Label
            {
                Text = "Maps with Game Type:",
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 78,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(0, 6, 0, 0)
            };

            btnAddGame = new Button { Text = "Add Game", Size = new Size(95, 30), Margin = new Padding(0, 0, 6, 6) };
            btnRemoveGame = new Button { Text = "Remove", Size = new Size(80, 30), Margin = new Padding(0, 0, 6, 6) };
            btnDuplicateGame = new Button { Text = "Duplicate", Size = new Size(95, 30), Margin = new Padding(0, 0, 6, 6) };

            btnAddGame.Click += BtnAddGame_Click;
            btnRemoveGame.Click += BtnRemoveGame_Click;
            btnDuplicateGame.Click += BtnDuplicateGame_Click;

            buttonPanel.Controls.Add(btnAddGame);
            buttonPanel.Controls.Add(btnRemoveGame);
            buttonPanel.Controls.Add(btnDuplicateGame);

            lstGames = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9f)
            };
            lstGames.SelectedIndexChanged += LstGames_SelectedIndexChanged;

            leftPanel.Controls.Add(lstGames);
            leftPanel.Controls.Add(buttonPanel);
            leftPanel.Controls.Add(lblGames);

            // ========== RIGHT SIDE - GAME EDITOR ==========
            grpEditor = new GroupBox
            {
                Text = "Game Editor",
                Dock = DockStyle.Fill,
                Padding = new Padding(6, 4, 6, 4)
            };

            int y = 28;
            int lx = 18;
            int fx = 155;
            int fw = 490;

            // Map  (now supports .json / .map / .mvar)
            grpEditor.Controls.Add(new Label { Text = "Map:", Location = new Point(lx, y + 3), AutoSize = true });
            txtMap = new TextBox { Location = new Point(fx, y), Width = fw - 90 };
            grpEditor.Controls.Add(txtMap);

            btnBrowseMap = new Button { Text = "Browse", Location = new Point(fx + fw - 85, y - 1), Size = new Size(80, 26) };
            btnBrowseMap.Click += BtnBrowseMap_Click;
            grpEditor.Controls.Add(btnBrowseMap);
            y += 34;

            // Display Name
            grpEditor.Controls.Add(new Label { Text = "Display Name:", Location = new Point(lx, y + 3), AutoSize = true });
            txtGameName = new TextBox { Location = new Point(fx, y), Width = fw };
            grpEditor.Controls.Add(txtGameName);
            y += 34;

            // Game Type (.bin / .json only)
            grpEditor.Controls.Add(new Label { Text = "Game Type:", Location = new Point(lx, y + 3), AutoSize = true });
            txtGameType = new TextBox { Location = new Point(fx, y), Width = fw - 210 };
            grpEditor.Controls.Add(txtGameType);

            btnBrowseGameType = new Button
            {
                Text = "Browse",
                Location = new Point(fx + fw - 205, y - 1),
                Size = new Size(80, 26)
            };
            btnBrowseGameType.Click += BtnBrowseGameType_Click;
            grpEditor.Controls.Add(btnBrowseGameType);

            btnEditGameType = new Button
            {
                Text = "Edit Game Type…",
                Location = new Point(fx + fw - 120, y - 1),
                Size = new Size(115, 26),
                Enabled = false
            };
            btnEditGameType.Click += BtnEditGameType_Click;
            grpEditor.Controls.Add(btnEditGameType);
            y += 34;

            // Preview Image
            grpEditor.Controls.Add(new Label { Text = "Preview Image:", Location = new Point(lx, y + 3), AutoSize = true });
            txtPreviewImage = new TextBox { Location = new Point(fx, y), Width = fw - 90 };
            grpEditor.Controls.Add(txtPreviewImage);

            btnBrowsePreviewImage = new Button { Text = "Browse", Location = new Point(fx + fw - 85, y - 1), Size = new Size(80, 26) };
            btnBrowsePreviewImage.Click += BtnBrowsePreviewImage_Click;
            grpEditor.Controls.Add(btnBrowsePreviewImage);
            y += 38;

            // Weight + Sprint
            chkUseWeight = new CheckBox { Text = "Weight:", Location = new Point(lx, y + 2), AutoSize = true };
            numWeight = new NumericUpDown { Location = new Point(fx, y), Width = 70, Minimum = -1, Maximum = 1000, Value = 1 };
            grpEditor.Controls.Add(chkUseWeight);
            grpEditor.Controls.Add(numWeight);

            grpEditor.Controls.Add(new Label { Text = "Sprint:", Location = new Point(270, y + 3), AutoSize = true });
            cmbSprint = new ComboBox
            {
                Location = new Point(320, y),
                Width = 130,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbSprint.Items.AddRange(new object[] { "off", "unlimited", "stamina" });
            cmbSprint.SelectedIndex = 0;
            grpEditor.Controls.Add(cmbSprint);
            y += 42;

            // Rules
            grpEditor.Controls.Add(new Label { Text = "Rules", Location = new Point(lx, y), Font = new Font(this.Font, FontStyle.Bold), AutoSize = true });
            y += 26;

            grpEditor.Controls.Add(new Label { Text = "Score to Win:", Location = new Point(lx, y + 3), AutoSize = true });
            numScoreToWin = new NumericUpDown { Location = new Point(110, y), Width = 65, Minimum = -1, Maximum = 100000, Value = 0 };
            grpEditor.Controls.Add(numScoreToWin);

            grpEditor.Controls.Add(new Label { Text = "Kill Pts:", Location = new Point(195, y + 3), AutoSize = true });
            numKillPoints = new NumericUpDown { Location = new Point(250, y), Width = 55, Minimum = -1, Maximum = 100000, Value = 0 };
            grpEditor.Controls.Add(numKillPoints);

            grpEditor.Controls.Add(new Label { Text = "Death Pts:", Location = new Point(325, y + 3), AutoSize = true });
            numDeathPoints = new NumericUpDown { Location = new Point(395, y), Width = 55, Minimum = -1, Maximum = 100000, Value = 0 };
            grpEditor.Controls.Add(numDeathPoints);

            grpEditor.Controls.Add(new Label { Text = "Suicide:", Location = new Point(470, y + 3), AutoSize = true });
            numSuicidePoints = new NumericUpDown { Location = new Point(530, y), Width = 55, Minimum = -1, Maximum = 100000, Value = 0 };
            grpEditor.Controls.Add(numSuicidePoints);

            grpEditor.Controls.Add(new Label { Text = "Betrayal:", Location = new Point(605, y + 3), AutoSize = true });
            numBetrayalPoints = new NumericUpDown { Location = new Point(665, y), Width = 55, Minimum = -1, Maximum = 100000, Value = 0 };
            grpEditor.Controls.Add(numBetrayalPoints);
            y += 40;

            // Common Options
            grpEditor.Controls.Add(new Label { Text = "Common Options", Location = new Point(lx, y), Font = new Font(this.Font, FontStyle.Bold), AutoSize = true });
            y += 26;

            grpEditor.Controls.Add(new Label { Text = "Teams (0/1):", Location = new Point(lx, y + 3), AutoSize = true });
            numTeams = new NumericUpDown { Location = new Point(110, y), Width = 50, Minimum = -1, Maximum = 1, Value = 0 };
            grpEditor.Controls.Add(numTeams);

            grpEditor.Controls.Add(new Label { Text = "Minutes:", Location = new Point(180, y + 3), AutoSize = true });
            numMinutes = new NumericUpDown { Location = new Point(245, y), Width = 55, Minimum = -1, Maximum = 1000, Value = 0 };
            grpEditor.Controls.Add(numMinutes);

            grpEditor.Controls.Add(new Label { Text = "Vehicles:", Location = new Point(320, y + 3), AutoSize = true });
            numVehicles = new NumericUpDown { Location = new Point(385, y), Width = 50, Minimum = -1, Maximum = 10, Value = 0 };
            grpEditor.Controls.Add(numVehicles);
            y += 34;

            grpEditor.Controls.Add(new Label { Text = "Primary Weapon:", Location = new Point(lx, y + 3), AutoSize = true });
            txtPrimaryWeapon = new TextBox { Location = new Point(130, y), Width = 200 };
            grpEditor.Controls.Add(txtPrimaryWeapon);

            grpEditor.Controls.Add(new Label { Text = "Secondary:", Location = new Point(350, y + 3), AutoSize = true });
            txtSecondaryWeapon = new TextBox { Location = new Point(430, y), Width = 200 };
            grpEditor.Controls.Add(txtSecondaryWeapon);
            y += 34;

            grpEditor.Controls.Add(new Label { Text = "Map Weapons:", Location = new Point(lx, y + 3), AutoSize = true });
            txtMapWeapons = new TextBox { Location = new Point(130, y), Width = 200 };
            grpEditor.Controls.Add(txtMapWeapons);

            grpEditor.Controls.Add(new Label { Text = "Pickup / Grenades / Ammo:", Location = new Point(350, y + 3), AutoSize = true });
            numPickup = new NumericUpDown { Location = new Point(520, y), Width = 45, Minimum = -1, Maximum = 2, Value = 0 };
            numGrenades = new NumericUpDown { Location = new Point(575, y), Width = 45, Minimum = -1, Maximum = 2, Value = 0 };
            numAmmo = new NumericUpDown { Location = new Point(630, y), Width = 45, Minimum = -1, Maximum = 2, Value = 0 };

            grpEditor.Controls.Add(numPickup);
            grpEditor.Controls.Add(numGrenades);
            grpEditor.Controls.Add(numAmmo);
            y += 50;

            btnSaveGame = new Button { Text = "Save Game to Playlist", Location = new Point(lx, y), Size = new Size(180, 34) };
            btnCancelEdit = new Button { Text = "Cancel", Location = new Point(215, y), Size = new Size(100, 34) };
            btnSaveGame.Click += BtnSaveGame_Click;
            btnCancelEdit.Click += BtnCancelEdit_Click;

            grpEditor.Controls.Add(btnSaveGame);
            grpEditor.Controls.Add(btnCancelEdit);

            ResumeDirtyTracking();

            this.Controls.Add(grpEditor);
            this.Controls.Add(leftPanel);
            this.Controls.Add(topPanel);
            this.Controls.Add(lblStatus);

            SetupToolTips();

            this.ResumeLayout(true);
        }

        private void SetupToolTips()
        {
            toolTip = new ToolTip
            {
                AutoPopDelay = 9000,
                InitialDelay = 400,
                ReshowDelay = 200,
                ShowAlways = true
            };

            toolTip.SetToolTip(txtPlaylistName, "Name of the playlist shown on the voting screen.");
            toolTip.SetToolTip(numVoteSeconds, "How many seconds players have to vote for the next game.");

            toolTip.SetToolTip(txtMap, "Map name or Map Variant.\nSupported: .json, .map (extension stripped), .mvar");
            toolTip.SetToolTip(btnBrowseMap, "Browse for a map file (.json / .map / .mvar).\n.map files automatically strip the extension.");

            toolTip.SetToolTip(txtGameName, "Display name shown to players (e.g. SHOTTY SNIPERS).");

            toolTip.SetToolTip(txtGameType, "Game type file.\nSupported: .bin (binary) and .json (scripted/editable)");
            toolTip.SetToolTip(btnBrowseGameType, "Select a .bin or .json game type.");
            toolTip.SetToolTip(btnEditGameType, "Opens the Game Type Editor.\nOnly available for .json files.");

            toolTip.SetToolTip(txtPreviewImage, "Image shown in the voting screen.");
            toolTip.SetToolTip(btnBrowsePreviewImage, "Select a preview image (.jpg, .png, .bmp).");

            toolTip.SetToolTip(chkUseWeight, "Enable weighted random selection.");
            toolTip.SetToolTip(numWeight, "Higher weight = more likely to be chosen.");
            toolTip.SetToolTip(cmbSprint, "Sprint setting: off / unlimited / stamina");

            toolTip.SetToolTip(numScoreToWin, "Score required to win. 0 = no limit.");
            toolTip.SetToolTip(numKillPoints, "Points for a kill.");
            toolTip.SetToolTip(numDeathPoints, "Points when you die.");
            toolTip.SetToolTip(numSuicidePoints, "Points for suicide.");
            toolTip.SetToolTip(numBetrayalPoints, "Points for betraying a teammate.");

            toolTip.SetToolTip(numTeams, "0 = Free-for-All, 1 = Teams");
            toolTip.SetToolTip(numMinutes, "Round time limit in minutes. 0 = no limit.");
            toolTip.SetToolTip(numVehicles, "0 = none, 1 = enabled, 2 = all");
            toolTip.SetToolTip(txtPrimaryWeapon, "Starting primary weapon (name or ID).");
            toolTip.SetToolTip(txtSecondaryWeapon, "Starting secondary weapon (-1 = none).");
            toolTip.SetToolTip(txtMapWeapons, "Weapons available on the map.");
            toolTip.SetToolTip(numPickup, "Weapon pickup setting.");
            toolTip.SetToolTip(numGrenades, "Grenade setting.");
            toolTip.SetToolTip(numAmmo, "Ammo setting.");
        }

        private void MarkDirty(object sender, EventArgs e)
        {
            if (grpEditor.Enabled)
                isDirty = true;
        }

        private void SuspendDirtyTracking()
        {
            txtMap.TextChanged -= MarkDirty;
            txtGameName.TextChanged -= MarkDirty;
            txtGameType.TextChanged -= MarkDirty;
            txtPreviewImage.TextChanged -= MarkDirty;
            chkUseWeight.CheckedChanged -= MarkDirty;
            numWeight.ValueChanged -= MarkDirty;
            cmbSprint.SelectedIndexChanged -= MarkDirty;

            numScoreToWin.ValueChanged -= MarkDirty;
            numKillPoints.ValueChanged -= MarkDirty;
            numDeathPoints.ValueChanged -= MarkDirty;
            numSuicidePoints.ValueChanged -= MarkDirty;
            numBetrayalPoints.ValueChanged -= MarkDirty;

            numTeams.ValueChanged -= MarkDirty;
            numMinutes.ValueChanged -= MarkDirty;
            numVehicles.ValueChanged -= MarkDirty;
            numPickup.ValueChanged -= MarkDirty;
            numGrenades.ValueChanged -= MarkDirty;
            numAmmo.ValueChanged -= MarkDirty;

            txtPrimaryWeapon.TextChanged -= MarkDirty;
            txtSecondaryWeapon.TextChanged -= MarkDirty;
            txtMapWeapons.TextChanged -= MarkDirty;
        }

        private void ResumeDirtyTracking()
        {
            txtMap.TextChanged += MarkDirty;
            txtGameName.TextChanged += MarkDirty;
            txtGameType.TextChanged += MarkDirty;
            txtPreviewImage.TextChanged += MarkDirty;
            chkUseWeight.CheckedChanged += MarkDirty;
            numWeight.ValueChanged += MarkDirty;
            cmbSprint.SelectedIndexChanged += MarkDirty;

            numScoreToWin.ValueChanged += MarkDirty;
            numKillPoints.ValueChanged += MarkDirty;
            numDeathPoints.ValueChanged += MarkDirty;
            numSuicidePoints.ValueChanged += MarkDirty;
            numBetrayalPoints.ValueChanged += MarkDirty;

            numTeams.ValueChanged += MarkDirty;
            numMinutes.ValueChanged += MarkDirty;
            numVehicles.ValueChanged += MarkDirty;
            numPickup.ValueChanged += MarkDirty;
            numGrenades.ValueChanged += MarkDirty;
            numAmmo.ValueChanged += MarkDirty;

            txtPrimaryWeapon.TextChanged += MarkDirty;
            txtSecondaryWeapon.TextChanged += MarkDirty;
            txtMapWeapons.TextChanged += MarkDirty;
        }

        // ==================== EVENT HANDLERS ====================

        private void BtnNew_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Start a new empty playlist?", "New Playlist", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                currentPlaylist = new Playlist();
                txtPlaylistName.Text = currentPlaylist.Name;
                numVoteSeconds.Value = 20;
                RefreshGameList();
                SetEditorEnabled(false);
                isDirty = false;
                lblStatus.Text = "New playlist created.";
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Playlist JSON (*.json;*.playlist.json)|*.json;*.playlist.json|All files|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string json = File.ReadAllText(ofd.FileName);
                        currentPlaylist = JsonConvert.DeserializeObject<Playlist>(json) ?? new Playlist();
                        txtPlaylistName.Text = currentPlaylist.Name;
                        numVoteSeconds.Value = Math.Max(-1, Math.Min(1000, currentPlaylist.VoteSeconds));
                        RefreshGameList();
                        SetEditorEnabled(false);
                        isDirty = false;
                        lblStatus.Text = $"Loaded: {Path.GetFileName(ofd.FileName)} ({currentPlaylist.Games.Count} games)";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to load playlist:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            currentPlaylist.Name = txtPlaylistName.Text.Trim();
            currentPlaylist.VoteSeconds = (int)numVoteSeconds.Value;

            if (currentPlaylist.Games.Count < 3)
            {
                MessageBox.Show("Project Reclaimer requires at least 3 games in a playlist.", "Not enough games", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Playlist JSON|*.playlist.json|JSON files|*.json";
                sfd.FileName = MakeSafeFileName(currentPlaylist.Name) + ".playlist.json";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var settings = new JsonSerializerSettings
                    {
                        Formatting = Newtonsoft.Json.Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    string json = JsonConvert.SerializeObject(currentPlaylist, settings);
                    File.WriteAllText(sfd.FileName, json);
                    lblStatus.Text = "Saved: " + Path.GetFileName(sfd.FileName);
                    MessageBox.Show("Playlist saved successfully!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnAddGame_Click(object sender, EventArgs e)
        {
            isNewGame = true;
            editingGame = new PlaylistGame();

            SuspendDirtyTracking();

            txtMap.Text = "";
            txtGameName.Text = "";
            txtGameType.Text = "";
            txtPreviewImage.Text = "";
            chkUseWeight.Checked = false;
            numWeight.Value = 1;
            cmbSprint.SelectedIndex = 0;

            numScoreToWin.Value = 0;
            numKillPoints.Value = 0;
            numDeathPoints.Value = 0;
            numSuicidePoints.Value = 0;
            numBetrayalPoints.Value = 0;

            numTeams.Value = 0;
            numMinutes.Value = 0;
            numVehicles.Value = 0;
            numPickup.Value = 0;
            numGrenades.Value = 0;
            numAmmo.Value = 0;

            txtPrimaryWeapon.Text = "";
            txtSecondaryWeapon.Text = "";
            txtMapWeapons.Text = "";

            btnEditGameType.Enabled = false;
            btnEditGameType.Tag = null;

            ResumeDirtyTracking();

            SetEditorEnabled(true);
            isDirty = false;
            txtMap.Focus();
            lblStatus.Text = "Adding new game (all fields empty)...";
        }

        private void BtnRemoveGame_Click(object sender, EventArgs e)
        {
            if (lstGames.SelectedIndex < 0) return;

            if (MessageBox.Show("Remove this game?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                currentPlaylist.Games.RemoveAt(lstGames.SelectedIndex);
                RefreshGameList();
                SetEditorEnabled(false);
                isDirty = false;
                lblStatus.Text = "Game removed.";
            }
        }

        private void BtnDuplicateGame_Click(object sender, EventArgs e)
        {
            if (lstGames.SelectedIndex < 0) return;

            var source = currentPlaylist.Games[lstGames.SelectedIndex];
            var copy = JsonConvert.DeserializeObject<PlaylistGame>(JsonConvert.SerializeObject(source));
            currentPlaylist.Games.Add(copy);
            RefreshGameList();
            lstGames.SelectedIndex = currentPlaylist.Games.Count - 1;
            lblStatus.Text = "Game duplicated.";
        }

        private void BtnSaveGame_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMap.Text) || string.IsNullOrWhiteSpace(txtGameType.Text))
            {
                MessageBox.Show("Map and Game Type are required.", "Missing data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            editingGame.Map = txtMap.Text.Trim();
            editingGame.Name = string.IsNullOrWhiteSpace(txtGameName.Text) ? null : txtGameName.Text.Trim();
            editingGame.Game = txtGameType.Text.Trim();
            editingGame.PreviewImage = string.IsNullOrWhiteSpace(txtPreviewImage.Text) ? null : txtPreviewImage.Text.Trim();
            editingGame.Weight = chkUseWeight.Checked ? (int?)numWeight.Value : null;

            string sprint = cmbSprint.SelectedItem?.ToString() ?? "off";
            editingGame.World = (sprint == "off") ? null : new WorldSettings { Sprint = sprint };

            bool hasRules = numScoreToWin.Value != 0 || numKillPoints.Value != 0 ||
                            numDeathPoints.Value != 0 || numSuicidePoints.Value != 0 ||
                            numBetrayalPoints.Value != 0;

            if (hasRules)
            {
                editingGame.Rules = new Rules
                {
                    ScoreToWin = (int)numScoreToWin.Value,
                    KillPoints = (int)numKillPoints.Value,
                    DeathPoints = (int)numDeathPoints.Value,
                    SuicidePoints = (int)numSuicidePoints.Value,
                    BetrayalPoints = (int)numBetrayalPoints.Value
                };
            }
            else
            {
                editingGame.Rules = null;
            }

            var options = new Dictionary<string, object>();

            if (numTeams.Value != 0) options["rounds.teams"] = (int)numTeams.Value;
            options["rounds.reset_players"] = 1;
            options["rounds.limit"] = 1;
            if (numMinutes.Value != 0) options["rounds.minutes"] = (int)numMinutes.Value;
            if (numVehicles.Value != 0) options["map.vehicles"] = (int)numVehicles.Value;
            if (numPickup.Value != 0) options["player.pickup"] = (int)numPickup.Value;
            if (numGrenades.Value != 0) options["player.grenades"] = (int)numGrenades.Value;
            if (numAmmo.Value != 0) options["player.ammo"] = (int)numAmmo.Value;

            SetOptionValue(options, "player.primary_weapon", txtPrimaryWeapon.Text);
            SetOptionValue(options, "player.secondary_weapon", txtSecondaryWeapon.Text);
            SetOptionValue(options, "map.weapons", txtMapWeapons.Text);

            editingGame.Options = options.Count > 0 ? options : null;

            if (isNewGame)
                currentPlaylist.Games.Add(editingGame);

            RefreshGameList();
            SetEditorEnabled(false);
            isDirty = false;
            lblStatus.Text = isNewGame ? "Game added." : "Game updated.";
        }

        private void BtnCancelEdit_Click(object sender, EventArgs e)
        {
            SetEditorEnabled(false);
            isDirty = false;
            lblStatus.Text = "Edit cancelled.";
        }

        private void LstGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.IsDisposed || lstGames == null || lstGames.IsDisposed)
                return;

            if (lstGames.SelectedIndex < 0)
            {
                SetEditorEnabled(false);
                return;
            }

            int newIndex = lstGames.SelectedIndex;

            if (isDirty && grpEditor.Enabled)
            {
                var result = MessageBox.Show(
                    this,
                    "You have unsaved changes.\n\nDo you want to save them before switching?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Cancel)
                    return;

                if (result == DialogResult.Yes)
                {
                    BtnSaveGame_Click(null, null);
                    if (newIndex >= currentPlaylist.Games.Count)
                        newIndex = currentPlaylist.Games.Count - 1;
                }
            }

            if (newIndex < 0 || newIndex >= currentPlaylist.Games.Count)
            {
                SetEditorEnabled(false);
                return;
            }

            isNewGame = false;
            isDirty = false;
            editingGame = currentPlaylist.Games[newIndex];
            PopulateEditor(editingGame);
            SetEditorEnabled(true);
            lblStatus.Text = $"Editing: {editingGame.Map} – {editingGame.Name ?? editingGame.Game}";
        }

        // ==================== BROWSE METHODS (UPDATED) ====================

        private void BtnBrowseMap_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Map / Map Variant";
                ofd.Filter =
                    "Map files (*.json;*.map;*.mvar)|*.json;*.map;*.mvar|" +
                    "JSON Map (*.json)|*.json|" +
                    "Map (*.map)|*.map|" +
                    "Map Variant (*.mvar)|*.mvar|" +
                    "All files (*.*)|*.*";
                ofd.FilterIndex = 1;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string ext = Path.GetExtension(ofd.FileName).ToLowerInvariant();

                    if (ext == ".map")
                    {
                        // Project Reclaimer crashes if .map extension is kept
                        txtMap.Text = Path.GetFileNameWithoutExtension(ofd.FileName);
                    }
                    else
                    {
                        // .json and .mvar keep the full filename
                        txtMap.Text = Path.GetFileName(ofd.FileName);
                    }
                }
            }
        }

        private void BtnBrowseGameType_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Game Type";
                ofd.Filter =
                    "Game Types (*.bin;*.json)|*.bin;*.json|" +
                    "Binary (*.bin)|*.bin|" +
                    "Scripted (*.json)|*.json|" +
                    "All files (*.*)|*.*";
                ofd.FilterIndex = 1;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtGameType.Text = Path.GetFileName(ofd.FileName);

                    string ext = Path.GetExtension(ofd.FileName).ToLowerInvariant();
                    bool isEditable = (ext == ".json");

                    btnEditGameType.Enabled = isEditable;
                    btnEditGameType.Tag = isEditable ? ofd.FileName : null;
                }
            }
        }

        private void BtnEditGameType_Click(object sender, EventArgs e)
        {
            string fullPath = btnEditGameType.Tag as string;

            if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
            {
                MessageBox.Show("Please select a valid .json game type first.", "No file",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Pass the current dark mode state from the main form
            bool isDark = cbxDarkMode.Checked;

            using (var editor = new FrmGameTypeEditor(fullPath, isDark))
            {
                editor.ShowDialog(this);
            }
        }

        private void BtnBrowsePreviewImage_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Preview Image";
                ofd.Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                    txtPreviewImage.Text = Path.GetFileName(ofd.FileName);
            }
        }

        // ==================== HELPERS ====================

        private void RefreshGameList()
        {
            lstGames.Items.Clear();
            foreach (var g in currentPlaylist.Games)
            {
                string weight = g.Weight.HasValue ? $"  [w{g.Weight}]" : "";
                string displayName = string.IsNullOrEmpty(g.Name) ? g.Game : g.Name;
                lstGames.Items.Add($"{g.Map}  –  {displayName}{weight}");
            }
        }

        private void PopulateEditor(PlaylistGame g)
        {
            SuspendDirtyTracking();

            try
            {
                txtMap.Text = "";
                txtGameName.Text = "";
                txtGameType.Text = "";
                txtPreviewImage.Text = "";
                chkUseWeight.Checked = false;
                numWeight.Value = 1;
                cmbSprint.SelectedIndex = 0;

                numScoreToWin.Value = 0;
                numKillPoints.Value = 0;
                numDeathPoints.Value = 0;
                numSuicidePoints.Value = 0;
                numBetrayalPoints.Value = 0;

                numTeams.Value = 0;
                numMinutes.Value = 0;
                numVehicles.Value = 0;
                numPickup.Value = 0;
                numGrenades.Value = 0;
                numAmmo.Value = 0;

                txtPrimaryWeapon.Text = "";
                txtSecondaryWeapon.Text = "";
                txtMapWeapons.Text = "";

                btnEditGameType.Enabled = false;
                btnEditGameType.Tag = null;

                if (g == null) return;

                txtMap.Text = g.Map ?? "";
                txtGameName.Text = g.Name ?? "";
                txtGameType.Text = g.Game ?? "";
                txtPreviewImage.Text = g.PreviewImage ?? "";

                if (!string.IsNullOrEmpty(g.Game) && g.Game.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    btnEditGameType.Enabled = true;
                }

                if (g.Weight.HasValue)
                {
                    chkUseWeight.Checked = true;
                    numWeight.Value = g.Weight.Value;
                }

                if (g.World != null && !string.IsNullOrEmpty(g.World.Sprint))
                    cmbSprint.SelectedItem = g.World.Sprint;

                if (g.Rules != null)
                {
                    if (g.Rules.ScoreToWin.HasValue) numScoreToWin.Value = g.Rules.ScoreToWin.Value;
                    if (g.Rules.KillPoints.HasValue) numKillPoints.Value = g.Rules.KillPoints.Value;
                    if (g.Rules.DeathPoints.HasValue) numDeathPoints.Value = g.Rules.DeathPoints.Value;
                    if (g.Rules.SuicidePoints.HasValue) numSuicidePoints.Value = g.Rules.SuicidePoints.Value;
                    if (g.Rules.BetrayalPoints.HasValue) numBetrayalPoints.Value = g.Rules.BetrayalPoints.Value;
                }

                if (g.Options != null)
                {
                    if (g.Options.ContainsKey("rounds.teams"))
                        numTeams.Value = Convert.ToInt32(g.Options["rounds.teams"]);
                    if (g.Options.ContainsKey("rounds.minutes"))
                        numMinutes.Value = Convert.ToInt32(g.Options["rounds.minutes"]);
                    if (g.Options.ContainsKey("map.vehicles"))
                        numVehicles.Value = Convert.ToInt32(g.Options["map.vehicles"]);
                    if (g.Options.ContainsKey("player.pickup"))
                        numPickup.Value = Convert.ToInt32(g.Options["player.pickup"]);
                    if (g.Options.ContainsKey("player.grenades"))
                        numGrenades.Value = Convert.ToInt32(g.Options["player.grenades"]);
                    if (g.Options.ContainsKey("player.ammo"))
                        numAmmo.Value = Convert.ToInt32(g.Options["player.ammo"]);

                    if (g.Options.ContainsKey("player.primary_weapon"))
                        txtPrimaryWeapon.Text = g.Options["player.primary_weapon"]?.ToString() ?? "";
                    if (g.Options.ContainsKey("player.secondary_weapon"))
                        txtSecondaryWeapon.Text = g.Options["player.secondary_weapon"]?.ToString() ?? "";
                    if (g.Options.ContainsKey("map.weapons"))
                        txtMapWeapons.Text = g.Options["map.weapons"]?.ToString() ?? "";
                }
            }
            finally
            {
                ResumeDirtyTracking();
                isDirty = false;
            }
        }

        private void SetOptionValue(Dictionary<string, object> options, string key, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            text = text.Trim();
            if (int.TryParse(text, out int number))
                options[key] = number;
            else
                options[key] = text;
        }

        private void SetEditorEnabled(bool enabled)
        {
            grpEditor.Enabled = enabled;
            if (!enabled)
            {
                editingGame = null;
                isNewGame = false;
                isDirty = false;
            }
        }

        private string MakeSafeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name.Trim();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SuspendDirtyTracking();
            if (lstGames != null)
                lstGames.SelectedIndexChanged -= LstGames_SelectedIndexChanged;

            isDirty = false;
            base.OnFormClosing(e);
        }

        private void FrmReclaimerPlaylist_Load(object sender, EventArgs e)
        {
            this.Text = "PR Playlist Designer";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1075, 650);

            BuildUI();
            RefreshGameList();
            SetEditorEnabled(false);

            cbxDarkMode.Checked = Properties.Settings.Default.IsDarkMode;
            ApplyTheme(Properties.Settings.Default.IsDarkMode);
        }
        private void BtnAbout_Click(object sender, EventArgs e)
        {
            string aboutText =
        @"Project Reclaimer Playlist Designer

A tool for creating and editing playlist files
for Project Reclaimer.

Created by xSoMuchPotx
with assistance from Grok (xAI).

Format reference:
https://projectreclaimer.dev/host#playlists";

            MessageBox.Show(aboutText, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cbxDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxDarkMode.Checked != Properties.Settings.Default.IsDarkMode)
            {
                var ans = MessageBox.Show(
                    "Application requires a restart. Do you want to continue?",
                    ProductName,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (ans == DialogResult.Yes)
                {
                    Properties.Settings.Default.IsDarkMode = cbxDarkMode.Checked;
                    Properties.Settings.Default.Save();
                    ApplyTheme(Properties.Settings.Default.IsDarkMode);
                    Application.Restart();
                }
                else
                {
                    cbxDarkMode.Checked = Properties.Settings.Default.IsDarkMode;
                }
            }
        }

        public static class ThemeColors
        {
            public static Color LightBackground = SystemColors.Control;
            public static Color LightForeground = Color.Black;
            public static Color DarkBackground = Color.FromArgb(45, 45, 48);
            public static Color DarkForeground = Color.WhiteSmoke;
        }

        public void ApplyTheme(bool isDarkMode)
        {
            if (isDarkMode)
            {
                this.BackColor = ThemeColors.DarkBackground;
                this.ForeColor = ThemeColors.DarkForeground;
            }
            else
            {
                this.BackColor = ThemeColors.LightBackground;
                this.ForeColor = ThemeColors.LightForeground;
            }

            foreach (Control control in this.Controls)
                ApplyThemeToControl(control, isDarkMode);
        }

        private void ApplyThemeToControl(Control control, bool isDarkMode)
        {
            if (control == cbxDarkMode) return;

            if (isDarkMode)
            {
                control.BackColor = ThemeColors.DarkBackground;
                control.ForeColor = ThemeColors.DarkForeground;

                if (control is ListBox || control is TextBox || control is NumericUpDown || control is ComboBox)
                {
                    control.BackColor = Color.FromArgb(30, 30, 30);
                    control.ForeColor = Color.WhiteSmoke;
                }
            }
            else
            {
                control.BackColor = ThemeColors.LightBackground;
                control.ForeColor = ThemeColors.LightForeground;

                if (control is ListBox || control is TextBox || control is NumericUpDown || control is ComboBox)
                {
                    control.BackColor = SystemColors.Window;
                    control.ForeColor = SystemColors.WindowText;
                }
            }

            if (control.HasChildren)
            {
                foreach (Control child in control.Controls)
                    ApplyThemeToControl(child, isDarkMode);
            }
        }
    }
}