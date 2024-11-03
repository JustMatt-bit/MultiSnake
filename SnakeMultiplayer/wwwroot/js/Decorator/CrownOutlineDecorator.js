class CrownOutlineDecorator extends ICrownDecorator {
    constructor(canvasCtx, baseCellParamsSize, startBorder, nextDecorator = null) {
        super(nextDecorator);
        this.canvasCtx = canvasCtx;
        this.baseCellParamsSize = baseCellParamsSize;
        this.startBorder = startBorder;
    }

    drawCrown(x, y) {
        console.log("CrownOutlineDecorator")
        const crownX = this.getCellCoord(x) + this.baseCellParamsSize / 2;
        const crownY = this.getCellCoord(y) + this.baseCellParamsSize / 2 - this.baseCellParamsSize / 4;

        this.canvasCtx.strokeStyle = "gold";
        this.canvasCtx.lineWidth = 2;
        this.canvasCtx.beginPath();
        this.canvasCtx.arc(crownX, crownY, this.baseCellParamsSize / 4, 0, 2 * Math.PI);
        this.canvasCtx.stroke();

        super.drawCrown(x,y)
    }

    getCellCoord(cell) {
        return this.startBorder + (cell * this.baseCellParamsSize);
    }
}