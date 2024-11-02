class Snake {
    constructor(name, color, stripes, shape) {
        this.body = new CustomLinkedList();
        this.name = name;
        this.color = color;
        this.isStriped = stripes;
        this.shape = shape;
    }

    setStartPoint(x, y) {
        this.currDirection = MoveDirection.Down;
        this.body.addFirst(new Cell(baseCell.size, this.color, baseCell.outlineColor, x, y));
    }

    // Always returns added head coordinates AND deleted tail coordinates.
    update(direction, isFood) {
        var newHead = this.body.getFirst().getCopy();
        newHead.update(direction);

        this.body.addFirst(newHead);

        if (isFood === true) {
            return { head: newHead, tail: null };
        } else {
            return { head: newHead, tail: this.body.deleteLast() };
        }
    }

    updateCoord(head, tail) {
        this.body.addFirst(head);

        if (tail != null)
            this.body.deleteLast();
    }

    getBodyArray() {
        return this.body.getArray();
    }
}

class GameController {
    constructor(playerName, lobbyId, mainDispatcher) {
        this.snakes = {};
        this.snakeCount = 0;
        this.mainDispatcher = mainDispatcher;
        this.socketDispatcher = new Dispatcher();
        this.socketDispatcher.on("onSocketOpen", this.onOpenedSocket.bind(this));
        this.socketDispatcher.on("onSocketMessage", this.onMessageReceived.bind(this));
        this.socketDispatcher.on("onSocketClose", this.onMessageReceived.bind(this));
        this.socketDispatcher.on("onSocketError", this.onMessageReceived.bind(this));
        this.playerName = playerName;
        this.lobbyId = lobbyId;
        this.signalRController = new SignalRController(this.playerName, this.lobbyId, this.socketDispatcher);
        this.signalRController.connect();
    }

    onOpenedSocket(e) {
        console.log("Socket opened");
        this.sendPlayerListRequest();
        var settings = { cellCount: this.cellContainer.gridSize, isWall: false, speed: "Normal" };
        this.sendSettingUpdate(settings);
        this.mainDispatcher.dispatch("onWebSocketOpened");
    }

    onMessageReceived(message) {
        console.log("GameController received message:", message);

        switch (message.type) {
            case "Players":
                // Show all current players
                var playerUpdate = message.body.players;
                console.log("New player list: ", playerUpdate);
                this.mainDispatcher.dispatch("onPlayerListReceived", playerUpdate);
                var removedPlayer = message.body.removed;
                if (removedPlayer != null) {

                    this.cellContainer.clearCoords(this.snakes[removedPlayer].getBodyArray());
                    this.snakes[removedPlayer] = null;
                }

                break;
            case "Update":
                // Update game state
                this.HandleUpdate(message.body.status, this.playerName);
                break;
            case "Settings":
                this.mainDispatcher.dispatch("onSettingsReceived", message.body.settings);
                break;
            case "Start":
                // Raise game started event
                this.mainDispatcher.dispatch("onStartReceived");
                // Do count down

                // Initialize positions
                this.HandleStart(message.body.start);
                break;
            case "End":
                // close web socket connection, throw error.
                this.cellContainer.drawGameOver();
                this.mainDispatcher.dispatch("onGameEndReceived");
                break;
            case "Exit":
                this.mainDispatcher.dispatch("onExitReceived", message.body);
                break;
        }
    }

    HandleStart(startMessage) {
        this.cellContainer.drawGrid();
        var food = startMessage.food;
        if (food !== null) {
            this.cellContainer.drawCell(food.x, food.y, "black");
        }

        // Draw obstacles
        const obstacles = startMessage.obstacles;
        if (obstacles) {
            for (const obstacle of obstacles) {
                this.cellContainer.drawCell(obstacle.position.x, obstacle.position.y, obstacle.color);
            }
        }

        const strategyCell = startMessage.strategyCell;
        if (strategyCell) {
            this.cellContainer.drawCell(strategyCell.position.x, strategyCell.position.y, strategyCell.color)
        }

        var snakesArray = startMessage.activeSnakes;
        var i;
        for (i = 0; i < snakesArray.length; i++) {
            var head = snakesArray[i].head;
            var tail = snakesArray[i].tail;
            var player = snakesArray[i].player;
            var color = snakesArray[i].color;
            var striped = snakesArray[i].isStriped;
            var shape = snakesArray[i].shape;

            //this.cellContainer.updateSnake(color, head, tail);
            var snake = new Snake(player, color, striped, shape);
            snake.setStartPoint(head.x, head.y);
            //this.snakes.push(snake);
            this.snakes[player] = snake;
        }
        this.drawSnakes();
    }

