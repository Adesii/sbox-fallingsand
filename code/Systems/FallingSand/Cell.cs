namespace Sand.Systems.FallingSand;

public struct Cell
{
	public int type;
	public Color color;
	public Vector2Int Velocity;

	public int Density;
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
