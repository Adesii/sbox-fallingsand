namespace Sand.Systems.FallingSand;

public class Sandworker
{
	protected WeakReference<SandWorld> wworld;
	protected WeakReference<SandChunk> wchunk;


	protected Sandworker( SandWorld world, SandChunk chunk )
	{
		this.wworld = new( world );
		this.wchunk = new( chunk );
	}


	public void UpdateChunk()
	{
		if ( wchunk.TryGetTarget( out var nchunk ) )
			for ( int x = 0; x < nchunk.Size.x; x++ )
			{
				for ( int y = 0; y < nchunk.Size.y; y++ )
				{
					UpdateCell( new Vector2Int( x, y ) + nchunk.Position );
				}
			}
	}


	public virtual void UpdateCell( Vector2Int Position )
	{

	}

	protected Cell GetCell( Vector2Int pos )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return default;
		if ( chunk.InBounds( pos ) )
			return chunk.GetCell( pos );
		else
			return world.GetCell( pos );
	}

	protected void SetCellVelocity( Vector2Int pos, Vector2Int vel )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return;
		if ( chunk.InBounds( pos ) )
			chunk.SetCellVelocity( pos, vel );
		else
			world.SetCellVelocity( pos, vel );
	}

	protected void SetCell( Vector2Int pos, Cell cell )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return;
		if ( chunk.InBounds( pos ) )
			chunk.SetCell( pos, cell );
		else
			world.SetCell( pos, cell );
	}

	protected void MoveCell( Vector2Int From, Vector2Int To, bool Swap = false )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return;
		if ( chunk.InBounds( From ) )
			chunk.MoveCell( chunk, From, To, Swap );
		else
			world.MoveCell( From, To, Swap );
	}

	protected bool InBounds( Vector2Int pos )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return false;
		return chunk.InBounds( pos ) || world.InBounds( pos );
	}

	protected bool IsEmpty( Vector2 pos )
	{
		return IsEmpty( new Vector2Int( pos.x.FloorToInt(), pos.y.FloorToInt() ) );
	}

	protected bool IsEmpty( Vector2Int pos )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return false;
		if ( chunk.InBounds( pos ) )
			return chunk.IsEmpty( pos );
		else
			return world.IsEmpty( pos );
	}


}

