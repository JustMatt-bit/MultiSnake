
class ColoredCrownDecorator extends ICrownDecorator {
    constructor(canvasCtx, baseCellParams, startBorder, nextDecorator = null) {
        super(nextDecorator);
        this.baseCellParams = baseCellParams;
        this.canvasCtx = canvasCtx;
        this.startBorder = startBorder;
    }

    drawCrown(x, y) {
        console.log("ColoredCrownDecorator")

        super.drawCrown(x, y)
        //this.outlineDecorator.drawCrown(x, y);

        const crownX = this.getCellCoord(x) + this.baseCellParams.size / 2;
        const crownY = this.getCellCoord(y) + this.baseCellParams.size / 2 - this.baseCellParams.size / 4;

        this.canvasCtx.fillStyle = "gold";
        this.canvasCtx.beginPath();
        this.canvasCtx.arc(crownX, crownY, this.baseCellParams.size / 4 - 2, 0, 2 * Math.PI);
        this.canvasCtx.fill();
    }

    getCellCoord(cell) {
        return this.startBorder + (cell * this.baseCellParams.size);
    }

}