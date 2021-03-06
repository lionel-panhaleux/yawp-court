﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Golconda.Models;
using Golconda.Services;
using Golconda.Services.Contracts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        private SlidingCard HoveringCard { get; set; }
        private Local<Hand> Hand { get; set; }

        public const float BOARD_MIN_SCALE = 0.1f;
        public const float BOARD_MAX_SCALE = 4;
        private const float ZOOM_STEP = 0.05f;
        private const int NOTCHES_PER_WHEEL_SCROLL = 120;
        private bool _mustChangeResolution = true;
        private Point _resolution = new Point(800, 600);
        private bool _fullScreen = true;

        private IInputService InputService { get; } = new InputService();
        private IProjector Projector { get; } = new Projector();

        private readonly IFrameCounterService _frameCounter = new FrameCounterService();

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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

            Board = new Local<Board>(new Board(InputService), 0.25f);

            // we create two dummy cards for the demo
            var card = new Card(CryptCards.Values.First());
            var refCard = new Local<Item>(card);
            refCard._translate = new Vector2(300, 10);
            Board.Value.Items.Add(refCard);
            card = new Card(CryptCards.Values.First());
            refCard = new Local<Item>(card);
            refCard._translate = new Vector2(700, 50);
            Board.Value.Items.Add(refCard);

            Hand = new Local<Hand>(new Hand(InputService, HandResize, 600));
            for (int i = 0; i < 2; ++i)
            {
                Hand.Value.AddCard(new Card(CryptCards.Values.First()));
            }
        }

        private void HandResize(Vector2 newSize)
        {
            var screenSize = GetScreenSize();
            Hand._translate = new Vector2(screenSize.X / 2, screenSize.Y + newSize.Y * 0.75f);
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

            CommonTextures.BoardBackground = Content.Load<Texture2D>("background");
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
                Graphics.IsFullScreen = Debugger.IsAttached ? false : _fullScreen;

                var size = GetScreenSize();
                Graphics.PreferredBackBufferWidth = size.X;
                Graphics.PreferredBackBufferHeight = size.Y;
                Graphics.ApplyChanges();

                _mustChangeResolution = false;
            }

            InputService.Update(gameTime);

            if (InputService.PressedKeys.Any(k => k == Keys.Escape))
            {
                Exit();
            }

            if (InputService.PressedKeys.Any(k => k == Keys.A))
            {
                Hand.Value.AddCard(new Card(CryptCards.Values.First()));
            }
            if (InputService.PressedKeys.Any(k => k == Keys.Z))
            {
                if(Hand.Value.Cards.Count > 0)
                Hand.Value.RemoveCard(Hand.Value.Cards.Last().Value);
            }

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

                    Board._translate += new Vector2(offsetX, offsetY);
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
                        Board._translate += dragDelta;
                        captureEvents = false;
                    }
                }
            }

            if (captureEvents)
            {
                if (Board.Value.HoveredCard?.Value != HoveringCard?.Card)
                {
                    if (Board.Value.HoveredCard != null)
                    {
                        SlidingCard result = CreateHoveringCard(gameTime);

                        HoveringCard = result;
                    }
                    else
                    {
                        HoveringCard = null;
                    }

                }
            }
            else
            {
                HoveringCard = null;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Creates an unscaled card picture next to the hovered card, preferably on the right, but switched on the other side if it would be out of the screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <returns>The hovering card.</returns>
        private SlidingCard CreateHoveringCard(GameTime gameTime)
        {
            var screenSize = GetScreenSize();

            var card = (Card)Board.Value.HoveredCard.Value;

            Vector2 position;

            // we switch to the board coordinates system to extract the screen values
            using (new ProjectorScope(Projector, Board.GetProjection()))
            {
                // get the position to the right of the card (adding the *scaled* width), with a margin of 10px
                Vector2 margin = new Vector2(10, 0);
                position = Projector.ProjectToScreen(Board.Value.HoveredCard._translate + new Vector2(card._size.X, 0)) + margin;
                if (position.X + card._size.X > screenSize.X)
                {
                    // get the position to the left of the card (minus the *unscaled* width), with a margin of 10px
                    position = Projector.ProjectToScreen(Board.Value.HoveredCard._translate) - new Vector2(card._size.X, 0) - margin;
                }
                if (position.Y + card._size.Y > screenSize.Y)
                {
                    position.Y -= card._size.Y;
                }
            }

            return new SlidingCard(gameTime, card, position);
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

            //SpriteBatch.Draw(CryptCards.Values.First(), new Vector2(200, 200), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            ////SpriteBatch.Draw(CryptCards.Values.First(), new Vector2(200, 200), null, Color.White, 0f, new Vector2(100, 0), 0.5f, SpriteEffects.None, 0);
            //var offset = new Vector2(334/2, 1000);
            //SpriteBatch.Draw(CryptCards.Values.First(), new Vector2(200, 200) + offset, null, Color.White, 0f, offset, 1f, SpriteEffects.None, 0);
            //SpriteBatch.Draw(CryptCards.Values.First(), new Vector2(200, 200) + offset, null, Color.White, 0.1f, offset, 1f, SpriteEffects.None, 0);
            //SpriteBatch.Draw(CryptCards.Values.First(), new Vector2(200, 200) + offset, null, Color.White, MathHelper.PiOver4, offset, 1f, SpriteEffects.None, 0);
            //SpriteBatch.Draw(CryptCards.Values.First(), new Vector2(200, 200) + offset, null, Color.White, MathHelper.PiOver2, offset, 1f, SpriteEffects.None, 0);

            //SpriteBatch.Draw(CommonTextures.WhiteRectangle, new Rectangle((int)(200 + offset.X)-1, (int)(200 + offset.Y)-1, 3, 3) , Color.White);
            //SpriteBatch.Draw(CommonTextures.WhiteRectangle, new Rectangle((int)(200 + offset.X), (int)(200 + offset.Y), 1, 1) , Color.Black);
            Board.Draw(gameTime, SpriteBatch, Projector);
            Hand.Draw(gameTime, SpriteBatch, Projector);

            if (HoveringCard != null)
            {
                // create a sliding translation at the same scale
                using (new ProjectorScope(Projector, new Projection(HoveringCard.SlideEffect.GetPosition(gameTime), 1, Rotation2.Zero)))
                {
                    HoveringCard.Card.DrawNaked(gameTime, SpriteBatch, Projector);
                }
            }

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
