using System.Collections.Concurrent;

namespace Sand.Systems.FallingSand;

public class SandChunk
{
	public Vector2Int Size, Position;

	private int working_minX, working_maxX, working_minY, working_maxY;
	public int rect_minX, rect_maxX, rect_minY, rect_maxY;

	public void KeepAlive( Vector2Int pos )
	{
		KeepAlivelocal( pos );
	}

	void KeepAlivelocal( Vector2Int global )
	{
		Vector2Int localPosition = global - Position;
		int x = localPosition.x;
		int y = localPosition.y;

		lock ( this )
		{
			working_minX = Math.Min( working_minX, x - 2 ).Clamp( 0, Size.x );
			working_minY = Math.Min( working_minY, y - 2 ).Clamp( 0, Size.y );

			working_maxX = Math.Max( working_maxX, x + 2 ).Clamp( 0, Size.x );
			working_maxY = Math.Max( working_maxY, y + 2 ).Clamp( 0, Size.y );
			//sleeping = false;
			ShouldWakeup = true;
			//Log.Info( $"KeepAlive: {x},{y} {working_minX} {working_maxX} {working_minY} {working_maxY} Size: {Size}" );
		}
		//if ( working_minX != rect_minX || working_maxX != rect_maxX || working_minY != rect_minY || working_maxY != rect_maxY )
		//	ShouldWakeup = true;
	}

	public void UpdateRect()
	{
		//Update Current, reset working
		rect_minX = working_minX;
		rect_maxX = working_maxX;

		rect_minY = working_minY;
		rect_maxY = working_maxY;

		working_minX = Size.x;
		working_maxX = -1;

		working_minY = Size.y;
		working_maxY = -1;
		ShouldWakeup = true;
	}


	public ConcurrentDictionary<int, Cell> cells;
	public List<CellMoveInfo> Changes = new();

	public Texture Texture { get; set; }

	public SandChunk( Vector2Int size, Vector2Int position )
	{
		Size = size;
		Position = position;
		cells = new();
	}

	public int GetIndex( Vector2Int pos )
	{
		return (pos.x - Position.x) + (pos.y - Position.y) * Size.x;
	}
	public bool InBounds( Vector2Int pos )
	{
		return pos.x >= Position.x && pos.y >= Position.y
			&& pos.x < Position.x + Size.x && pos.y < Position.y + Size.y;
	}

	public bool IsEmpty( Vector2Int pos )
	{
		//if ( !InBounds( pos ) )
		//	return false;
		return IsEmpty( GetIndex( pos ) );
	}
	bool IsEmpty( int index )
	{
		var c = GetCell( index );
		return c is EmptyCell || c == null;
	}

	public Cell GetCell( Vector2Int pos )
	{
		if ( !InBounds( pos ) )
		{
			return Cell.Empty;
		}
		return GetCell( GetIndex( pos ) );
	}
	Cell GetCell( int index )
	{
		if ( cells.TryGetValue( index, out var cell ) )
		{
			return (cell is EmptyCell || cell == null) ? Cell.Empty : cell;
		}
		return Cell.Empty;
	}


	public void SetCell( Vector2Int pos, Cell cell, bool wake = false )
	{
		if ( !InBounds( pos ) )
			return;
		int index = GetIndex( pos );
		SetCell( index, pos, cell, wake );
		//KeepAlive( pos );
	}

	void SetCell( int index, Vector2Int Position, Cell cell, bool wake = false )
	{
		if ( cell is EmptyCell || cell == null )
		{
			cells.TryRemove( index, out var _ );
		}
		else
		{
			cell.Position = Position;
			cells[index] = cell;
		}
		//DrawPixel( index, cell == null ? Color.Transparent : cell.CellColor );
	}

	public void MoveCell( SandChunk src, Vector2Int From, Vector2Int To, bool Swap = false )
	{
		lock ( this )
		{
			Changes.Add( new( src, From, To, Swap ) );
		}
	}




	public void DrawPixel( int x, Color color )
	{
		//if ( x < 0 || x >= Size.x * Size.y ) return;
		pixels ??= new Color32[Size.x * Size.y];
		pixels[x] = color;
	}
	Color32[] pixels;

	private bool _sleep;

	public bool IsCurrentlySleeping => _sleep;
	public bool sleeping
	{
		set
		{
			if ( value && !_sleep )
			{
				SleepTime = 0;
			}
			else
			{
				ShouldWakeup = true;
			}
			_sleep = value;
		}
		get
		{
			return _sleep;
		}
	}
	public bool ShouldWakeup = false;

	public TimeSince SleepTime = 0;

	public void Draw()
	{
		pixels ??= new Color32[Size.x * Size.y];
		Texture ??= Texture.Create( Size.x, Size.y ).WithDynamicUsage().Finish();

		for ( int i = 0; i < Size.x * Size.y; i++ )
		{
			pixels[i] = GetCell( i )?.GetColor() ?? Color.Transparent;
		}
		Texture.Update( pixels, 0, 0, Size.x, Size.y );
	}
}

