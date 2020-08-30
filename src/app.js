var gameState = {
    journal: ["hello world"],
    table: [{ card: 100001, position: (0, 0) }],
    players: {}
}
var focus = new Set()
var selectedCard = null;

var buttonColors = [
    0xFF110C,
    0xFCA40A,
    0xFCFD01,
    0x3DFD0B,
    0x0CACFA,
    0x7442FD
];

var totalButtons = buttonColors.length;
var arrayButtons = [];
var menuContaner = null;
var tapButton = null;
var buttonSelected = null;

let crypt = []
let uncontrolled = [{id: "https://res.cloudinary.com/djhqderty/image/upload/v1598627817/yawp/matthias.jpg", blood: 7}, {id: "https://res.cloudinary.com/djhqderty/image/upload/v1598628468/yawp/arika.jpg", blood: 11}, {id: "https://res.cloudinary.com/djhqderty/image/upload/v1598628702/yawp/nergal.jpg", blood: 10}]

let scale = (window.innerWidth - 380) / 1024

var pixi_app = new PIXI.Application(
    { width: 1024 * scale, height: 576 * scale, antialias: true }
)
var zoom_app = new PIXI.Application({ width: 358, height: 500, antialias: true })

var buttonCenterDistance = 40 * scale;
var buttonRadius = 18 * scale;
canvas = new Vue({
    el: "#canvas",
    mounted: function () {
        this.$el.appendChild(pixi_app.view)
    }
})

card_zoom = new Vue({
    el: "#card-zoom",
    mounted: function () {
        this.$el.appendChild(zoom_app.view)
    }
})

messages = new Vue({
    el: "#messages",
    data: {
        message: "",
        history: ["new game started"],
        send: function () {
            this.history.push(this.message)
            this.message = ""
        }
    },
    updated: function () {
        var container = this.$el.querySelector("#history");
        container.scrollTop = container.scrollHeight;
    }
})

controls = new Vue({
    el: "#controls",
    methods: {
        addCryptCard: function (event) {
            addCard(uncontrolled[0].id, true, "crypt", uncontrolled[0].blood);
            uncontrolled.shift()
        }
    }
})

const loader = PIXI.Loader.shared
loader
    .add("https://images.krcg.org/yawpcourt.jpg")
    .add("https://images.krcg.org/francoisvillon.jpg")
    .add("https://res.cloudinary.com/djhqderty/image/upload/v1598627817/yawp/matthias.jpg")
    .add("https://res.cloudinary.com/djhqderty/image/upload/v1598628468/yawp/arika.jpg")
    .add("https://res.cloudinary.com/djhqderty/image/upload/v1598628702/yawp/nergal.jpg")
    .add("https://res.cloudinary.com/djhqderty/image/upload/v1598628796/yawp/shamblinghordes.jpg")
    .add("background", "https://images.krcg.org/background.jpg")
    .add("facedown", "https://res.cloudinary.com/djhqderty/image/upload/v1598784559/yawp/vtes-crypt-facedown.jpg")
    .add("menu", "https://images.krcg.org/menu.png")
    .load(setup)

