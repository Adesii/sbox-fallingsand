using Sand.util;

namespace Sand.Systems.FallingSand;

public partial class Cell
{
	public Vector2Int Position;
	public static Cell Empty = new EmptyCell();
	protected Color CellColor;
	public Vector2 Velocity;
	public virtual int Density { get; set; } = 1;

	public virtual float MaxVelocity { get; private set; } = 6f;

	protected virtual float HorizontalConversion { get; set; } = 20f;
	protected virtual bool ShouldBounceToSide { get; set; } = true;

	protected virtual float Friction => 0.5f;

	public virtual Color GetColor()
	{
		return CellColor;
	}

	public void LogicUpdate( Sandworker worker, out bool sleep )
	{
		PreStep( worker );
		Step( worker );
		PostStep( worker, out sleep );
	}


	public virtual void PreStep( Sandworker worker )
	{
	}
	public virtual void Step( Sandworker worker )
	{

	}
	public virtual void PostStep( Sandworker worker, out bool sleep )
	{
		sleep = false;
	}



	public virtual void OnHit( Sandworker worker, Cell hitCell )
	{
		return;
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
