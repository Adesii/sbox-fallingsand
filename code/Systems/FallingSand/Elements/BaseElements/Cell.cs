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

	//In Kelvin
	public float Heat { get; set; } = 293;
	public virtual float HeatTransferRate => 0.1f;

	TimeSince LastHeatTransfer = 0f;

	public virtual int HighTemperatureTransitionPoint => 373;
	public virtual int LowTemperatureTransitionPoint => 273;

	public virtual Color GetColor()
	{
		return CellColor;
	}

	public void LogicUpdate( Sandworker worker, out bool sleep )
	{
		PreStep( worker );
		Step( worker );
		bool heat = false;
		//if ( LastHeatTransfer > 0.1f )
		{
			HeatStep( worker, out heat );

			LastHeatTransfer = 0f;
		}
		PostStep( worker, out sleep );

		/* if ( heat && sleep )
		{
			sleep = false;
		} */

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
	public virtual void HeatStep( Sandworker worker, out bool heattransfered )
	{

		heattransfered = true;
		PropagateHeat( worker, 2, Heat );
		Heat -= HeatTransferRate;
		Heat = Math.Min( Heat, 0 );
		//diseperse heat
		//worker.KeepAlive( Position );
	}


	public virtual void OnHit( Sandworker worker, Cell hitCell )
	{
		return;
	}

	public virtual void HeatElement( Sandworker worker, float heat )
	{
		Heat += heat;
	}

	public virtual void OnHeated( Sandworker worker )
	{
	}
	public void PropagateHeat( Sandworker worker, int maxDistance, float Heat )
	{
		float heatToDistrbute = Heat;
		for ( int x = -maxDistance; x < maxDistance; x++ )
		{
			for ( int y = -maxDistance; y < maxDistance; y++ )
			{
				if ( x == 0 && y == 0 )
				{
					continue;
				}
				Vector2Int position = Position + new Vector2Int( x, y );
				SandUtils.PointToPointFunction( Position, position, ( pos ) =>
				{
					if ( worker.GetCell( pos ) is Cell cell )
					{
						float DistanceFactor = 1f - (pos - Position).Length / maxDistance;
						float HeatToDistrubute = (heatToDistrbute * DistanceFactor);
						float HeatDifference = Heat - cell.Heat;
						if ( HeatDifference < 0 )
						{
							HeatToDistrubute = Math.Min( HeatToDistrubute, -HeatDifference );
						}
						cell.HeatElement( worker, HeatDifference );
						cell.OnHeated( worker );
					}
				} );
			}
		}

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