function setup() {
    const background = new PIXI.Sprite(loader.resources["background"].texture)
    background.scale.set(scale, scale)
    background.interactive = true
    background.sortableChildren = true;
    background.on('pointerdown', onBackgroundDragStart)
    background.on('pointerup', onBackgroundDragEnd)
    background.on('pointerupoutside', onBackgroundDragEnd)
    background.on('pointermove', onBackgroundDragMove)
    background.clearMenu = function () {
        for (card of this.children) {
            if (focus.size && !focus.has(card)) {
                card.alpha = 0.5
            }
            else {
                card.alpha = 1
                if (focus.size) {
                    continue
                }
            }

            hideButtons()
        }
    }
    const graphics = new PIXI.Graphics()
    pixi_app.stage.addChild(background)
    pixi_app.stage.addChild(graphics)
    addCard("https://images.krcg.org/yawpcourt.jpg", false, "master")
    addCard("https://images.krcg.org/francoisvillon.jpg", false,  "crypt", 10)
    addCard("https://res.cloudinary.com/djhqderty/image/upload/v1598628796/yawp/shamblinghordes.jpg", false,  "ally", 4)


    var stage = pixi_app.stage;
    menuContaner = new PIXI.Container();
    stage.addChild(menuContaner);
    menuContaner.interactive = true;
    buttonSelected = false;
    tapButton = new PIXI.Sprite(loader.resources["menu"].texture)    
    tapButton.anchor.set(0.5);
    tapButton.width = buttonRadius * 2;
    tapButton.height = buttonRadius * 2;
    menuContaner.visible = false;   
    addListeners(tapButton, scale);
    addTapButtonClick(tapButton);


    createMenu();
}

function addBlood(cardName, cardType){
    let container = new PIXI.Container();
    const graphics = new PIXI.Graphics();
    
    graphics.lineStyle(0)
    graphics.beginFill(0xDE3249, 1)
    graphics.drawCircle(100, 250, 50)
    graphics.endFill();
    
    let result = crypt.filter(item => {
        return item.id === cardName
      })
    const blood = new PIXI.Text(result[0].blood,{fontFamily : 'Arial', fontSize: 52, fill : 0xffffff, align : 'left'});
    if(cardType === "crypt") {
        graphics.x = 0
        graphics.y = -175
        blood.y = 43
        blood.x = result[0].blood>9 ? 68 : 83
    }
    if(cardType === "ally") {
        graphics.x = 25
        graphics.y = -265
        blood.y = -43
        blood.x = result[0].blood>9 ? 90 : 105
    }
    
    
    blood.interactive = true
    blood.buttonMode = true
    

    graphics.addChild(blood)
    
    container.addChild(graphics)
    container.addChild(blood)
    
    return container
}

function addCard(name, faceDown, cardType, blood) {
    if(cardType === "crypt" || cardType ==="ally"){
        crypt.push({id: name, blood: blood})
    }
    // const faceDownTexture = loader.resource["https://res.cloudinary.com/djhqderty/image/upload/c_scale,w_360/v1598783309/yawp/vtes_back.jpg"].texture // PIXI.Texture.WHITE
    const faceDownTexture = loader.resources["facedown"].texture
    const faceUpTexture = loader.resources[name].texture

    const texture = faceDown ? faceDownTexture : faceUpTexture

    const card = new PIXI.Sprite(texture)
    card.card = name
    card.cardType = cardType
    card.faceDownTexture = faceDownTexture
    card.faceUpTexture = faceUpTexture
    card.anchor.x = 0.5
    card.anchor.y = 0.5
    card.x = Math.random() * (700 - 50) + 50
    card.y = (cardType === "crypt" || cardType === "ally") ? 384 : 260
    card.interactive = true
    card.buttonMode = true
    card.width = 71.6
    card.height = 100
    card.isFaceDown = faceDown
    card
        .on('pointerdown', onCardDragStart)
        .on('pointerup', onCardDragEnd)
        .on('pointerupoutside', onCardDragEnd)
        .on('pointermove', onCardDragMove)
        .on('pointertap', onCardTap)
        .on('pointerover', onCardOver)
        .on('pointerout', onCardEndOver)
        
    if(cardType === "crypt" || cardType  === "ally")
        card.addChild(addBlood(name, cardType))
    pixi_app.stage.getChildAt(0).addChild(card)
}

function onBackgroundDragStart(event) {
    focus.clear()
    this.clearMenu()
    this.data = event.data
    this.selection_start = this.data.getLocalPosition(this.parent)
    this.dragged = false
    this.dragging = true
    this.start = Date.now()
}

function onBackgroundDragEnd(event) {
    this.dragging = false
    this.data = null
    this.selection_start = null
    pixi_app.stage.getChildAt(1).clear()
}

