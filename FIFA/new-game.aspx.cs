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

		protected void Page_Load(object sender, EventArgs e) {
			if (!IsPostBack) {
				SetControls();
			}
		}

		protected void ddlPlayer1TeamType_SelectedIndexChanged(object sender, EventArgs e) {
			phPlayer1ClubTeam.Visible = false;
			phPlayer1NationalTeam.Visible = false;

			using (FIFADataContext db = new FIFADataContext()) {
				// club
				if (ddlPlayer1TeamType.SelectedValue == "Club") {
					phPlayer1ClubTeam.Visible = true;

					// leagues ddl
					if (ddlPlayer1ClubLeague.Items.Count == 1) {
						var leagues = from l in db.Leagues
									  orderby l.Name
									  select l;

						foreach (var l in leagues) {
							ddlPlayer1ClubLeague.Items.Add(new ListItem(l.Name, l.ID.ToString()));
						}
					}

					// teams ddl
					if (ddlPlayer1ClubTeam.Items.Count == 1) {
						var teams = from t in db.Teams
									orderby t.Name
									select t;

						foreach (var t in teams) {
							ddlPlayer1ClubTeam.Items.Add(new ListItem(t.Name, t.ID.ToString()));
						}
					}

					// formations ddl
					if (ddlPlayer1ClubFormation.Items.Count == 1) {
						var formations = from f in db.Formations
										 orderby f.Formation1
										 select f;

						foreach (var f in formations) {
							ddlPlayer1ClubFormation.Items.Add(new ListItem(f.Formation1, f.ID.ToString()));
						}
					}
				}
				// national team
				else if (ddlPlayer1TeamType.SelectedValue == "National") {
					phPlayer1NationalTeam.Visible = true;

					// teams ddl
					if (ddlPlayer1NationalTeam.Items.Count == 1) {
						var countries = from c in db.Countries
										orderby c.Name
										select c;

						foreach (var c in countries) {
							ddlPlayer1NationalTeam.Items.Add(c.Name);
						}
					}

					// formations ddl
					if (ddlPlayer1NationalFormation.Items.Count == 1) {
						var formations = from f in db.Formations
										 orderby f.Formation1
										 select f;

						foreach (var f in formations) {
							ddlPlayer1NationalFormation.Items.Add(new ListItem(f.Formation1, f.ID.ToString()));
						}
					}
				}
			}
		}

		protected void ddlPlayer2TeamType_SelectedIndexChanged(object sender, EventArgs e) {
			phPlayer2ClubTeam.Visible = false;
			phPlayer2NationalTeam.Visible = false;

			using (FIFADataContext db = new FIFADataContext()) {
				if (ddlPlayer2TeamType.SelectedValue == "Club") {
					phPlayer2ClubTeam.Visible = true;

					// leagues ddl
					if (ddlPlayer2ClubLeague.Items.Count == 1) {
						var leagues = from l in db.Leagues
									  orderby l.Name
									  select l;

						foreach (var l in leagues) {
							ddlPlayer2ClubLeague.Items.Add(new ListItem(l.Name, l.ID.ToString()));
						}
					}

					// teams ddl
					if (ddlPlayer2ClubTeam.Items.Count == 1) {
						var teams = from t in db.Teams
									orderby t.Name
									select t;

						foreach (var t in teams) {
							ddlPlayer2ClubTeam.Items.Add(new ListItem(t.Name, t.ID.ToString()));
						}
					}

					// formations ddl
					if (ddlPlayer2ClubFormation.Items.Count == 1) {
						var formations = from f in db.Formations
										 orderby f.Formation1
										 select f;

						foreach (var f in formations) {
							ddlPlayer2ClubFormation.Items.Add(new ListItem(f.Formation1, f.ID.ToString()));
						}
					}
				}
				else if (ddlPlayer2TeamType.SelectedValue == "National") {
					phPlayer2NationalTeam.Visible = true;

					if (ddlPlayer2NationalTeam.Items.Count == 1) {
						var countries = from c in db.Countries
										orderby c.Name
										select c;

						foreach (var c in countries) {
							ddlPlayer2NationalTeam.Items.Add(c.Name);
						}
					}
				}
			}
		}

		protected void SetControls() {
			using (FIFADataContext db = new FIFADataContext()) {
				// my division ddl
				if (CurrentUser != null) {
					ddlDivision.SelectValue(CurrentUser.Division.ToString());
				}
			}
		}

		protected void btnStartMatch_Click(object sender, EventArgs e) {
			phBtnReset.Visible = false;
			btnStartMatch.Visible = false;

			using (FIFADataContext db = new FIFADataContext()) {
				Match match = null;
				
				// player 1
				// club
				if(phPlayer1ClubTeam.Visible) {
					if (match == null)
						match = new Match();

					match.Player1 = CurrentUser.ID;
					match.Player1Team = new Guid(ddlPlayer1ClubTeam.SelectedValue);
					match.Player1Formation = new Guid(ddlPlayer1ClubFormation.SelectedValue);
						
				}
				// national
				else {
					if (match == null)
						match = new Match();

					match.Player1 = CurrentUser.ID;
					match.Player1Team = new Guid(ddlPlayer1NationalTeam.SelectedValue);
					match.Player1Formation = new Guid(ddlPlayer1NationalFormation.SelectedValue);
				}


				// player 2
				match.Player2 = GetPlayerByGamerTag();

				// club
				if(phPlayer2ClubTeam.Visible) {
					match.Player2Team = new Guid(ddlPlayer2ClubTeam.SelectedValue);
					match.Player2Formation = new Guid(ddlPlayer2ClubFormation.SelectedValue);
				}
				// national
				else {
					match.Player2Team = new Guid(ddlPlayer2NationalTeam.SelectedValue);
					match.Player2Formation = new Guid(ddlPlayer2NationalFormation.SelectedValue);
				}


				if (match != null) {
					db.Matches.InsertOnSubmit(match);
					db.SubmitChanges();
				}
			}

			btnStopMatch.Visible = true;
		}

		protected void btnStopMatch_Click(object sender, EventArgs e) {
			btnStopMatch.Visible = false;
			
			phScore.Visible = true;
			btnFinalizeMatch.Visible = true;
		}

		protected void btnFinalizeMatch_Click(object sender, EventArgs e) {
			phScore.Visible = false;
			
			pnlMatchFinalized.Visible = true;
		}

		private Guid GetPlayer1Team() {
			Guid guid = new Guid();

			//if (txtPlayer1NewTeam.Text != "") {
			//	using (FIFADataContext db = new FIFADataContext()) {
			//		Team team = new Team { Name = txtPlayer1NewTeam.Text, 
			//	}
			//}

			return guid;
		}

		private Guid GetPlayer1League() {
			Guid guid = new Guid();

			//using (FIFADataContext db = new FIFADataContext()) {
			//	if (txtPlayer1NewLeague.Text != "") {
					
			//	}
			//	else {

			//	}
			//}

			return guid;
		}

		private Guid GetPlayerByGamerTag() {
			Guid guid = new Guid();
			
			using (FIFADataContext db = new FIFADataContext()) {
				var player = (from p in db.Players
							  where p.GamerTag == txtPlayer2GamerTag.Text
							  select p).FirstOrDefault();

				if (player != null) {
					guid = player.ID;
				}
				else {
					Player newPlayer = new Player { GamerTag = txtPlayer2GamerTag.Text };

					db.Players.InsertOnSubmit(newPlayer);
					db.SubmitChanges();

					guid = newPlayer.ID;
				}
			}

			return guid;
		}
	}
}