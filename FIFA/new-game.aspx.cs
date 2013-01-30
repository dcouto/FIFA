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
		User _currentUser;
		User CurrentUser {
			get {
				if(_currentUser == null) {
					using(FIFADataContext db = new FIFADataContext()) {
						_currentUser = (from u in db.Users
										select u).FirstOrDefault();
					}
				}

				return _currentUser;
			}
		}

		string CurrentMatchGuid {
			get {
				return (string)ViewState["CurrentMatchGuid"];
			}
			set {
				ViewState["CurrentMatchGuid"] = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e) {
			if (!IsPostBack) {
				SetControls();
			}
		}

		protected void SetControls() {
			// my division ddl
			if (CurrentUser != null) {
				ddlDivision.SelectValue(CurrentUser.Division.ToString());
			}
			
			using (FIFADataContext db = new FIFADataContext()) {
				// teams drop-down lists
				var teams = from t in db.Teams
							orderby t.Name
							select t;

				foreach (Team t in teams) {
					ListItem li = new ListItem(t.Name, t.ID.ToString());

					ddlPlayer1Team.Items.Add(li);
					ddlPlayer2Team.Items.Add(li);
				}


				// formations drop-down lists
				var formations = from f in db.Formations
								 orderby f.Formation1
								 select f;

				foreach (Formation f in formations) {
					ListItem li = new ListItem(f.Formation1, f.ID.ToString());

					ddlPlayer1Formation.Items.Add(li);
					ddlPlayer2Formation.Items.Add(li);
				}
			}
		}

		protected void btnStartMatch_Click(object sender, EventArgs e) {
			phBtnReset.Visible = false;
			btnStartMatch.Visible = false;

			using (FIFADataContext db = new FIFADataContext()) {
				Match match = new Match {
					Player1 = CurrentUser.ID,
					Player1Team = GetTeamGuid(ddlPlayer1Team, txtPlayer1NewTeam),
					Player1Formation = GetFormationGuid(ddlPlayer1Formation, txtPlayer1NewFormation),
					Player2 = GetPlayerGuid(),
					Player2Team = GetTeamGuid(ddlPlayer2Team, txtPlayer2NewTeam),
					Player2Formation = GetFormationGuid(ddlPlayer2Formation, txtPlayer2NewFormation)
				};
				
				if (match != null) {
					db.Matches.InsertOnSubmit(match);
					db.SubmitChanges();
				}

				CurrentMatchGuid = match.ID.ToString();
			}

			btnStopMatch.Visible = true;
		}

		private Guid GetTeamGuid(DropDownList ddl, TextBox txt) {
			Guid guid = new Guid();
			
			if (txt.Text != "") {
				using (FIFADataContext db = new FIFADataContext()) {
					Team team = (from t in db.Teams
								where t.Name == txt.Text
								select t).FirstOrDefault();

					if (team == null) {
						team = new Team { Name = txt.Text };

						db.Teams.InsertOnSubmit(team);
						db.SubmitChanges();
					}

					guid = team.ID;
				}
			}
			else {
				guid = new Guid(ddl.SelectedValue);
			}

			return guid;
		}

		private Guid GetFormationGuid(DropDownList ddl, TextBox txt) {
			Guid guid = new Guid();

			if (txt.Text != "") {
				using (FIFADataContext db = new FIFADataContext()) {
					Formation formation = (from f in db.Formations
										   where f.Formation1 == txt.Text
										   select f).FirstOrDefault();

					if (formation == null) {
						formation = new Formation { Formation1 = txt.Text };

						db.Formations.InsertOnSubmit(formation);
						db.SubmitChanges();
					}

					guid = formation.ID;
				}
			}
			else {
				guid = new Guid(ddl.SelectedValue);
			}

			return guid;
		}

		private Guid GetPlayerGuid() {
			Guid guid = new Guid();

			using (FIFADataContext db = new FIFADataContext()) {
				Player player = (from p in db.Players
								 where p.GamerTag == txtPlayer2GamerTag.Text
								 select p).FirstOrDefault();

				if(player == null) {
					player = new Player { GamerTag = txtPlayer2GamerTag.Text };

					db.Players.InsertOnSubmit(player);
					db.SubmitChanges();
				}

				guid = player.ID;
			}

			return guid;
		}

		protected void btnStopMatch_Click(object sender, EventArgs e) {
			btnStopMatch.Visible = false;
			
			phScore.Visible = true;
			btnFinalizeMatch.Visible = true;
		}

		protected void btnFinalizeMatch_Click(object sender, EventArgs e) {
			phScore.Visible = false;

			using (FIFADataContext db = new FIFADataContext()) {
				var match = (from m in db.Matches
							 where m.ID.ToString() == CurrentMatchGuid
							 select m).FirstOrDefault();

				match.MatchEnd = DateTime.Now;

				match.Player1Goals = int.Parse(txtPlayer1Goals.Text);
				match.Player2Goals = int.Parse(txtPlayer2Goals.Text);

				db.SubmitChanges();
			}
			
			pnlMatchFinalized.Visible = true;
		}
	}
}