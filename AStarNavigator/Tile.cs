namespace AStarNavigator
{
    public struct Tile
    {
	    public readonly int X;
	    public readonly int Y;

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }

	    public bool Equals(Tile other)
	    {
		    return X == other.X && Y == other.Y;
	    }

	    public override bool Equals(object obj)
	    {
		    if (ReferenceEquals(null, obj)) return false;
		    return obj is Tile && Equals((Tile) obj);
	    }

	    public override int GetHashCode()
	    {
		    unchecked
		    {
			    return (X * 397) ^ Y;
		    }
	    }
    }
}
