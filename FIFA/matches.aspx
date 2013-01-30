<%@ Page Title="" Language="C#" MasterPageFile="~/FIFA.Master" AutoEventWireup="true" CodeBehind="matches.aspx.cs" Inherits="FIFA.matches" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphH1" runat="server">Matches</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMain" runat="server">
	<asp:ScriptManager runat="server"></asp:ScriptManager>

	<telerik:RadGrid ID="trgMatches" OnNeedDataSource="trgMatches_NeedDataSource" AutoGenerateColumns="true" runat="server"></telerik:RadGrid>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphFooter" runat="server">
</asp:Content>
