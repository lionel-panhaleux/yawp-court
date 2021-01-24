using Microsoft.Xna.Framework;

namespace Golconda.Models
{
    /// <summary>
    /// An item and its position on the board.
    /// </summary>
    public class BoardItem
    {
        /// <summary>
        /// The item on the board.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// The board position of the item.
        /// </summary>
        public Vector2 _position;

        public BoardItem(Item item, Vector2 position)
        {
            Item = item;
            _position = position;
        }
    }
}
