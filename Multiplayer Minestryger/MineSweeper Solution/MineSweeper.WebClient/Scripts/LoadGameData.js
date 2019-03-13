
var firstRun = true;
var htmlButtonHolder = [];


function UpdateBoard(boardData, localPlayer) {
	for (var y = 0; y < boardData['Height']; y++) {
		for (var x = 0; x < boardData['Width']; x++) {
			var currentButtonId = [x, y].join('_');
			var currentButton = document.getElementById(currentButtonId)
			var fieldObject = boardData["JaggedMinefield"][y][x];
			if (fieldObject["IsPressed"] && !fieldObject["IsMine"]) {
				if (fieldObject["AdjacentMines"] != "0") {
					currentButton.innerHTML = fieldObject["AdjacentMines"];
				}
				if (fieldObject["PressedByPlayer"] != null) {
					if (fieldObject["PressedByPlayer"]["Username"] == localPlayer) {
						currentButton.classList.add("localPress");
					} else {
						currentButton.classList.add("otherPress");
					}
				}
				
				currentButton.classList.add("blank");
				currentButton.classList.remove("flag")
				$("#" + currentButtonId).attr('disabled', 'disabled');
			}
		}
	}

	//TODO Begge spillere bliver tilføjet til oppomentplayers
	var players = boardData["OpponentPlayers"];
	for (var i = 0; i < players.length; i++) {
		var player = players[i];
		document.getElementById("player" + i).innerHTML = player["Username"];
	}
}

function CallOnEndGame(boardData) {
	for (var y = 0; y < boardData['Height']; y++) {
		for (var x = 0; x < boardData['Width']; x++) {
			var fieldObject = boardData["JaggedMinefield"][y][x];
			var currentButtonId = [x, y].join('_');
			if (fieldObject["IsMine"]) {
				$("#" + currentButtonId).addClass("bomb");
			}
			$("#" + currentButtonId).attr('disabled', 'disabled');
		}
	}
	
}

function BuildBoard(boardData) {
	console.log("BuildingBoard");
	htmlButtonHolder = [];
	htmlPlayerHolder = [];
	for (var y = 0; y < boardData['Height']; y++) {
		for (var x = 0; x < boardData['Width']; x++) {
			PlaceButton(x, y)
		}
	}
	$("#game").html(htmlButtonHolder.join(""));

	var players = boardData["OpponentPlayers"];
	for (var i = 0; i < players.length; i++) {
		var player = players[i];
		console.log(player['Username']);
		var playerID = "player" + i;
		htmlPlayerHolder.push('<p id="', playerID, '">', player['Username'], '</p>');
	}
	$("#players").html(htmlPlayerHolder.join(""));
}

function PlaceButton(x, y) {
	htmlButtonHolder.push('<button onclick="MakeMove(this)" oncontextmenu="AddFlag(this);return false;" class="field" id ="', x, "_", y, '" >  </button>')
}


function AddFlag(button) {
	console.log("Tried to place flag")
	if (button.classList.contains("flag")) {
		button.classList.remove("flag");
	} else {
		button.classList.add("flag");
	}
}

function ClearOldGameData() {
	//TODO clear spil data hvis spilleren vil spilleigen uden at forlade siden (Onjoingame-ish)
}
