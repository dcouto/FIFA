<%@ Page Title="" Language="C#" MasterPageFile="~/FIFA.Master" AutoEventWireup="true" CodeBehind="new-game.aspx.cs" Inherits="FIFA.new_game" %>

<asp:Content ContentPlaceHolderID="cphH1" runat="server">New Game</asp:Content>

<asp:Content ContentPlaceHolderID="cphMain" runat="server">
	<label class="my-division">
		My current division: <asp:DropDownList ID="ddlDivision" runat="server"></asp:DropDownList>
	</label>
	<em>To update your division, change it above.  It will be saved to your profile when you click the Start Match button below.</em>
	
	<section class="my-team">
		<h2>Me</h2>

		<asp:DropDownList ID="ddlMyTeam" runat="server">
			<asp:ListItem Value="- Select your team -"></asp:ListItem>
		</asp:DropDownList>

		<asp:DropDownList ID="ddlMyFormation" runat="server">
			<asp:ListItem Value="- Select your formation -"></asp:ListItem>
		</asp:DropDownList>
	</section>

	<section class="opponents-team">
		<h2>Opponent</h2>

		<asp:TextBox ID="txtOpponentsGamerTag" placeholder="Opponents Gamer Tag" runat="server"></asp:TextBox>

		<asp:DropDownList ID="ddlOpponentsTeam" runat="server">
			<asp:ListItem Value="- Select your opponent's team -"></asp:ListItem>
		</asp:DropDownList>

		<asp:DropDownList ID="ddlOpponentsFormation" runat="server">
			<asp:ListItem Value="- Select your opponent's formation -"></asp:ListItem>
		</asp:DropDownList>
	</section>

	<div class="clear"></div>

	<input type="reset" value="Reset" />

	<asp:Button ID="btnStartMatch" CssClass="btn-start-match" Text="Start Match" runat="server" />
</asp:Content>