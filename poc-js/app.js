var gameState = {
    journal: ["hello world"],
    table: [{card: 100001, position: (0,0)}],
    players: {}
}

const scale = (window.innerWidth - 380) / 1024
var pixi_app = new PIXI.Application({width: 1024 * scale, height: 576 * scale, antialias: true})
var zoom_app = new PIXI.Application({width: 358, height: 500, antialias: true})

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

messages = new Vue({el: "#messages"})

const loader = PIXI.Loader.shared
loader
  .add("https://images.krcg.org/yawpcourt.jpg")
  .add("https://images.krcg.org/francoisvillon.jpg")
  .add("background", "https://images.krcg.org/background.jpg")
  .add("menu", "https://images.krcg.org/menu.png")
  .load(setup)

function setup() {
    const background = new PIXI.Sprite(loader.resources["background"].texture)
    background.scale.set(scale, scale)
    background.interactive = true
    background.sortableChildren = true;
    background.on('pointertap', onTapBackground)
    background.clearMenu = function (source) {
        for(card of this.children) {
            if(card === source) {continue}
            for(menu of card.children) {
                menu.visible = false
            }
        }
    }
    pixi_app.stage.addChild(background)
    addCard("https://images.krcg.org/yawpcourt.jpg")
    addCard("https://images.krcg.org/francoisvillon.jpg")
}

function addCard(name){
    const card = new PIXI.Sprite(loader.resources[name].texture)
    card.card = name
    card.scale.set(0.2, 0.2)
    card.anchor.x = 0.5
    card.anchor.y = 0.5
    card.x = 640
    card.y = 384
    card.interactive = true
    card.buttonMode = true
    card
        .on('pointerdown', onDragStart)
        .on('pointerup', onDragEnd)
        .on('pointerupoutside', onDragEnd)
        .on('pointermove', onDragMove)
        .on('pointertap', onTapCard)
        .on('pointerover', onCardOver)
        .on('pointerout', onCardEndOver)
    const menu = new PIXI.Sprite(loader.resources["menu"].texture)
    menu.anchor.x = 0.5
    menu.anchor.y = 0.5
    menu.scale.set(5, 5)
    menu.interactive = true
    menu.buttonMode = true
    menu.visible = false
    menu.on('pointertap', onTapMenu)
    card.addChild(menu)
    pixi_app.stage.getChildAt(0).addChild(card)
}

function onTapBackground(){
    this.clearMenu()
}

function onTapCard(event){
    // pointertap is sent at then end of an actual drag&drop - filter it
    if(this.dragged){
        return
    }
    menu = this.getChildAt(0)
    position = event.data.getLocalPosition(this)
    menu.x = position.x
    menu.y = position.y
    menu.visible = true
    this.parent.clearMenu(this)
    event.stopPropagation()
}

function onDragStart(event) {
    // store a reference to the data because of multitouch:
    // we want to track the movement of this particular touch
    this.data = event.data
    this.dragged = false
    this.dragging = true
    this.start = Date.now()
}

function onDragEnd(event) {
    this.dragging = false
    this.data = null
}

function onDragMove() {
    if (this.dragging) {
        const newPosition = this.data.getLocalPosition(this.parent)
        this.x = newPosition.x
        this.y = newPosition.y
        // pointerup-pointerdown is also called in case of a simple tap or click
        // if we are really dragging, note it and put the card on the foreground
        if(!this.dragged && Date.now() - this.start > 200){
            this.dragged = true
            this.zIndex = Math.max(...this.parent.children.map(c => c.zIndex)) + 0.1
        }
    }
}

function onTapMenu(event) {
    // stopping tap propagation also stops the pointerup propagation
    // we need to stop the dragging started on pointerdown manually
    this.parent.dragging = false
    if(this.parent.angle === 0){
        this.parent.angle = 90
        // make sure the menu does not rotate with the card
        this.angle = -90
    } else {
        this.parent.angle = 0
        this.angle = 0
    }
    this.visible = false
    event.stopPropagation()
}

function onCardOver() {
    zoom_app.stage.addChild(new PIXI.Sprite(loader.resources[this.card].texture))
}

function onCardEndOver() {
    zoom_app.stage.removeChildren()
}
