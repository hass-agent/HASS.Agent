using HASS.Agent.Controls;
using HASS.Agent.Enums;
using HASS.Agent.Functions;
using HASS.Agent.HomeAssistant;
using HASS.Agent.Models.Internal;
using HASS.Agent.Resources.Localization;
using Serilog;
using Syncfusion.Windows.Forms;

namespace HASS.Agent.Forms.QuickActions
{
    public partial class QuickActions : MetroForm
    {
        public event EventHandler ClearFocus;

        private readonly List<QuickAction> _quickActions = new();
        private readonly List<QuickActionPanelControl> _quickActionPanelControls = new();

        private readonly Dictionary<int, int> _rowColumnCounts = new();

        private int _columns = 0;
        private int _rows = 0;

        private int _selectedColumn = -1;
        private int _selectedRow = -1;

        public QuickActions(List<QuickAction> quickActions)
        {
            foreach (var quickAction in quickActions)
            {
                _quickActions.Add(quickAction);
            }

            InitializeComponent();
        }

        private async void QuickActions_Load(object sender, EventArgs e)
        {
            // catch all key presses
            KeyPreview = true;

            // hide to prevent flickering
            Visible = false;

            // create our layout
            BuildLayout();

            // re-center
            CenterToScreen();

            // show ourselves
            Opacity = 100;
            Visible = true;

            // get focus (so the esc button works)
            BringToFront();
            Activate();

            // check hass status
            var hass = await CheckHassManagerAsync();
            if (!hass)
                CloseWindow();

            // select first item
            SelectQuickActionItem(0, 0);
        }

        /// <summary>
        /// Prepares the FlowLayout, resizes our form accordingly and load the QuickAction controls
        /// </summary>
        private void BuildLayout()
        {
            // determine layout
            var (columns, rows) = HelperFunctions.DetermineRowColumnCount(_quickActions);
            if (columns == 0 && rows == 0)
            {
                CloseWindow();
                return;
            }

            _columns = columns;
            _rows = rows;

            // prepare our panel
            PnlActions.AutoSize = true;

            PnlActions.ColumnCount = _columns;
            PnlActions.RowCount = _rows;

            PnlActions.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

            for (var c = 0; c <= _columns; c++)
            {
                PnlActions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 152));
            }

            for (var r = 0; r <= _columns; r++)
            {
                PnlActions.RowStyles.Add(new RowStyle(SizeType.Absolute, 255));
            }

            // resize window
            Width = 152 * columns + 20;
            Height = 255 * rows + 30;

            if (columns > 1)
                Width += 5 * (columns - 1);
            if (rows > 1)
                Height += 5 * (rows - 1);

            var scalingFactor = HelperFunctions.GetScalingFactors();
            Width = (int)(Width * scalingFactor.dpiScalingFactor);
            Height = (int)(Height * scalingFactor.dpiScalingFactor);