function onBackgroundDragMove(event) {
    if (!this.dragging) { return }
    if (!this.dragged && Date.now() - this.start > 200) {
        this.dragged = true
    }
    // draw a red selection rectangle
    const newPosition = this.data.getLocalPosition(this.parent)
    // sort uses lexicographical order by default, we need numeric order
    x = [this.selection_start.x, newPosition.x].sort((a, b) => a - b)
    y = [this.selection_start.y, newPosition.y].sort((a, b) => a - b)
    var graphics = pixi_app.stage.getChildAt(1)
    graphics.clear()
    graphics.lineStyle(2, 0xFF3300, 1)
    graphics.drawRect(x[0], y[0], x[1] - x[0], y[1] - y[0])
    // put selected cards in focus, remove cards outside of the selection
    for (var card of pixi_app.stage.getChildAt(0).children) {
        const bounds = card.getBounds()
        if (
            bounds.x < x[1] &&
            bounds.x + bounds.width >= x[0] &&
            bounds.y < y[1] &&
            bounds.y + bounds.height >= y[0]
        ) {
            focus.add(card)
            card.alpha = 1
        }
        else {
            focus.delete(card)
            card.alpha = 0.5
        }
    }
}

function onCardTap(event) {
    
    // pointertap is sent at then end of an actual drag&drop - filter it
    if (this.dragged) {
        this.dragged = false
        return
    }
    selectedCard = this;
    focus.clear()
    focus.add(this)

    if (buttonSelected) {
        return;
    }
        
    position = event.data.getLocalPosition(pixi_app.stage.getChildAt(0))
    showButtons({x: ( position.x ) * scale, y: ( position.y ) * scale})
}

function onCardDragStart(event) {
    // store a reference to the data because of multitouch:
    // we want to track the movement of this particular touch
    focus.add(this)
    this.parent.clearMenu()
    this.data = event.data
    this.selection_start = this.data.getLocalPosition(this.parent)
    this.dragged = false
    this.dragging = true
    this.start = Date.now()
    event.stopPropagation()
}

function onCardDragEnd(event) {
    this.dragging = false
    this.data = null
    if (this.dragged) {
        focus.clear()
        this.parent.clearMenu()
    }
}

function onCardDragMove() {
    if (this.dragging) {
        const newPosition = this.data.getLocalPosition(this.parent)
        delta = [
            newPosition.x - this.selection_start.x,
            newPosition.y - this.selection_start.y
        ]
        for (card of focus) {
            card.x += delta[0]
            card.y += delta[1]
        }
        this.selection_start.x = newPosition.x
        this.selection_start.y = newPosition.y
        // pointerup-pointerdown is also called in case of a simple tap or click
        // if we are really dragging, note it and put the card on the foreground
        if (!this.dragged && Date.now() - this.start > 200) {
            this.dragged = true
            this.zIndex = Math.max(...this.parent.children.map(c => c.zIndex)) + 0.1
        }
    }
}

function onMenuTap() {
    if(!selectedCard){
        return;
    }

    // stopping tap propagation also stops the pointerup propagation
    // we need to stop the dragging started on pointerdown manually
    selectedCard.dragging = false
    if (selectedCard.angle === 0) {
        selectedCard.angle = 90
        messages.history.push(`locked ${selectedCard.card}`)
    } else {
        selectedCard.angle = 0
        messages.history.push(`unlocked ${selectedCard.card}`)
    }
    focus.clear()
    selectedCard.parent.clearMenu()
}


