using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FIFA.COM.DataContext;
using FIFA.COM;

namespace FIFA
{
	public partial class new_game : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e) {
			if (!IsPostBack) {
				SetControls();
			}
		}

		protected void SetControls() {
			using (FIFADataContext db = new FIFADataContext()) {
				// my division ddl
				var user = (from u in db.Users
							select u).FirstOrDefault();

				if (user != null) {
					ddlDivision.SelectValue(user.Division.ToString());
				}


				// teams ddls
				var teams = from t in db.Teams
							orderby t.Name
							select t;

				foreach (var t in teams) {
					ddlMyTeam.Items.Add(t.Name);
					ddlOpponentsTeam.Items.Add(t.Name);
				}
			}
		}

		protected void btnStartMatch_Click(object sender, EventArgs e) {
			phBtnReset.Visible = false;
			btnStartMatch.Visible = false;

			btnStopMatch.Visible = true;
		}

		protected void btnStopMatch_Click(object sender, EventArgs e) {
			btnStopMatch.Visible = false;
			phScore.Visible = true;
		}

		protected void btnFinalizeMatch_Click(object sender, EventArgs e) {
			phScore.Visible = false;
			
			lblMatchFinalized.Visible = true;
		}
	}
}