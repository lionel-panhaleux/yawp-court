using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Golconda.Models;
using Golconda.Services;
using Golconda.Services.Contracts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Golconda
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager Graphics { get; }
        private SpriteBatch SpriteBatch { get; set; }
        private SpriteFont RegularFont { get; set; }

        private Dictionary<string, Texture2D> CryptCards { get; } = new Dictionary<string, Texture2D>();

        private Local<Board> Board { get; set; }

        public const float BOARD_MIN_SCALE = 0.1f;
        public const float BOARD_MAX_SCALE = 4;
        private const float ZOOM_STEP = 0.05f;
        private const int NOTCHES_PER_WHEEL_SCROLL = 120;
        private bool _mustChangeResolution = true;
        private Point _resolution = new Point(800, 600);
        private bool _fullScreen = true;

        private IInputService InputService { get; } = new InputService();
        private IProjector Projector { get; } = new Projector();
        private IFrameCounterService _frameCounter = new FrameCounterService();

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            RegularFont = Content.Load<SpriteFont>("Regular");

            // load the textures on the fly
            foreach (var file in Directory.GetFiles(@"Content\cards\crypt", "*.jpg"))
            {
                FileStream filestream = new FileStream(file, FileMode.Open);
                Texture2D myTexture = Texture2D.FromStream(GraphicsDevice, filestream);
                CryptCards.Add("alabastrom", myTexture);
            }

            CommonTextures.CardBorder = Content.Load<Texture2D>(@"cards\cardborder");

            // Create a 1px square rectangle texture that will be scaled to the
            // desired size and tinted the desired color at draw time
            CommonTextures.WhiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
            CommonTextures.WhiteRectangle.SetData(new[] { Color.White });
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Board = new Local<Board>(new Board(InputService));

            // we create two dummy cards for the demo
            var card = new Card(CryptCards.Values.First());
            var refCard = new Local<Item>(card);
            refCard._origin = new Vector2(300, 10);
            Board.Value.Items.Add(refCard);
            card = new Card(CryptCards.Values.First());
            refCard = new Local<Item>(card);
            refCard._origin = new Vector2(700, 50);
            Board.Value.Items.Add(refCard);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            SpriteBatch.Dispose();
            CommonTextures.WhiteRectangle.Dispose();
        }

        private Point GetScreenSize()
        {
            return _fullScreen
                ? new Point(Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width, Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height)
                : _resolution;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (_mustChangeResolution)
            {
                Graphics.IsFullScreen = _fullScreen;

                var size = GetScreenSize();
                Graphics.PreferredBackBufferWidth = size.X;
                Graphics.PreferredBackBufferHeight = size.Y;
                Graphics.ApplyChanges();

                _mustChangeResolution = false;
            }

            InputService.Update(gameTime);

            bool captureEvents = true;
            if (InputService.IsZooming)
            {
                var scrollValue = InputService.ZoomDelta;
                var newZoomValue = Math.Min(BOARD_MAX_SCALE, Math.Max(BOARD_MIN_SCALE, Board._scale + ZOOM_STEP * scrollValue / NOTCHES_PER_WHEEL_SCROLL));

                if (newZoomValue != Board._scale)
                {
                    // https://stackoverflow.com/questions/2916081/zoom-in-on-a-point-using-scale-and-translate
                    var scalechange = newZoomValue - Board._scale;
                    var offsetX = -(InputService.MousePosition.X * scalechange);
                    var offsetY = -(InputService.MousePosition.Y * scalechange);

                    Board._origin += new Vector2(offsetX, offsetY);
                    Board._scale = newZoomValue;
                }
                captureEvents = false;
            }

            Board.Update(gameTime, ref captureEvents, Projector);

            if (captureEvents)
            {
                if (InputService.LeftMousePressed)
                {
                    if (InputService.IsDragging)
                    {
                        var dragDelta = Projector.ScaleToLocal(InputService.DragDelta.ToVector2());
                        Board._origin += dragDelta;
                        captureEvents = false;
                    }
                }

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _frameCounter.Update(deltaTime);

            SpriteBatch.Begin();

            Board.Draw(gameTime, SpriteBatch, Projector);

            // draw the debug info
            var lines = new[]
            {
                $"FPS: {_frameCounter.AverageFramesPerSecond}",
                InputService.MousePosition.ToString(),
                $"Left {(InputService.LeftMousePressed ? "Pressed": "Released")}",
                $"Clicked {InputService.Clicked}",
                $"DoubleClicked {InputService.DoubleClicked}",
                $"IsZooming {InputService.IsZooming}",
                $"Zoom delta {InputService.ZoomDelta}",
                $"IsDragging {(InputService.IsDragging? "True": "False")}",
                $"Drag delta {InputService.DragDelta}",
                $"Last left{((InputService)InputService).LastLeftButton.Value}",
                $"Last change{((InputService)InputService).LastLeftButtonChangeState}",
            };

            int y = 0;
            foreach (var line in lines)
            {
                SpriteBatch.DrawString(RegularFont, line, new Vector2(0, y), Color.White);
                y += 16;
            }

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
