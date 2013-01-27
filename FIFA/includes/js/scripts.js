$(function () {
	var myTeam = $('.my-team');
	var opponentsTeam = $('.opponents-team');

	if (myTeam.height() > opponentsTeam.height()) {
		opponentsTeam.height(myTeam.height());
	}
	else {
		myTeam.height(opponentsTeam.height());
	}
});