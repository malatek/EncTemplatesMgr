using EllieMae.Encompass.Automation;
using EllieMae.Encompass.Forms;
using System;
using System.Threading;

namespace EncTemplatesMgr.View
{
    public partial class Codebase : Form
    {
        private Button _btnLaunchForm;
        private Thread _wpfThread;

        public override void CreateControls()
        {
            base.CreateControls();
            this.LoadControls();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void LoadControls()
        {
            this._btnLaunchForm = (Button)this.FindControl("btnLaunchForm");
            this._btnLaunchForm.Click += new EventHandler(BtnLaunchForm_Click);
        }

        private void BtnLaunchForm_Click(object sender, EventArgs e)
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

        private bool AccessToToolAllowed()
        {
            var currentUser = EncompassApplication.CurrentUser;
            if (currentUser.Personas.Contains(EncompassApplication.Session.Users.Personas.GetPersonaByName("Super Administrator")) || currentUser.ID == "admin")
                return true;

            return false;
        }
    }
}
