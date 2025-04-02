using System;

public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return "x: " + x + " y: " + y;
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.x == b.x && a.y == b.y;
    }
    
    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.y + b.y);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x - b.x, a.y - b.y);
    }

    public override bool Equals(object obj)
    {
        if (obj is GridPosition)
        {
            GridPosition gridPosition = (GridPosition)obj;
            return x == gridPosition.x && y == gridPosition.y;
        }
        return false;
    }

    public bool Equals(GridPosition other)
    {
        return x == other.x && y == other.y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }
}
