using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ClientManagerMaterial
{
    public partial class MainForm : Form
    {
        private const string DbPath = "clients.db";
        private string connectionString;

        // Controls
        private TextBox txtSearch;
        private Button btnSearch, btnAdd, btnEdit, btnDelete, btnRefresh;
        private DataGridView dataGridView1;
        private Label lblTitle, lblSearch, lblClientInfo, lblHotkeys;
        private Panel headerPanel, footerPanel, contentPanel, searchPanel, gridPanel;

        public MainForm()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadClients();
            ApplyModernStyle();
            SetupHotkeysInfo();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Client Manager - Modern UI";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 600);
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.KeyPreview = true;
            this.Font = new Font("Segoe UI", 9);

            // Main container panel
            contentPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(20)
            };

            // Header Panel
            headerPanel = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(74, 20, 140)
            };

            lblTitle = new Label()
            {
                Text = "Client Manager",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(30, 25),
                AutoSize = true
            };

            // Search Panel
            searchPanel = new Panel()
            {
                Location = new Point(20, 100),
                Size = new Size(1160, 70),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(15)
            };

            lblSearch = new Label()
            {
                Text = "Поиск клиентов",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 22),
                Size = new Size(150, 25),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            txtSearch = new TextBox()
            {
                PlaceholderText = "Введите имя, телефон или марку машины...",
                Location = new Point(180, 20),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250)
            };
            txtSearch.KeyPress += txtSearch_KeyPress;

            btnSearch = new Button()
            {
                Text = "Поиск",
                Location = new Point(590, 20),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(74, 20, 140),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSearch.Click += btnSearch_Click;

            btnRefresh = new Button()
            {
                Text = "Обновить",
                Location = new Point(700, 20),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.Click += (s, e) => LoadClients();

            // Action Buttons Panel
            var actionPanel = new Panel()
            {
                Location = new Point(820, 15),
                Size = new Size(320, 45),
                BackColor = Color.Transparent
            };

            btnAdd = new Button()
            {
                Text = "+ Добавить клиента",
                Location = new Point(0, 5),
                Size = new Size(150, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.Click += ShowAddClientForm;

            btnEdit = new Button()
            {
                Text = "✎ Редактировать",
                Location = new Point(160, 5),
                Size = new Size(150, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEdit.Click += btnEdit_Click;

            btnDelete = new Button()
            {
                Text = "🗑️ Удалить",
                Location = new Point(0, 45),
                Size = new Size(150, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDelete.Click += btnDelete_Click;

            // Grid Panel
            gridPanel = new Panel()
            {
                Location = new Point(20, 190),
                Size = new Size(1160, 400),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(15)
            };

            // DataGridView
            dataGridView1 = new DataGridView()
            {
                Location = new Point(15, 40),
                Size = new Size(1130, 345),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                Font = new Font("Segoe UI", 9),
                GridColor = Color.FromArgb(240, 240, 240)
            };
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            dataGridView1.KeyDown += dataGridView1_KeyDown;

            // Grid header
            var gridHeader = new Label()
            {
                Text = "Список клиентов",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(15, 10),
                Size = new Size(200, 25),
                ForeColor = Color.FromArgb(74, 20, 140)
            };

            // Hotkeys info
            lblHotkeys = new Label()
            {
                Text = "Горячие клавиши: Ctrl+N - Очистить • Ctrl+S - Поиск • Ctrl+A - Добавить • Ctrl+E - Редактировать • Delete - Удалить • F5 - Обновить",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(20, 610),
                Size = new Size(1160, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Footer Panel
            footerPanel = new Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = Color.FromArgb(74, 20, 140)
            };

            var footerLabel = new Label()
            {
                Text = "© 2024 Client Manager • Управление клиентской базой",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8),
                Dock = DockStyle.Right,
                AutoSize = true,
                Padding = new Padding(0, 8, 20, 0)
            };

            // Build UI hierarchy
            actionPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });
            searchPanel.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnSearch, btnRefresh, actionPanel });
            gridPanel.Controls.AddRange(new Control[] { gridHeader, dataGridView1 });
            footerPanel.Controls.Add(footerLabel);
            headerPanel.Controls.Add(lblTitle);

            contentPanel.Controls.AddRange(new Control[] {
                searchPanel, gridPanel, lblHotkeys
            });

            this.Controls.AddRange(new Control[] {
                headerPanel, contentPanel, footerPanel
            });

            this.KeyDown += MainForm_KeyDown;
        }

        private void ShowAddClientForm(object sender, EventArgs e)
        {
            using (var addForm = new AddClientForm())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    // Добавляем клиента в базу
                    AddClientToDatabase(addForm.ClientName, addForm.ClientPhone, addForm.CarModel, addForm.Recipe, addForm.PaintCode);
                    LoadClients();
                    ShowSuccess("Клиент успешно добавлен");
                }
            }
        }

        private void AddClientToDatabase(string name, string phone, string carModel, string recipe, string paintCode)
        {
            try
            {
                using var connection = new SQLiteConnection(connectionString);
                connection.Open();
                string sql = @"INSERT INTO clients (Name, Phone, CarModel, Recipe, PaintCode) 
                             VALUES (@n, @p, @c, @r, @pc)";
                using var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@n", name.Trim());
                cmd.Parameters.AddWithValue("@p", phone.Trim());
                cmd.Parameters.AddWithValue("@c", carModel.Trim());
                cmd.Parameters.AddWithValue("@r", recipe.Trim());
                cmd.Parameters.AddWithValue("@pc", paintCode.Trim());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при добавлении клиента: {ex.Message}");
            }
        }

        private void ApplyModernStyle()
        {
            foreach (Control control in GetAllControls(this))
            {
                if (control is Button button)
                {
                    button.FlatAppearance.BorderSize = 0;
                    button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(button.BackColor, 0.1f);
                    button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(button.BackColor, 0.2f);

                    button.MouseEnter += (s, e) =>
                    {
                        button.BackColor = ControlPaint.Light(button.BackColor, 0.1f);
                    };
                    button.MouseLeave += (s, e) =>
                    {
                        if (button == btnAdd) button.BackColor = Color.FromArgb(40, 167, 69);
                        else if (button == btnEdit) button.BackColor = Color.FromArgb(255, 193, 7);
                        else if (button == btnDelete) button.BackColor = Color.FromArgb(220, 53, 69);
                        else if (button == btnSearch || button == btnRefresh)
                            button.BackColor = Color.FromArgb(108, 117, 125);
                    };
                }
                else if (control is TextBox textBox)
                {
                    textBox.Enter += (s, e) =>
                    {
                        textBox.BackColor = Color.White;
                    };
                    textBox.Leave += (s, e) =>
                    {
                        textBox.BackColor = Color.FromArgb(250, 250, 250);
                    };
                }
            }

            // Style DataGridView
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(74, 20, 140);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersHeight = 40;

            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dataGridView1.RowTemplate.Height = 35;

            // Add shadow effects
            AddShadowEffect(searchPanel);
            AddShadowEffect(gridPanel);
        }

        private void AddShadowEffect(Control control)
        {
            control.Paint += (s, e) =>
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 2, 2, control.Width - 4, control.Height - 4);
                }
                using (var borderPen = new Pen(Color.FromArgb(200, 200, 200)))
                {
                    e.Graphics.DrawRectangle(borderPen, 0, 0, control.Width - 1, control.Height - 1);
                }
            };
        }

        private IEnumerable<Control> GetAllControls(Control control)
        {
            foreach (Control child in control.Controls)
            {
                yield return child;
                foreach (Control descendant in GetAllControls(child))
                    yield return descendant;
            }
        }

        private void SetupHotkeysInfo()
        {
            var toolTip = new ToolTip();
            toolTip.IsBalloon = true;
            toolTip.SetToolTip(btnAdd, "Ctrl+A - Добавить нового клиента");
            toolTip.SetToolTip(btnEdit, "Ctrl+E - Редактировать выделенного клиента");
            toolTip.SetToolTip(btnDelete, "Delete - Удалить выделенного клиента");
            toolTip.SetToolTip(btnSearch, "Ctrl+S - Быстрый поиск");
            toolTip.SetToolTip(btnRefresh, "F5 - Обновить данные");
        }

        // Остальные методы без изменений...
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                        btnAdd.PerformClick();
                        e.Handled = e.SuppressKeyPress = true;
                        break;
                    case Keys.S:
                        btnSearch.PerformClick();
                        e.Handled = e.SuppressKeyPress = true;
                        break;
                    case Keys.E:
                        if (btnEdit.Enabled) btnEdit.PerformClick();
                        e.Handled = e.SuppressKeyPress = true;
                        break;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.F5:
                        LoadClients();
                        e.Handled = e.SuppressKeyPress = true;
                        break;
                    case Keys.Escape:
                        txtSearch.Text = "";
                        LoadClients();
                        e.Handled = e.SuppressKeyPress = true;
                        break;
                }
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (btnDelete.Enabled)
                    btnDelete.PerformClick();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (btnEdit.Enabled && dataGridView1.CurrentRow != null)
                    btnEdit.PerformClick();
                e.Handled = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.A))
            {
                btnAdd.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.S))
            {
                btnSearch.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.E))
            {
                if (btnEdit.Enabled) btnEdit.PerformClick();
                return true;
            }
            if (keyData == Keys.F5)
            {
                LoadClients();
                return true;
            }
            if (keyData == Keys.Delete && dataGridView1.Focused)
            {
                if (btnDelete.Enabled) btnDelete.PerformClick();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void InitializeDatabase()
        {
            connectionString = $"Data Source={DbPath};Version=3;FailIfMissing=False;";

            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
            }

            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            string sql = @"CREATE TABLE IF NOT EXISTS clients (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Phone TEXT NOT NULL,
                CarModel TEXT NOT NULL,
                Recipe TEXT,
                PaintCode TEXT,
                CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
            )";
            using var command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        private void LoadClients(string search = "")
        {
            try
            {
                using var connection = new SQLiteConnection(connectionString);
                connection.Open();
                string sql = @"SELECT Id, Name, Phone, CarModel, Recipe, PaintCode, CreatedDate, UpdatedDate FROM clients";

                if (!string.IsNullOrWhiteSpace(search))
                {
                    sql += " WHERE Name LIKE @search OR Phone LIKE @search OR CarModel LIKE @search";
                }

                sql += " ORDER BY UpdatedDate DESC";

                using var command = new SQLiteCommand(sql, connection);
                if (!string.IsNullOrWhiteSpace(search))
                {
                    command.Parameters.AddWithValue("@search", $"%{search}%");
                }

                using var adapter = new SQLiteDataAdapter(command);
                DataTable dt = new();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;
                ConfigureDataGridView();
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void ConfigureDataGridView()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["Name"].HeaderText = "Имя";
                dataGridView1.Columns["Phone"].HeaderText = "Телефон";
                dataGridView1.Columns["CarModel"].HeaderText = "Марка машины";
                dataGridView1.Columns["Recipe"].HeaderText = "Рецепт";
                dataGridView1.Columns["PaintCode"].HeaderText = "Код краски";
                dataGridView1.Columns["CreatedDate"].HeaderText = "Дата создания";
                dataGridView1.Columns["UpdatedDate"].HeaderText = "Дата обновления";

                dataGridView1.Columns["CreatedDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                dataGridView1.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadClients(txtSearch.Text.Trim());
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                ShowError("Выберите клиента для редактирования");
                return;
            }

            // Редактирование через модальное окно
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Id"].Value);
            string name = dataGridView1.CurrentRow.Cells["Name"].Value?.ToString() ?? "";
            string phone = dataGridView1.CurrentRow.Cells["Phone"].Value?.ToString() ?? "";
            string carModel = dataGridView1.CurrentRow.Cells["CarModel"].Value?.ToString() ?? "";
            string recipe = dataGridView1.CurrentRow.Cells["Recipe"].Value?.ToString() ?? "";
            string paintCode = dataGridView1.CurrentRow.Cells["PaintCode"].Value?.ToString() ?? "";

            using (var editForm = new AddClientForm(name, phone, carModel, recipe, paintCode))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    UpdateClientInDatabase(id, editForm.ClientName, editForm.ClientPhone, editForm.CarModel, editForm.Recipe, editForm.PaintCode);
                    LoadClients();
                    ShowSuccess("Данные клиента успешно обновлены");
                }
            }
        }

        private void UpdateClientInDatabase(int id, string name, string phone, string carModel, string recipe, string paintCode)
        {
            try
            {
                using var connection = new SQLiteConnection(connectionString);
                connection.Open();
                string sql = @"UPDATE clients SET Name=@n, Phone=@p, CarModel=@c, Recipe=@r, PaintCode=@pc, 
                             UpdatedDate=CURRENT_TIMESTAMP WHERE Id=@id";
                using var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@n", name.Trim());
                cmd.Parameters.AddWithValue("@p", phone.Trim());
                cmd.Parameters.AddWithValue("@c", carModel.Trim());
                cmd.Parameters.AddWithValue("@r", recipe.Trim());
                cmd.Parameters.AddWithValue("@pc", paintCode.Trim());
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при редактировании клиента: {ex.Message}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                ShowError("Выберите клиента для удаления");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить выбранного клиента?",
                "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Id"].Value);

                    using var connection = new SQLiteConnection(connectionString);
                    connection.Open();
                    string sql = "DELETE FROM clients WHERE Id=@id";
                    using var cmd = new SQLiteCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();

                    ShowSuccess("Клиент успешно удален");
                    LoadClients();
                }
                catch (Exception ex)
                {
                    ShowError($"Ошибка при удалении клиента: {ex.Message}");
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // Обновляем доступность кнопок редактирования/удаления
            bool hasSelection = dataGridView1.CurrentRow != null;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                LoadClients(txtSearch.Text.Trim());
                e.Handled = true;
            }
        }
    }

    // Форма для добавления/редактирования клиента
    public class AddClientForm : Form
    {
        private TextBox txtName, txtPhone, txtCarModel, txtRecipe, txtPaint;
        private Button btnSave, btnCancel;

        public string ClientName => txtName.Text;
        public string ClientPhone => txtPhone.Text;
        public string CarModel => txtCarModel.Text;
        public string Recipe => txtRecipe.Text;
        public string PaintCode => txtPaint.Text;

        public AddClientForm(string name = "", string phone = "", string carModel = "", string recipe = "", string paintCode = "")
        {
            InitializeComponent();
            txtName.Text = name;
            txtPhone.Text = phone;
            txtCarModel.Text = carModel;
            txtRecipe.Text = recipe;
            txtPaint.Text = paintCode;

            this.Text = string.IsNullOrEmpty(name) ? "Добавить клиента" : "Редактировать клиента";
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            var title = new Label()
            {
                Text = this.Text,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(74, 20, 140),
                Location = new Point(0, 10),
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            txtName = CreateStyledTextBox("Имя *", new Point(50, 60), 400);
            txtPhone = CreateStyledTextBox("Телефон *", new Point(50, 110), 400);
            txtCarModel = CreateStyledTextBox("Марка машины *", new Point(50, 160), 400);
            txtRecipe = CreateStyledTextBox("Рецепт", new Point(50, 210), 400);
            txtRecipe.Multiline = true;
            txtRecipe.Height = 60;
            txtPaint = CreateStyledTextBox("Код краски", new Point(50, 285), 400);

            // Buttons
            btnSave = new Button()
            {
                Text = "Сохранить",
                Location = new Point(150, 340),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };

            btnCancel = new Button()
            {
                Text = "Отмена",
                Location = new Point(260, 340),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };

            btnSave.Click += ValidateForm;
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            mainPanel.Controls.AddRange(new Control[] {
                title, txtName, txtPhone, txtCarModel, txtRecipe, txtPaint, btnSave, btnCancel
            });

            this.Controls.Add(mainPanel);
        }

        private TextBox CreateStyledTextBox(string placeholder, Point location, int width)
        {
            return new TextBox()
            {
                PlaceholderText = placeholder,
                Location = location,
                Size = new Size(width, 35),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250)
            };
        }

        private void ValidateForm(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Поле 'Имя' обязательно для заполнения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtName.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Поле 'Телефон' обязательно для заполнения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPhone.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCarModel.Text))
            {
                MessageBox.Show("Поле 'Марка машины' обязательно для заполнения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCarModel.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }
        }
    }
}
