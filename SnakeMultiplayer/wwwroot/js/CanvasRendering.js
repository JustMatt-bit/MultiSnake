var Canvas = document.getElementById("Canvas");
var CanvasContext = Canvas.getContext("2d");
// Constants that depend on current screen size 
var length;
var TLborder;
var BRborder;
var margin;
var baseCell = new Cell(undefined, "whitesmoke", "dimgrey");
var cellCount = 20;
var relMarginSize = 0.1;
var canvasLength = -1;

function onResize() {
    console.log(">>Resizing")
    ClearCanvas();
    ResizeCanvas();
    DrawBaseCanvas();
}

function ResizeCanvas() {
    
    if (window.innerWidth > window.innerHeight) {
        canvasLength = window.innerHeight - 5;
    } else {
        canvasLength = window.innerWidth - 5;
    }
    canvasLength = getCanvasLength(canvasLength);
    SetCanvasVariables(canvasLength);
}

function SetCanvasVariables(canvasLength) {
    Canvas.width = canvasLength;
    Canvas.height = canvasLength;
    //console.log("Canvas length: ", canvasLength);
    setOtherCanvasVariables(canvasLength);
}

/*
    Returns corrected canvas length, so that cell size would be whole number.
*/
function getCanvasLength(currentLength) {
    var arenaLength = currentLength * (1 - (relMarginSize));
    var cellSize = arenaLength / cellCount;
    baseCell.size = cellSize;
    return (Math.floor(cellSize) * cellCount) / (1 - (relMarginSize));
}

function ClearCanvas() {
    CanvasContext.clearRect(0, 0, Canvas.width, Canvas.height);
}

function setOtherCanvasVariables(canvasLength) {
    length = canvasLength;

    margin = length * 0.1;

    TLborder = length * relMarginSize * 0.5;
    BRborder = length - (TLborder * 2);
    //console.log("Real arena length:", BRborder - TLborder);
    //console.log("Cell Length: ", baseCell.size);
}

function DrawBaseCanvas() {

    console.log(TLborder, BRborder);
    CanvasContext.fillStyle = "lightgrey";
    CanvasContext.fillRect(0, 0, length, length);
    CanvasContext.stroke();

    CanvasContext.fillStyle = "white";
    CanvasContext.fillRect(TLborder, TLborder, BRborder, BRborder);
    CanvasContext.stroke();
    DrawCanvasBorder();
}
function DrawCanvasBorder() {
    CanvasContext.beginPath();
    CanvasContext.moveTo(1, 1);
    CanvasContext.lineTo(Canvas.width - 1, 1);
    CanvasContext.lineTo(Canvas.width - 1, Canvas.height - 1);
    CanvasContext.lineTo(1, Canvas.height - 1);
    CanvasContext.lineTo(1, 1);
    CanvasContext.strokeStyle = "red";
    CanvasContext.stroke();
}
function DrawFillRenctangle(x, y, length, fillColor) {
    CanvasContext.fillStyle = fillColor;
    CanvasContext.fillRect(x, y, length, length);
}

function DrawOutlineRectangle(x, y, length, outlineColor) {
    CanvasContext.strokeStyle = outlineColor;
    CanvasContext.rect(x, y, length, length);
    CanvasContext.stroke();
}
// Function to draw a filled circle
function DrawFillCircle(x, y, radius, fillColor) {
    CanvasContext.fillStyle = fillColor;
    CanvasContext.beginPath();
    CanvasContext.arc(x, y, radius, 0, Math.PI * 2, false);
    CanvasContext.fill();
}

