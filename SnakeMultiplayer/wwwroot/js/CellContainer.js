﻿class CellGridContainer {
    previousHead = null
    constructor(gridSize, baseCellParams, canvasCtx, startBorder, endBorder) {
        this.gridSize = gridSize;
        this.baseCellParams = baseCellParams;
        this.canvasCtx = canvasCtx;
        this.startBorder = startBorder;
        this.endBorder = endBorder;

        this.initializeDecorators();
    }

    initializeDecorators() {
        try {
            this.outlineDecorator = new CrownOutlineDecorator(this.canvasCtx, this.baseCellParams.size, this.startBorder);
            this.coloredDecorator = new ColoredCrownDecorator(this.canvasCtx, this.baseCellParams, this.startBorder, this.outlineDecorator);
            this.fullCrownDecorator = new FullCrownDecorator(this.canvasCtx, this.baseCellParams, this.startBorder, this.coloredDecorator);
        } catch (error) {
            console.error(error, "An error occurred while trying to initialize decorators")
        }
    }

    createGrid(draw = true) {
        this.Cells = new Array(this.gridSize);
        var x, y;
        for (x = 0; x < this.gridSize; x++) {
            this.Cells[x] = new Array(this.gridSize);
            for (y = 0; y < this.gridSize; y++) {
                this.Cells[x][y] = new Cell(this.baseCellParams.size, this.baseCellParams.innerColor, this.baseCellParams.outlineColor, x, y);
                if (draw === true) {
                    this.drawCustomCell(this.Cells[x][y]);
                }
            }
        }
    }

    drawGrid() {
        var x, y;
        for (x = 0; x < this.gridSize; x++) {
            for (y = 0; y < this.gridSize; y++) {
                this.drawCustomCell(this.Cells[x][y]);
            }
        }
    }

    clearCoords(coordinateArray) {
        if (coordinateArray == null)
            return;
        var i;
        for (i = 0; i < coordinateArray.length; i++) {
            this.drawBaseCell(coordinateArray[i].x, coordinateArray[i].y);
        }
    }

    initializeSnake(snakeBody, color, shape) {
        //iterate through snakeBody

        var i;
        for (i = 0; i < snakeBody.length; i++) {
            this.updateSnake(color, snakeBody[i], null, null, shape);
        }
    }

    getRandomColor() {
        const letters = '0123456789ABCDEF';
        let color = '#';
        for (let i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    }

    getCrownDecorator(crownStage) {
        console.log(crownStage)
        switch (crownStage) {
            case "FullWithJewels":
                return this.fullCrownDecorator;
            case "Colored":
                return this.coloredDecorator;
            case "Outline":
                return this.outlineDecorator;
            default:
                return null;
        }
    }

    updateSnake(snakeColor, head, tail = null, striped = false, shape, crownStage = null) {
        if (this.previousHead) {
            if (striped) {
                this.drawCell(this.previousHead.x, this.previousHead.y, this.getRandomColor(), shape);
            } else {
                this.drawBaseCell(this.previousHead.x, this.previousHead.y, "white", "square");
                this.drawCell(this.previousHead.x, this.previousHead.y, snakeColor, shape);
            }
        }

        if (striped) {
            this.drawCell(head.x, head.y, this.getRandomColor(), shape);
        } else {
            this.drawBaseCell(head.x, head.y, "white", "square"); 
            this.drawCell(head.x, head.y, snakeColor, shape); 

            const crownDecorator = this.getCrownDecorator(crownStage);
            if (crownDecorator) {
                crownDecorator.drawCrown(head.x, head.y);
            }
        }
        if (tail !== null) {
            this.drawCell(tail.x, tail.y, this.baseCellParams.innerColor, "square");
        }

        this.previousHead = { x: head.x, y: head.y };
    }

    drawShape(x, y, fillColor, outlineColor, shape) {
        var coordx = this.getCellCoord(x);
        var coordy = this.getCellCoord(y);

        switch (shape) {
            case "circle":
                DrawFillCircle(coordx + this.baseCellParams.size / 2, coordy + this.baseCellParams.size / 2, this.baseCellParams.size / 2, fillColor);
                DrawOutlineCircle(coordx + this.baseCellParams.size / 2, coordy + this.baseCellParams.size / 2, this.baseCellParams.size / 2, outlineColor);
                break;

            case "triangle":
                DrawFillTriangle(coordx + this.baseCellParams.size / 2, coordy + this.baseCellParams.size / 2, this.baseCellParams.size, fillColor);
                DrawOutlineTriangle(coordx + this.baseCellParams.size / 2, coordy + this.baseCellParams.size / 2, this.baseCellParams.size, outlineColor);
                break;

            case "ellipse":
                DrawFillEllipse(coordx + this.baseCellParams.size / 2, coordy + this.baseCellParams.size / 2, this.baseCellParams.size / 2, this.baseCellParams.size / 4, fillColor);
                DrawOutlineEllipse(coordx + this.baseCellParams.size / 2, coordy + this.baseCellParams.size / 2, this.baseCellParams.size / 2, this.baseCellParams.size / 4, outlineColor);
                break;
            
            case "polygon":
                DrawFillPolygon(coordx + this.baseCellParams.size / 2, coordy + this.baseCellParams.size / 2, this.baseCellParams.size / 2, 8, fillColor);
                DrawOutlinePolygon(coordx + this.baseCellParams.size / 2, coordy + this.baseCellParams.size / 2, this.baseCellParams.size / 2, 8, outlineColor);
                break;

            default: 
                DrawFillRenctangle(coordx, coordy, this.baseCellParams.size, fillColor);
                DrawOutlineRectangle(coordx, coordy, this.baseCellParams.size, outlineColor);
                break;
        }
    }

    drawCell(x, y, fillColor, shape) {
        this.drawShape(x, y, fillColor, this.baseCellParams.outlineColor, shape);
    }

    drawBaseCell(x, y, shape) {
        this.drawShape(x, y, this.baseCellParams.innerColor, this.baseCellParams.outlineColor, shape);
    }

    drawCustomCell(cell, shape) {
        this.drawShape(cell.x, cell.y, cell.innerColor, cell.outlineColor, shape);
    }

    getCellCoord(cellNumber) {
        return this.startBorder + (cellNumber * this.baseCellParams.size);
    }

    drawGameOver() {
        var gameOverY = this.getCellCoord(7);
        var gameOverX = this.getCellCoord(4);
        var fontSize = (BRborder- TLborder) / 10;
        DrawText("Game Over", gameOverY, gameOverY, fontSize);
    }
}