    HandleUpdate(updateMessage, playerName) {
        var food = updateMessage.food;
        if (food !== null) {
            this.cellContainer.drawCell(food.x, food.y, "black");
        }
        const strategyCell = updateMessage.strategyCell;
        if (strategyCell) {
            this.cellContainer.drawCell(strategyCell.position.x, strategyCell.position.y, strategyCell.color)
        }

        // Update active snakes
        var snakesArray = updateMessage.activeSnakes;
        var i;
        for (i = 0; i < snakesArray.length; i++) {
            var head = snakesArray[i].head;
            var tail = snakesArray[i].tail;
            var player = snakesArray[i].player;
            var color = snakesArray[i].color;
            var score = snakesArray[i].score;
            var striped = snakesArray[i].isStriped;
            var movementStrategy = snakesArray[i].movementStrategy;
            var shape = snakesArray[i].shape;
            console.log(`Updating score for ${player}: ${score}`);
            console.log(`Movement strat: ${movementStrategy}`)
            console.log(`Logged in player: ${playerName}`)


            this.snakes[player].updateCoord(head, tail);
            this.cellContainer.updateSnake(color, head, tail, striped, shape);
            updatePlayerScore(player, score); // Update the player's score
            displayMovementStrategy(playerName, player, movementStrategy);
        }

        // Unpaint inactive snakes
        var snakesArray = updateMessage.disabledSnakes;
        for (i = 0; i < snakesArray.length; i++) {
            var player = snakesArray[i];
            console.log(`Disabling snake: ${player}`);
            if (this.snakes[player] != null) {
                this.cellContainer.clearCoords(this.snakes[player].getBodyArray());
                this.snakes[player] = null;
                
            }
        }

        // Paint snakes to revive
        var snakesToRevive = updateMessage.snakesToRevive;
        for (i = 0; i < snakesToRevive.length; i++) {
            console.log(`Reviving snake: ${snakesToRevive[i]}`);
            var player = snakesToRevive[i];            
            var head = snakesToRevive[i].head;
            var tail = snakesToRevive[i].tail;
            var player = snakesToRevive[i].player;
            var bodyArray = snakesToRevive[i].body;
            var color = snakesToRevive[i].color;
            var striped = snakesToRevive[i].isStriped;
            var shape = snakesToRevive[i].shape;

            var snake = new Snake(player, color, striped, shape);
            snake.setStartPoint(head.x, head.y);
            for (var j = 0; j < bodyArray.length; j++) {
                snake.body.addFirst(bodyArray[j]);
            }
            this.snakes[player] = snake;
            this.snakes[player].updateCoord(head, tail);
            this.cellContainer.updateSnake(color, head, tail, striped, shape);
            this.drawSnake(player);
        }
    }

    setEnvironment() {
        onResize();
    }

    setCellContainer(container) {
        this.cellContainer = container;
        this.cellContainer.createGrid(false);
        this.cellContainer.drawGrid();
    }

    drawElements() {
        this.cellContainer.createGrid(false);
        this.cellContainer.drawGrid();
        this.drawSnakes();
    }

    raiseOnOpen() {

    }

    createSnake() {

    }

    moveSnake(direction) {

    }

    //TODO: Is this needed?
    sendPlayerListRequest() {
        var messageBody = "";
        //this.socketController.send("Players", JSON.stringify(messageBody));
    }

    sendMovementUpdate(direction) {
        this.signalRController.updatePlayerState(direction);
    }

    sendGameStart() {
        this.signalRController.initiateGameStart();
    }

    sendSettingUpdate(settings) {
        this.signalRController.updateLobbySettings(settings);
    }

    drawSnakes() {
        for (var key in this.snakes) {
            var snake = this.snakes[key];
            if (snake != null) {
                this.cellContainer.initializeSnake(this.snakes[key].getBodyArray(), this.snakes[key].color, this.snakes[key].shape);
            }
        }
    }

    
    drawSnake(key) {
        // Draw an existing snake (used for reviving a snake)
        var snake = this.snakes[key];
        if (snake != null) {
            console.log(`Snake body: ${JSON.stringify(snake.getBodyArray())}`);
            this.cellContainer.initializeSnake(this.snakes[key].getBodyArray(), this.snakes[key].color,this.snakes[key].shape);
        }
    }
}


function htmlEscape(str) {
    return str.toString()
        .replace(/&/g, '&amp;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;');
}

// Function to update player scores in the DOM
function updatePlayerScore(player, score) {
    const playerScoreElement = document.getElementById(`score-${player}`);
    if (playerScoreElement) {
        playerScoreElement.textContent = `${player}: Score: ${score}`;
    } else {
        // Create a new element if it doesn't exist
        const newScoreElement = document.createElement('div');
        newScoreElement.id = `score-${player}`;
        newScoreElement.textContent = `${player}: Score: ${score}`;
        document.getElementById('scoreBoard').appendChild(newScoreElement);
    }
}

function displayMovementStrategy(currentPlayer, player, movementStrategy) {
    if (currentPlayer === player) {
        const playerMovementStrategyElement = document.getElementById(`movement-${player}`);
        if (playerMovementStrategyElement) {
            playerMovementStrategyElement.textContent = `Current moving strategy: ${movementStrategy}`;
        } else {
            const newPlayerMovementStrategyElement = document.createElement('div');
            newPlayerMovementStrategyElement.id = `movement-${player}`;
            newPlayerMovementStrategyElement.textContent = `Current moving strategy: ${movementStrategy}`;
            document.getElementById('currentPlayerMovementStrategy').appendChild(newPlayerMovementStrategyElement);
        }
    }
}