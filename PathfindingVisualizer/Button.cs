using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingVisualizer
{
    public class Button
    {
        private Color color;
        private SpriteFont font;

        public Rectangle Hitbox { get; set; }

        public string Text { get; set; }

        public Button(Color color, Rectangle hitbox, string text, SpriteFont font)
        {
            this.color = color;
            this.font = font;
            Hitbox = hitbox;
            Text = text;
        }

        public bool IsClicked()
        {
            MouseState mouse = Mouse.GetState();
            return new Rectangle(mouse.Position, new Point(1, 1)).Intersects(Hitbox) && mouse.LeftButton == ButtonState.Pressed;
        }

        public void Draw(SpriteBatch sp)
        {
            sp.FillRectangle(Hitbox, color);
            Vector2 pos = new Vector2(Hitbox.Location.X + ((Hitbox.Width - font.MeasureString(Text).X) / 2), Hitbox.Location.Y + ((Hitbox.Height - font.MeasureString(Text).Y) / 2));
            sp.DrawString(font, Text, pos, Color.Black);
        }
    }
}
