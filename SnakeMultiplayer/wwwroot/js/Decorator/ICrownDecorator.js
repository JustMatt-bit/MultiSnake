class ICrownDecorator {
    constructor(nextDecorator = null) {
        this.nextDecorator = nextDecorator;
    }

    drawCrown(x, y) {
        // Enforce override by subclasses
        if (this.constructor === ICrownDecorator) {
            throw new Error("ICrownDecorator is an abstract class and cannot be instantiated directly");
        }

        if (this.nextDecorator) {
            this.nextDecorator.drawCrown(x, y);
        }
    }
}