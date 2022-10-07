using EllieMae.Encompass.Automation;
using EllieMae.Encompass.ComponentModel;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace EncTemplatesMgr.View
{
    [Plugin]
    public class Plugin
    {
        private Thread _wpfThread;

        /// <summary>
        /// Constructor needs to be public for Encompass to run the plugin.
        /// </summary>
        public Plugin()
        {
            EncompassApplication.Login += new EventHandler(this.EncompassApplication_Login);
            EncompassApplication.Logout += new EventHandler(this.EncompassApplication_Logout);
        }

        private void EncompassApplication_Login(object sender, EventArgs e)
        {
            if (!this.AccessToToolAllowed())
                return;

            try
            {
                this.AddAccessControlToUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EncompassApplication_Logout(object sender, EventArgs e)
        {
            if (_wpfThread != null)
                _wpfThread.Abort();
        }

        private bool AccessToToolAllowed()
        {
            var currentUser = EncompassApplication.CurrentUser;
            if (currentUser.Personas.Contains(EncompassApplication.Session.Users.Personas.GetPersonaByName("Super Administrator")) || currentUser.ID == "admin")
                return true;

            return false;
        }

        private void AddAccessControlToUI()
        {
            System.Windows.Forms.Form main = null;
            var openForms = Application.OpenForms;
            foreach (System.Windows.Forms.Form form in openForms)
            {
                if (form.Text.Contains("Encompass"))
                {
                    main = form;
                    break;
                }
            }

            if (main == null)
            {
                if (openForms[0] == null)
                    return;

                main = openForms[0];
            }

            System.Windows.Forms.Control[] controls = main.Controls.Find("mainMenu", true);

            if (controls.Count() == 0) 
                return;

            MenuStrip menuStrip = controls[0] as MenuStrip;

            if (menuStrip == null) 
                return;

            ToolStripMenuItem menuRow = menuStrip.Items[0] as ToolStripMenuItem;
            ToolStripMenuItem newItem = new ToolStripMenuItem("Enc Templates Mgr");
            newItem.Click += MenuItem_Click;
            menuRow.DropDownItems.Insert(3, newItem);
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            // Needed to launch this in a new thread to fix the text entry issues.
            // https://www.codeproject.com/questions/495662/canplusnotplusenterplusaplustextplusinsideplustext

            if (!this.AccessToToolAllowed())
                return;

            _wpfThread = new Thread(new ThreadStart(WpfForm));
            if (this._wpfThread == null)
                return;

            _wpfThread.SetApartmentState(ApartmentState.STA);
            _wpfThread.IsBackground = false;
            _wpfThread.Start();
        }

        private void WpfForm()
        {
            var window = new MainWindow();
            window.Show();
            System.Windows.Threading.Dispatcher.Run();
        }
    }
}