            // add the quickactions as controls
            var currentColumn = 0;
            var currentRow = 0;
            foreach (var quickAction in _quickActions.Select(qA => new QuickActionControl(qA, this)))
            {
                // prepare the panelcontrol (so we can its location later)
                var panelControl = new QuickActionPanelControl
                {
                    QuickActionControl = quickAction,
                    Column = currentColumn,
                    Row = currentRow
                };

                // update position when item is selected by mouse cursor
                panelControl.QuickActionControl.MouseEnter += QuickActionItemMouseEnter;

                // add to list
                _quickActionPanelControls.Add(panelControl);

                // store position
                if (!_rowColumnCounts.ContainsKey(currentRow))
                {
                    _rowColumnCounts.Add(currentRow, currentColumn);
                }
                else
                {
                    _rowColumnCounts[currentRow] = currentColumn;
                }

                // add to the panel
                PnlActions.Controls.Add(quickAction, currentColumn, currentRow);

                // set next column & row
                if (currentColumn < columns - 1)
                {
                    currentColumn++;
                }
                else
                {
                    // on to the next row (if there is one)
                    currentColumn = 0;
                    if (currentRow < rows - 1)
                        currentRow++;
                }
            }
        }

        private void QuickActionItemMouseEnter(object sender, EventArgs e)
        {
            var position = PnlActions.GetPositionFromControl(sender as QuickActionControl);
            _selectedColumn = position.Column;
            _selectedRow = position.Row;
            return;
        }

        /// <summary>
        /// Tries to close the window
        /// </summary>
        internal void CloseWindow()
        {
            if (!IsHandleCreated)
                return;
            if (IsDisposed)
                return;

            Invoke(new MethodInvoker(delegate
            {
                DialogResult = DialogResult.OK;
                Close();
            }));
        }

        /// <summary>
        /// Check whether the HASS manager is ready to process our request
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CheckHassManagerAsync()
        {
            switch (HassApiManager.ManagerStatus)
            {
                case HassManagerStatus.Ready:
                    return true;

                case HassManagerStatus.ConfigMissing:
                    MessageBoxAdv.Show(this, Languages.QuickActions_CheckHassManager_MessageBox1, Variables.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;

                case HassManagerStatus.Failed:
                    MessageBoxAdv.Show(this, Languages.QuickActions_MessageBox_EntityFailed, Variables.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;

                case HassManagerStatus.Initialising:
                case HassManagerStatus.LoadingData:
                    SetGuiLoading(true);
                    while (HassApiManager.ManagerStatus != HassManagerStatus.Ready)
                    {
                        await Task.Delay(150);
                        if (HassApiManager.ManagerStatus != HassManagerStatus.Failed) continue;

                        MessageBoxAdv.Show(this, Languages.QuickActions_MessageBox_EntityFailed, Variables.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                    SetGuiLoading(false);
                    break;
            }

            return true;
        }

        /// <summary>
        /// Lock the UI while loading
        /// </summary>
        /// <param name="loading"></param>
        private void SetGuiLoading(bool loading)
        {
            if (!IsHandleCreated)
                return;
            if (IsDisposed)
                return;

            Invoke(new MethodInvoker(delegate
            {
                LblLoading.Visible = loading;
            }));
        }

        /// <summary>
        /// Close when ESC is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuickActions_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Escape) return;
            Close();
        }

        private void QuickActions_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (var control in _quickActionPanelControls) control.Dispose();
            }
            catch
            {
                // best effort
            }
        }

        /// <summary>
        /// Selects QuickAction item at given position
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        private bool SelectQuickActionItem(int row, int column)
        {
            var control = _quickActionPanelControls.Find(x => x.Row == row && x.Column == column);
            if (control == null)
                return false;

            control.QuickActionControl.OnFocus();
            _selectedColumn = column;
            _selectedRow = row;

            return true;
        }

        /// <summary>
        /// Intercepts and processes the arrow keys
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                // should not happen, but select first item if nothing is selected
                if (_selectedColumn == -1)
                {
                    SelectQuickActionItem(0, 0);

                    return true;
                }

                if (keyData == Keys.Down)
                {
                    // wrap up if we're at the last row
                    if (_selectedRow == _rows - 1)
                    {
                        SelectQuickActionItem(0, _selectedColumn);

                        return true;
                    }

                    var nextRow = _selectedRow + 1;
                    var selected = SelectQuickActionItem(nextRow, _selectedColumn);
                    if (!selected)
                    {
                        SelectQuickActionItem(nextRow, _rowColumnCounts[nextRow]);
                    }

                    return true;
                }

                if (keyData == Keys.Right)
                {
                    var maxColumnsForRow = _rowColumnCounts[_selectedRow];

                    // wrap up to first row if there is nothing below
                    if (_selectedColumn == maxColumnsForRow)
                    {
                        var nextRow = _selectedRow == (_rows - 1) ? 0 : _selectedRow + 1;
                        SelectQuickActionItem(nextRow, 0);

                        return true;
                    }

                    SelectQuickActionItem(_selectedRow, _selectedColumn + 1);

                    return true;
                }

                if (keyData == Keys.Left)
                {
                    // wrap up to last row if there is nothing above
                    if (_selectedColumn == 0)
                    {
                        var nextRow = _selectedRow == 0 ? _rows - 1 : _selectedRow - 1;
                        SelectQuickActionItem(nextRow, _rowColumnCounts[nextRow]);

                        return true;
                    }

                    SelectQuickActionItem(_selectedRow, _selectedColumn - 1);

                    return true;
                }

                if (keyData == Keys.Up)
                {
                    var nextRow = _selectedRow - 1;

                    // wrap down if we're at the first row
                    if (_selectedRow == 0)
                    {
                        nextRow = _rows - 1;
                        var maxColumnsForNextRow = _rowColumnCounts[nextRow];
                        var nextColumn = maxColumnsForNextRow < _selectedColumn ? maxColumnsForNextRow : _selectedColumn;
                        SelectQuickActionItem(nextRow, nextColumn);

                        return true;
                    }

                    var selected = SelectQuickActionItem(nextRow, _selectedColumn);
                    if (!selected)
                    {
                        SelectQuickActionItem(nextRow, _rowColumnCounts[nextRow]);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[QUICKACTIONS] Error while trying to handle arrowkeys: {msg}", ex.Message);
            }

            // ignore the rest
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Selects next Quick Action item following right->down->up pattern
        /// </summary>
        public void SelectNextQuickActionItem()
        {
            var maxColumnsForRow = _rowColumnCounts[_selectedRow];

            // are we at the end of the row / wrap up to first row if there is nothing below
            if (_selectedColumn == maxColumnsForRow)
            {
                var nextRow = _selectedRow == (_rows - 1) ? 0 : _selectedRow + 1;
                SelectQuickActionItem(nextRow, 0);

                return;
            }

            SelectQuickActionItem(_selectedRow, _selectedColumn + 1);

            return;
        }

        /// <summary>
        /// Triggers clearing the focus of all controls
        /// </summary>
        public void ClearAllFocus() => OnClearFocus();
        protected virtual void OnClearFocus() => ClearFocus?.Invoke(this, EventArgs.Empty);

        private void QuickActions_ResizeEnd(object sender, EventArgs e)
        {
            if (Variables.ShuttingDown)
                return;
            if (!IsHandleCreated)
                return;
            if (IsDisposed)
                return;

            try
            {
                Refresh();
            }
            catch
            {
                // best effort
            }
        }
    }
}