function flipCard(card) {
    if (card.isFaceDown) {
        messages.history.push(`flip up ${card.card}`)
        card.texture = card.faceUpTexture
    } else {
        messages.history.push(`flip down ${card.card}`)
        card.texture = card.faceDownTexture;
    }
    card.isFaceDown = !card.isFaceDown;
}
function updateBlood(id, amount){
    crypt.forEach((element, index) => {
        if(element.id === id) {
            crypt[index].blood = amount;
        }
    });
    for (var card of pixi_app.stage.getChildAt(0).children) {        
        if(card.card === id){
            card.getChildAt(0).getChildAt(1).text = amount
            console.log(`card.cardType: ${JSON.stringify(card.cardType)}`)
            if(card.cardType === "crypt") {
                card.getChildAt(0).getChildAt(0).x = 0
                card.getChildAt(0).getChildAt(0).y = -175
                card.getChildAt(0).getChildAt(1).y = 43
                card.getChildAt(0).getChildAt(1).x = amount > 9 ? 68 : 83
            }
            if(card.cardType === "ally") {
                card.getChildAt(0).getChildAt(0).x = 25
                card.getChildAt(0).getChildAt(0).y = -265
                card.getChildAt(0).getChildAt(1).y = -43
                card.getChildAt(0).getChildAt(1).x = amount>9 ? 90 : 105
            }
            // card.getChildAt(0).getChildAt(1).x = amount>9 ? 68 : 83
        }
    }
}
function addBloodCard(card){
    messages.history.push(`add blood ${card.card}`)
    let result = crypt.filter(item => {
        return item.id === card.card
      })
    updateBlood(card.card, result[0].blood+1)
}
function removeBloodCard(card){
    messages.history.push(`remove blood ${card.card}`)
    let result = crypt.filter(item => {
        return item.id === card.card
      })
    if(result[0].blood>0){
        updateBlood(card.card, result[0].blood-1)
    }
    
}

function onCardOver() {
    zoom_app.stage.addChild(new PIXI.Sprite(loader.resources[this.card].texture))
}

function onCardEndOver() {
    zoom_app.stage.removeChildren()
}

function addTapButtonClick(target) {
    target.click = target.tap = function (clickData) {
        onMenuTap();
        hideButtons();
    }
}

function flipCardClick(target) {
    target.click = target.tap = function (clickData) {
        flipCard(selectedCard);
        hideButtons();
    }
}
function addBloodClick(target) {
    target.click = target.tap = function (clickData) {
        addBloodCard(selectedCard);
    }
}
function removeBloodClick(target) {
    target.click = target.tap = function (clickData) {
        removeBloodCard(selectedCard);        
    }
}

function addListeners(target, scale) {
    target.interactive = true;
    target.buttonMode = true;
    target.mouseover = target.touchstart = function (clickData) {
        target.scale.x = 1.3 * scale;
        target.scale.y = 1.3 * scale;
        buttonSelected = true;
    }
    target.mouseout = target.touchend = function (clickData) {
        target.scale.x = 1 * scale;
        target.scale.y = 1 * scale;
        buttonSelected = false;
    }
}
function createMenu() {
    // Flip button (Red button)
    const flipButton = createButton(buttonRadius, buttonColors[0]);
    addListeners(flipButton, 1);
    flipCardClick(flipButton);
    arrayButtons.push(flipButton);
    menuContaner.addChild(flipButton);

    // button to be defined (Orange button)
    const orangeButton = createButton(buttonRadius, buttonColors[1]);
    addListeners(orangeButton, 1);
    //flipCardClick(button);
    arrayButtons.push(orangeButton);
    menuContaner.addChild(orangeButton);

    // button to be defined (Yellow button)
    const yellowButton = createButton(buttonRadius, buttonColors[2]);
    addListeners(yellowButton, 1);
    //flipCardClick(button);
    arrayButtons.push(yellowButton);
    menuContaner.addChild(yellowButton);

     // button to be defined (Green button)
     const greenButton = createButton(buttonRadius, buttonColors[3]);
     addListeners(greenButton, 1);
     //flipCardClick(button);
     arrayButtons.push(greenButton);
     menuContaner.addChild(greenButton);

     // button to be defined (Cyan button)
     const removeBloodButton = createButton(buttonRadius, buttonColors[4]);
     addListeners(removeBloodButton, 1);
     removeBloodClick(removeBloodButton);
     arrayButtons.push(removeBloodButton);
     menuContaner.addChild(removeBloodButton);

     // button to be defined (Violet button)
     const addBloodButton = createButton(buttonRadius, buttonColors[5]);
     addListeners(addBloodButton, 1);
     addBloodClick(addBloodButton);
     arrayButtons.push(addBloodButton);
     menuContaner.addChild(addBloodButton);

    // Tap button
    menuContaner.addChild(tapButton)           
}



