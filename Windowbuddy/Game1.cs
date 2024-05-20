

using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Windowbuddy
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Microsoft.Xna.Framework.Point velocity;
        public Microsoft.Xna.Framework.Point position;
        public Microsoft.Xna.Framework.Point center;

        private Texture2D face;

        System.Windows.Forms.Form myForm;

        private Random ran;

        public int width = 17 * 4;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            myForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(this.Window.Handle);

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 17 * 4;
            _graphics.PreferredBackBufferHeight = 28 * 4;




            _graphics.ApplyChanges();
            base.Initialize();

            ran = new Random();

            position = new Microsoft.Xna.Framework.Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - Window.ClientBounds.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 - Window.ClientBounds.Height);
            velocity = new Microsoft.Xna.Framework.Point(ran.Next(-50, 50), ran.Next(-50, 50));


            Window.Position = position;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            face = Content.Load<Texture2D>("Windowbiddy");

            // TODO: use this.Content to load your game content here
        }


        
        private Microsoft.Xna.Framework.Point _lastMousePosition;
        private Microsoft.Xna.Framework.Point _MousePosition;
        private Vector2 _mouseDelta;
        private bool _isDragging;

        private int WalkLength;
        private int WalkTimer;
        private int walkDirection;

        private List<Microsoft.Xna.Framework.Point> oldMousePositions = new List<Microsoft.Xna.Framework.Point>();

        private bool Click;
        private bool UnClick;
        private Point windowmousepos;

        private bool flinging;

        private bool walking = false;
        private int frame;
        private int animTimer;

        private Point Addpostomouse;
        protected override void Update(GameTime gameTime)
        {
            oldMousePositions.Add(_MousePosition);

            if (oldMousePositions.Count >= 6)
            {
                oldMousePositions.RemoveAt(0);
            }

            position = Window.Position;


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                Exit();

            if (ran.Next(0, 1000) < 25 && !walking)
            {
                walking = true;
                WalkLength = ran.Next(120, 320);
                WalkTimer = 0;
                if (ran.Next(0, 100) < 50)
                {
                    walkDirection = -1;
                }
                else
                {
                    walkDirection = 1;
                }
            }



            
            bool atTopEdge = position.Y <= 0;
            bool atBottomEdge = position.Y >= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - Window.ClientBounds.Height;

            bool atLeftEdge = position.X <= 0;
            bool atRightEdge = position.X >= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - width;

            

            _isDragging = (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) && myForm.ClientRectangle.IntersectsWith(new System.Drawing.Rectangle(windowmousepos.X, windowmousepos.Y, 5, 5));

            if (!_isDragging)
            {
                velocity.Y += 2;


                if (atBottomEdge)
                {

                    velocity.Y = (int)(velocity.Y * -0.5f);
                    if (velocity.Y >= -6)
                    {
                        velocity.Y = 0;
                    }
                    position.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - Window.ClientBounds.Height;

                    if (walking)
                    {
                        if (WalkTimer++ < WalkLength)
                        {
                            velocity.X = 3 * walkDirection;
                        }
                        else
                        {
                            walking = false;
                            velocity.X = 0;
                        }
                    }
                    else
                    {
                        if(velocity.X > 2000)
                        {
                            velocity.X = 0;
                        }
                    }

                    if (flinging)
                    {
                        Debug.WriteLine(velocity.X);
                        velocity.X = 0;
                        flinging = false;
                    }

                }
                if (atTopEdge)
                {
                    if (velocity.Y != 0)
                    {
                        velocity.Y = 6 * (velocity.Y / 5);
                        velocity.Y *= -1;
                    } else
                    {
                        velocity.Y = 2;
                    }
                    position.Y = 1;

                }

                if (atLeftEdge)
                {
                    velocity.X = (int)(velocity.X * -0.5f);
                    position.X = 0;
                    walkDirection *= -1;
                }

                if (atRightEdge)
                {
                    velocity.X = (int)(velocity.X * -0.5f);
                    position.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - width;
                    walkDirection *= -1;
                }

                if (UnClick)
                {
                    velocity = oldMousePositions[4] - oldMousePositions[0];
                    velocity = new Point((int)(velocity.X * 0.25f), (int)(velocity.Y * 0.25f));
                    UnClick = false;
                }

                if (flinging)
                {
                    walkDirection = (int)(velocity.X / MathF.Abs(velocity.X));
                }




                Click = true;
            }
            if (_isDragging)
            {
                flinging = true;
                walking = false;
                if (Click)
                {
                    Addpostomouse = Window.Position - new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);

                }
                velocity = Point.Zero;
                position = new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y) + Addpostomouse;
                UnClick = true;
                Click = false;
            }

            if (MathF.Abs(velocity.X) > 2000) { velocity.X = 0; }

            Window.Position = position + velocity;



            Window.IsBorderless = true;

            base.Update(gameTime);

            animation();

            myForm.TopMost = true;
            myForm.TransparencyKey = System.Drawing.Color.Black;
            myForm.AllowTransparency = true;

            _MousePosition = new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
            windowmousepos = Mouse.GetState().Position;
        }

       

        
        

        
        public void animation()
        {

            if (walking)
            {
                if (animTimer++ % 7 == 0)
                    frame++;
                if (frame >= 13)
                {
                    frame = 1;
                }
            }
            else
            {
                frame = 0;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(samplerState: Microsoft.Xna.Framework.Graphics.SamplerState.PointClamp, blendState: Microsoft.Xna.Framework.Graphics.BlendState.AlphaBlend);

            _spriteBatch.Draw(face, new Rectangle(0, 0, 17 * 4, 28 * 4), new Rectangle(0, 28 * frame, 17, 28), Color.White, 0, Vector2.Zero, walkDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
