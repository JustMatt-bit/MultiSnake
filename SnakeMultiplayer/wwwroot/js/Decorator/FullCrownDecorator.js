class FullCrownDecorator extends ICrownDecorator {
    constructor(canvasCtx, baseCellParams, startBorder, nextDecorator = null) {
        super(nextDecorator);
        this.canvasCtx = canvasCtx;
        this.baseCellParams = baseCellParams;
        this.startBorder = startBorder;
    }

    drawCrown(x, y) {
        console.log("FullCrownDecorator")

        //this.coloredDecorator.drawCrown(x, y);
        super.drawCrown(x,y)

        const crownX = this.getCellCoord(x) + this.baseCellParams.size / 2;
        const crownY = this.getCellCoord(y) + this.baseCellParams.size / 2 - this.baseCellParams.size / 4;

        this.canvasCtx.fillStyle = "red";
        this.canvasCtx.beginPath();
        this.canvasCtx.arc(crownX, crownY - this.baseCellParams.size / 8, 3, 0, 2 * Math.PI);
        this.canvasCtx.fill();

        this.canvasCtx.fillStyle = "blue";
        this.canvasCtx.beginPath();
        this.canvasCtx.arc(crownX - this.baseCellParams.size / 6, crownY, 2, 0, 2 * Math.PI);
        this.canvasCtx.fill();

        this.canvasCtx.beginPath();
        this.canvasCtx.arc(crownX + this.baseCellParams.size / 6, crownY, 2, 0, 2 * Math.PI);
        this.canvasCtx.fill();
    }

    getCellCoord(cell) {
        return this.startBorder + (cell * this.baseCellParams.size);
    }
}