// Function to draw an outlined circle
function DrawOutlineCircle(x, y, radius, outlineColor) {
    CanvasContext.strokeStyle = outlineColor;
    CanvasContext.beginPath();
    CanvasContext.arc(x, y, radius, 0, Math.PI * 2, false);
    CanvasContext.stroke();
}
//Triangle
function DrawFillTriangle(x, y, size, fillColor) {
    const halfSize = size / 2;
    const height = (Math.sqrt(3) / 2) * size;

    CanvasContext.fillStyle = fillColor;
    CanvasContext.beginPath();
    CanvasContext.moveTo(x, y - height / 2); // Top point
    CanvasContext.lineTo(x - halfSize, y + height / 2); // Bottom left point
    CanvasContext.lineTo(x + halfSize, y + height / 2); // Bottom right point
    CanvasContext.closePath();
    CanvasContext.fill();
}
//Triangle
function DrawOutlineTriangle(x, y, size, outlineColor) {
    const halfSize = size / 2;
    const height = (Math.sqrt(3) / 2) * size;

    CanvasContext.strokeStyle = outlineColor;
    CanvasContext.lineWidth = 1; // Set a reasonable line width
    CanvasContext.beginPath();
    CanvasContext.moveTo(x, y - height / 2); // Top point
    CanvasContext.lineTo(x - halfSize, y + height / 2); // Bottom left point
    CanvasContext.lineTo(x + halfSize, y + height / 2); // Bottom right point
    CanvasContext.closePath();
    CanvasContext.stroke();
}


function DrawRectangle(x, y, xx, yy, fillColor, outlineColor) {
    CanvasContext.fillStyle = fillColor;
    CanvasContext.fillRect(x, y, xx, yy);
    CanvasContext.strokeStyle = outlineColor;
    CanvasContext.rect(x, y, xx, yy);
    CanvasContext.stroke();
}



function DrawFillEllipse(x, y, radiusX, radiusY, fillColor) {
    CanvasContext.fillStyle = fillColor;
    CanvasContext.beginPath();
    CanvasContext.ellipse(x, y, radiusX, radiusY, 0, 0, Math.PI * 2); // Draw ellipse
    CanvasContext.fill();
}

function DrawOutlineEllipse(x, y, radiusX, radiusY, outlineColor) {
    CanvasContext.strokeStyle = outlineColor;
    CanvasContext.beginPath();
    CanvasContext.ellipse(x, y, radiusX, radiusY, 0, 0, Math.PI * 2); // Draw ellipse
    CanvasContext.stroke();
}


function DrawFillPolygon(x, y, radius, sides, fillColor) {
    CanvasContext.fillStyle = fillColor;
    CanvasContext.beginPath();
    for (let i = 0; i < sides; i++) {
        const angle = (i * 2 * Math.PI) / sides; 
        const vertexX = x + radius * Math.cos(angle);
        const vertexY = y + radius * Math.sin(angle);
        CanvasContext.lineTo(vertexX, vertexY); 
    }
    CanvasContext.closePath();
    CanvasContext.fill(); 
}

function DrawOutlinePolygon(x, y, radius, sides, outlineColor) {
    CanvasContext.strokeStyle = outlineColor;
    CanvasContext.beginPath(); 
    for (let i = 0; i < sides; i++) {
        const angle = (i * 2 * Math.PI) / sides;
        const vertexX = x + radius * Math.cos(angle);
        const vertexY = y + radius * Math.sin(angle);
        CanvasContext.lineTo(vertexX, vertexY); 
    }
    CanvasContext.closePath(); 
    CanvasContext.stroke(); 
}

function DrawText(text, startx, starty, fontSize) {
    CanvasContext.save();
    var middle = canvasLength / 2;
    CanvasContext.textAlign = "center";

    CanvasContext.font = fontSize + "px  Arial"

    CanvasContext.strokeStyle = "black";
    CanvasContext.lineWidth = fontSize/10;
    //CanvasContext.strokeText(text, startx, starty)
    CanvasContext.strokeText(text, middle, starty)
    CanvasContext.fillStyle = "red"
    //CanvasContext.fillText(text, startx, starty);
    CanvasContext.fillText(text, middle, starty);
    CanvasContext.restore();
}
