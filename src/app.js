var gameState = {
    journal: ["hello world"],
    table: [{card: 100001, position: (0,0)}],
    players: {}
}
var focus = new Set()

var scale = (window.innerWidth - 380) / 1024
var pixi_app = new PIXI.Application(
    {autoresize: true, width: (940 * scale) - (130 * scale), height: (576 * scale) - (52 * scale) , antialias: true}
)
var zoom_app = new PIXI.Application({width: 232 * scale, height: 330 * scale, antialias: true})

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
        send: function(){
            this.history.push(this.message)
            this.message = ""
        }
    },
    updated: function () {
        var container = this.$el.querySelector("#history");
        container.scrollTop = container.scrollHeight;
    }
})

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
    background.on('pointerdown', onBackgroundDragStart)
    background.on('pointerup', onBackgroundDragEnd)
    background.on('pointerupoutside', onBackgroundDragEnd)
    background.on('pointermove', onBackgroundDragMove)
    background.clearMenu = function () {
        for(card of this.children) {
            if(focus.size && !focus.has(card)){
                card.alpha = 0.5
            }
            else {
                card.alpha = 1
                if(focus.size){
                    continue
                }
            }
            for(menu of card.children) {
                menu.visible = false
            }
        }
    }
    const graphics = new PIXI.Graphics()
    pixi_app.stage.addChild(background)
    pixi_app.stage.addChild(graphics)	
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
        .on('pointerdown', onCardDragStart)
        .on('pointerup', onCardDragEnd)
        .on('pointerupoutside', onCardDragEnd)
        .on('pointermove', onCardDragMove)
        .on('pointertap', onCardTap)
        .on('pointerover', onCardOver)
        .on('pointerout', onCardEndOver)
    const menu = new PIXI.Sprite(loader.resources["menu"].texture)
    menu.anchor.x = 0.5
    menu.anchor.y = 0.5
    menu.scale.set(5, 5)
    menu.interactive = true
    menu.buttonMode = true
    menu.visible = false
    menu.on('pointertap', onMenuTap)
    card.addChild(menu)
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
    if (!this.dragging) {return}
    if (!this.dragged && Date.now() - this.start > 200){
        this.dragged = true
    }
    // draw a red selection rectangle
    const newPosition = this.data.getLocalPosition(this.parent)
    // sort uses lexicographical order by default, we need numeric order
    x = [this.selection_start.x, newPosition.x].sort((a,b) => a - b)
    y = [this.selection_start.y, newPosition.y].sort((a,b) => a - b)
    var graphics = pixi_app.stage.getChildAt(1)
    graphics.clear()
    graphics.lineStyle(2, 0xFF3300, 1)
    graphics.drawRect(x[0], y[0], x[1] - x[0], y[1] - y[0])
    // put selected cards in focus, remove cards outside of the selection
    for (var card of pixi_app.stage.getChildAt(0).children){
        const bounds = card.getBounds()
        if (
            bounds.x < x[1] && 
            bounds.x + bounds.width >= x[0] && 
            bounds.y < y[1] && 
            bounds.y + bounds.height >= y[0]
        )
        {
            focus.add(card)
            card.alpha = 1
        }
        else{
            focus.delete(card)
            card.alpha = 0.5
        }
    }
}

function onCardTap(event){
    // pointertap is sent at then end of an actual drag&drop - filter it
    if(this.dragged){
        this.dragged = false
        return
    }
    focus.clear()
    focus.add(this)
    this.parent.clearMenu()
    menu = this.getChildAt(0)
    position = event.data.getLocalPosition(this)
    menu.x = position.x
    menu.y = position.y
    menu.visible = true
    event.stopPropagation()
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
    if(this.dragged){
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
        for(card of focus){
            card.x += delta[0]
            card.y += delta[1]
        }
        this.selection_start.x = newPosition.x 
        this.selection_start.y = newPosition.y
        // pointerup-pointerdown is also called in case of a simple tap or click
        // if we are really dragging, note it and put the card on the foreground
        if(!this.dragged && Date.now() - this.start > 200){
            this.dragged = true
            this.zIndex = Math.max(...this.parent.children.map(c => c.zIndex)) + 0.1
        }
    }
}

function onMenuTap(event) {
    // stopping tap propagation also stops the pointerup propagation
    // we need to stop the dragging started on pointerdown manually
    this.parent.dragging = false
    if(this.parent.angle === 0){
        this.parent.angle = 90
        // make sure the menu does not rotate with the card
        this.angle = -90
        messages.history.push(`locked ${this.parent.card}`)
    } else {
        this.parent.angle = 0
        this.angle = 0
        messages.history.push(`unlocked ${this.parent.card}`)
    }
    focus.clear()
    this.parent.parent.clearMenu()
    event.stopPropagation()
}

function onCardOver() {
    var sprite = new PIXI.Sprite(loader.resources[this.card].texture)
    sprite.width = 232 * scale
    sprite.height = 330 * scale
    zoom_app.stage.addChild(sprite)

}

function onCardEndOver() {
    zoom_app.stage.removeChildren()
}

function resizeApps(){
    scale = (document.documentElement.clientWidth)  / 1024 
    pixi_app.renderer.resize(940 * scale - (130 * scale), 576 * scale - (52 * scale)) 
    pixi_app.stage.width = (940 * scale) - (130 * scale)
    pixi_app.stage.height = 576 * scale - (52 * scale)

    zoom_app.renderer.resize(232 * scale, 330 * scale)
	zoom_app.stage.width = 232 * scale
	zoom_app.stage.height = 330 * scale
    //pixi_app.renderer.resize(100, 100)
    //pixi_app.queueResize()
	//pixi_app.renderer.resize(parent.clientWidth, parent.clientHeight);
	//pixi_app.resize();	
	/*scale = (window.innerWidth - 380) / 1024
	pixi_app.stage.width = 1024 * scale
	pixi_app.stage.height = 576 * scale
	zoom_app.screen.width = 358 * scale
	zoom_app.screen.height = 330 * scale
	
	zoom_app.queueResize()*/
}

window.addEventListener("resize", resizeApps);