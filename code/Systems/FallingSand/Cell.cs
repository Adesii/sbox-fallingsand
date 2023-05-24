namespace Sand.Systems.FallingSand;

public class Cell
{
	public static Cell Empty = new EmptyCell();
	public Color color;
	public Vector2Int Velocity;
	public int Density;
}

public class EmptyCell : Cell
{
	public EmptyCell()
	{
		color = Color.Transparent;
		Velocity = Vector2Int.Zero;
		Density = 0;
	}
}

public class SandElement : Cell
{
	public SandElement()
	{
		//brown
		color = Color.FromBytes( 139, 69, 19, 255 ) * Game.Random.Float( 0.8f, 1.2f );
		Velocity = Vector2Int.Zero;
		Density = 1;
	}
}

public class WaterElement : Cell
{
	public WaterElement()
	{
		color = Color.FromBytes( 0, 0, 255, 125 ) * Game.Random.Float( 0.9f, 1.1f );
		Velocity = Vector2Int.Zero;
		Density = 2;
	}
}

public struct CellMoveInfo
{
	public Vector2Int From;
	public Vector2Int To;

	public SandChunk Source;

	public bool Swap;

	public CellMoveInfo( SandChunk source, Vector2Int from, Vector2Int to, bool swap = false )
	{
		From = from;
		To = to;
		Swap = swap;
		Source = source;
	}
}