function createButton(radius, color, line) {
    var button = new PIXI.Graphics();
    if (line) {
        button.lineStyle(4, 0x222222);
        button.beginFill(0, 0.5);

    } else {
        button.beginFill(0xFFFFFF);
        button.tint = color ? color : 0xAAAAAA;
    }

    button.drawCircle(0, 0, radius ? radius : buttonRadius);
    return button
}

function distance(x1, y1, x2, y2) {
    if (!x2) x2 = 0;
    if (!y2) y2 = 0;
    return Math.sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
}

function showButtons(point) {

    var parentWidth = pixi_app.stage.width;
    var parentHeight = pixi_app.stage.height;

    menuContaner.visible = true;
    var barSize = 0;
    var collideLeft = distance(0, 0, point.x, 0) < (buttonRadius * 2 + buttonCenterDistance);
    var collideRight = distance(parentWidth - barSize, 0, point.x, 0) < (buttonRadius * 2 + buttonCenterDistance);
    var collideUp = distance(0, 0, 0, point.y) < (buttonRadius * 2 + buttonCenterDistance);
    var collideDown = distance(0, parentHeight - barSize, 0, point.y) < (buttonRadius * 2 + buttonCenterDistance);

    var noCollide = collideLeft || collideRight || collideUp || collideDown;
    tapButton.position = point;
    tapButton.visible = true;
    for (i = 0; i < arrayButtons.length; i++) {
        var button = arrayButtons[i];
        var refAngle = 360;
        var refAnglePlus = 180;
        var plusDistance = 0;
        var totButtons = (arrayButtons.length - 1);
        var angleFactorCorner = (totButtons * 1.5 + 2);
        if (collideLeft) {
            refAngle = 180 + 360 / totButtons / 2;
        } else if (collideRight) {
            refAngle = -180 - 360 / totButtons / 2;
        }
        if (collideUp) {
            refAnglePlus = -90
            plusDistance = buttonRadius * 3;
            if (collideLeft) {
                refAngle = 90 + 360 / angleFactorCorner / 2;
            } else if (collideRight) {
                refAngle = 90 + 360 / angleFactorCorner / 2;
                refAnglePlus = 0
            }
            else {
                refAngle = 180 + (360 / totButtons / 2);
                plusDistance = buttonRadius / 2;
            }

        } else if (collideDown) {

            refAnglePlus = -90
            plusDistance = buttonRadius * 3;
            if (collideLeft) {
                refAngle = -90 - 360 / angleFactorCorner / 2;
            } else if (collideRight) {
                refAngle = -90 - 360 / angleFactorCorner / 2;
                refAnglePlus = 180
            }
            else {
                refAngle = -180 - 360 / totButtons / 2;
                plusDistance = buttonRadius / 2;
            }
        }

        var tempRad = -((refAngle / arrayButtons.length) * i + refAnglePlus) / 180 * Math.PI;
        button.scale.set(1);
        button.position.x = Math.sin(tempRad) * (buttonCenterDistance + plusDistance) + point.x;
        button.position.y = Math.cos(tempRad) * (buttonCenterDistance + plusDistance) + point.y;
        button.alpha = 1;
    }
}

function hideButtons() {
    menuContaner.visible = false;
    tapButton.visible = false;
    for (i = 0; i < arrayButtons.length; i++) {
        var button = arrayButtons[i];
        button.alpha = 0;
        button.scale.x = 0;
        button.scale.y = 0;
    }
    buttonSelected = false;
}
