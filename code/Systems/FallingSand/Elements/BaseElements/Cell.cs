using Sand.util;

namespace Sand.Systems.FallingSand;

public partial class Cell
{
	public Vector2Int Position;
	public static Cell Empty = new EmptyCell();
	public Color32 color;
	public Vector2 Velocity;
	public int Density;


	public virtual void Step( Sandworker worker, out bool sleep )
	{
		sleep = false;
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
