using System.Drawing;
using Squirrel.Data;

namespace Squirrel.Client.Objects
{
    public class Player : Entity
    {
        public Player(Bitmap asset)
            : base(asset)
        { 
        }

        public Player(Bitmap asset, Orientation orientation)
            : base(asset, orientation)
        {
        }

        public Player(Bitmap asset, float x, float y, float rotation)
            : base(asset, x, y, rotation)
        {
        }
    }
}
