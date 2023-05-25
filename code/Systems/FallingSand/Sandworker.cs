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
		{
			bool sleep = true;
			bool shouldsleep = true;

			for ( int y = nchunk.rect_minY; y < nchunk.rect_maxY; y++ )
			{
				for ( int x = nchunk.rect_minX; x < nchunk.rect_maxX; x++ )
				{
					UpdateCell( new Vector2Int( x, y ) + nchunk.Position, out shouldsleep );
					if ( !shouldsleep )
					{
						sleep = false;
					}
				}
			}
			nchunk.sleeping = sleep;
		}

	}


	public virtual void UpdateCell( Vector2Int Position, out bool sleep )
	{
		sleep = true;
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

	protected void SetCell( Vector2Int pos, ref Cell cell, bool wake = false )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return;

		if ( chunk.InBounds( pos ) )
			chunk.SetCell( pos, ref cell, wake );
		else
			world.SetCell( pos, ref cell, wake );

	}


	protected void MoveCell( Vector2Int From, Vector2Int To, bool Swap = false )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return;

		int pingx = 0, pingy = 0;

		if ( From.x == chunk.Position.x ) pingx = -1;
		if ( From.x == chunk.Position.x + chunk.Size.x - 1 ) pingx = 1;
		if ( From.y == chunk.Position.y ) pingy = -1;
		if ( From.y == chunk.Position.y + chunk.Size.y - 1 ) pingy = 1;

		if ( pingx != 0 ) world.KeepAlive( new Vector2Int( From.x + pingx, From.y ) );
		if ( pingy != 0 ) world.KeepAlive( new Vector2Int( From.x, From.y + pingy ) );

		if ( pingx != 0 && pingy != 0 ) world.KeepAlive( new Vector2Int( From.x + pingx, From.y + pingy ) );


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

	protected bool IsEmpty( Vector2Int pos )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return false;
		if ( chunk.InBounds( pos ) )
			return chunk.IsEmpty( pos );
		else
			return world.IsEmpty( pos );
	}


}

