using HASS.Agent.Functions;
using System.Windows.Forms;

namespace HASS.Agent.Controls.Onboarding
{
    public partial class OnboardingHotKey : UserControl
    {
        private Keys _key = Keys.None;
        private Keys _modifiers = Keys.None;
        
        public OnboardingHotKey()
        {
            InitializeComponent();
        }

        private void OnboardingHotKey_Load(object sender, EventArgs e)
        {
            TbQuickActionsHotkey.ReadOnly = true;
            TbQuickActionsHotkey.KeyDown += TbQuickActionsHotkey_KeyDown;

            if (string.IsNullOrEmpty(Variables.AppSettings.QuickActionsHotKey))
            {
                // if nothing set, load default
                LoadDefault();
            }
            else if (Variables.AppSettings.QuickActionsHotKey == string.Empty)
            {
                // if set to empty, show empty
                TbQuickActionsHotkey.Text = string.Empty;
            }
            else
            {
                // show set value
                LoadSetValue();
            }
        }

        private void LoadDefault()
        {
            /*            if (!HelperFunctions.InputLanguageCheckDiffers(out var knownToCollide, out var warning))
                        {
                            TbQuickActionsHotkey.Text = "Shift, Control + Q";
                            LblLanguageWarning.Visible = false;
                            return;
                        }

                        if (knownToCollide)
                        {
                            // the system's input language collides with our hotkey, let the user know and set empty key
                            LblLanguageWarning.Text = warning;
                            TbQuickActionsHotkey.Text = _hotkeySelector.EmptyHotkeyText;
                            return;
                        }*/
            //Amadeo(Note): above was commented out when we changed default hotkey to ctrl+shift+q, leaving this here because reasons

            // the system's input language is unknown, we're presetting the default but warn the user
            // deprecated, we're not doing this anymore
            //TbQuickActionsHotkey.Text = "Shift, Control + Q";
            //LblLanguageWarning.ForeColor = Color.DarkOrange;
            //LblLanguageWarning.Text = warning;

            TbQuickActionsHotkey.Text = "Shift, Control + Q";
            LblLanguageWarning.Visible = false;
        }

        private void LoadSetValue()
        {
            TbQuickActionsHotkey.Text = Variables.AppSettings.QuickActionsHotKey;

            if (!HelperFunctions.InputLanguageCheckDiffers(out var knownToCollide, out var warning))
                return;

            // the system's input language is unknown or collides with our hotkey, let the user know if it's set to default
            if (Variables.AppSettings.QuickActionsHotKey != "Shift, Control + Q")
                return;

            if (knownToCollide)
                LblLanguageWarning.Text = warning;
        }

        internal bool Store()
        {
            Variables.AppSettings.QuickActionsHotKey = TbQuickActionsHotkey.Text;
            TbQuickActionsHotkey.KeyDown -= TbQuickActionsHotkey_KeyDown;
            return true;
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            TbQuickActionsHotkey.Text = string.Empty;
        }
        
        private void TbQuickActionsHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;

            var key = e.KeyCode;

            if (key is Keys.LControlKey or Keys.RControlKey
                or Keys.LShiftKey or Keys.RShiftKey
                or Keys.LWin or Keys.RWin
                or Keys.Alt)
            {
                key = Keys.None;
            }

            if (key == Keys.Escape)
            {
                _key = Keys.None;
                _modifiers = Keys.None;
                TbQuickActionsHotkey.Text = string.Empty;

                return;
            }
            
            _key = key;
            TbQuickActionsHotkey.Text = FormatHotkey(_key, e.Modifiers);
        }
        
        private string FormatHotkey(Keys key, Keys modifiers)
        {
            var parts = new List<string>();
            if ((modifiers & Keys.Shift) != 0)
            {
                parts.Add(nameof(Keys.Shift));
            }

            if ((modifiers & Keys.Control) != 0)
            {
                parts.Add(nameof(Keys.Control));
            }

            if ((modifiers & Keys.Alt) != 0)
            {
                parts.Add(nameof(Keys.Alt));
            }

            return parts.Count > 0 ? string.Join(", ", parts) + " + " + key : key.ToString();
        }
    }
}
