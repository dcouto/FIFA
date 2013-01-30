<%@ Page Title="" Language="C#" MasterPageFile="~/FIFA.Master" AutoEventWireup="true" CodeBehind="new-game.aspx.cs" Inherits="FIFA.new_game" %>

<%--<asp:Content ContentPlaceHolderID="head" runat="server">
	<link href="/includes/less/jquery.stopwatch.less" rel="Stylesheet" media="all" />
</asp:Content>--%>

<asp:Content ContentPlaceHolderID="cphH1" runat="server">New Game</asp:Content>

<asp:Content ContentPlaceHolderID="cphMain" runat="server">
	<label class="my-division">
		My current division:
		<asp:DropDownList ID="ddlDivision" runat="server">
			<asp:ListItem Value="1"></asp:ListItem>
			<asp:ListItem Value="2"></asp:ListItem>
			<asp:ListItem Value="3"></asp:ListItem>
			<asp:ListItem Value="4"></asp:ListItem>
			<asp:ListItem Value="5"></asp:ListItem>
			<asp:ListItem Value="6"></asp:ListItem>
			<asp:ListItem Value="7"></asp:ListItem>
			<asp:ListItem Value="8"></asp:ListItem>
			<asp:ListItem Value="9"></asp:ListItem>
			<asp:ListItem Value="10"></asp:ListItem>
		</asp:DropDownList>
	</label>
	<em>To update your division, change it above.  It will be saved to your profile when you click the Start Match button below.</em>
	
	<section class="player">
		<h2>Me</h2>

		<asp:DropDownList ID="ddlPlayer1Team" runat="server">
			<asp:ListItem Value="- Select your team -"></asp:ListItem>
		</asp:DropDownList>
		<asp:TextBox ID="txtPlayer1NewTeam" placeholder="Enter a new team" runat="server"></asp:TextBox>

		<asp:DropDownList ID="ddlPlayer1Formation" runat="server">
			<asp:ListItem Value="- Select your formation -"></asp:ListItem>
		</asp:DropDownList>
		<asp:TextBox ID="txtPlayer1NewFormation" placeholder="Enter a new formation" runat="server"></asp:TextBox>
	</section>

	<section class="player">
		<h2>Opponent</h2>

		<asp:TextBox ID="txtPlayer2GamerTag" placeholder="Opponents Gamer Tag" runat="server"></asp:TextBox>

		<asp:DropDownList ID="ddlPlayer2Team" runat="server">
			<asp:ListItem Value="- Select your team -"></asp:ListItem>
		</asp:DropDownList>
		<asp:TextBox ID="txtPlayer2NewTeam" placeholder="Enter a new team" runat="server"></asp:TextBox>

		<asp:DropDownList ID="ddlPlayer2Formation" runat="server">
			<asp:ListItem Value="- Select your formation -"></asp:ListItem>
		</asp:DropDownList>
		<asp:TextBox ID="txtPlayer2NewFormation" placeholder="Enter a new formation" runat="server"></asp:TextBox>
	</section>

	<div class="clear"></div>

	<asp:PlaceHolder ID="phBtnReset" runat="server">
		<input type="reset" class="btn-reset" value="Reset" />
	</asp:PlaceHolder>
	
	<asp:Button ID="btnStartMatch" CssClass="btn-start-match" OnClick="btnStartMatch_Click" Text="Start Match" runat="server" />
	
	<asp:Button ID="btnStopMatch" CssClass="btn-stop-match" OnClick="btnStopMatch_Click" Text="Stop Match" Visible="false" runat="server" />

	<%--<section class="match-time">Match time: <div class="stopwatch"></div></section>--%>

	<asp:PlaceHolder ID="phScore" Visible="false" runat="server">
		<section class="score">
			<h2>Score</h2>
			<asp:TextBox ID="txtPlayer1Goals" runat="server"></asp:TextBox> x <asp:TextBox ID="txtPlayer2Goals" runat="server"></asp:TextBox>
		</section>
	</asp:PlaceHolder>

	<asp:Button ID="btnFinalizeMatch" CssClass="btn-finalize-match" OnClick="btnFinalizeMatch_Click" Text="Finalize match" Visible="false" runat="server" />

	<asp:Panel ID="pnlMatchFinalized" CssClass="match-finalized" Visible="false" runat="server">Match finalized!</asp:Panel>
</asp:Content>

<%--<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
	<script type="text/javascript" src="/includes/js/jquery.stopwatch.js"></script>

	<script type="text/javascript">
		$(function () {
			$('.stopwatch').stopwatch();

			//var matchTime = $('.match-time');

			//$('.btn-start-match').click(function () {
				//matchTime.show();

			//});
		});
	</script>
</asp:Content>--%>