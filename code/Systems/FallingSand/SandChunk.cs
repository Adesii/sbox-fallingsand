namespace Sand.Systems.FallingSand;

public class SandChunk
{
	public Vector2Int Size, Position;


	Cell[] cells;
	public List<CellMoveInfo> Changes = new();

	public int filledcells = 0;

	public Texture Texture { get; set; }

	public SandChunk( Vector2Int size, Vector2Int position )
	{
		Size = size;
		Position = position;
		cells = new Cell[size.x * size.y];
	}

	public int GetIndex( Vector2Int pos )
	{
		return (pos.x - Position.x) +
			(pos.y - Position.y) * Size.x;
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
		return GetCell( index ).type == 0;
	}

	public Cell GetCell( Vector2Int pos )
	{
		if ( !InBounds( pos ) )
		{
			return new Cell();
		}
		return GetCell( GetIndex( pos ) );
	}
	Cell GetCell( int index )
	{
		return cells[index];
	}


	public void SetCell( Vector2Int pos, Cell cell, bool wake = false )
	{
		if ( !InBounds( pos ) )
			return;
		SetCell( GetIndex( pos ), cell, wake );
	}
	void SetCell( int index, Cell cell, bool wake = false )
	{
		if ( cell.type == 0 && cells[index].type != 0 )
		{
			System.Threading.Interlocked.Decrement( ref filledcells );
		}
		else if ( cell.type != 0 && cells[index].type == 0 )
		{
			System.Threading.Interlocked.Increment( ref filledcells );
		}
		//sleeping = false;
		ShouldWakeup = wake;

		cells[index] = cell;
		DrawPixel( index, cell.type == 0 ? Color.Transparent : cell.color );
	}

	public void MoveCell( SandChunk src, Vector2Int From, Vector2Int To, bool Swap = false )
	{

		Changes.Add( new( src, From, To, Swap ) );
	}




	public void DrawPixel( int x, Color color )
	{
		if ( x < 0 || x >= Size.x * Size.y ) return;
		pixels ??= new Color32[Size.x * Size.y];
		pixels[x] = color;
	}
	Color32[] pixels;

	private bool _sleep;
	public bool sleeping
	{
		set
		{
			if ( value )
			{
				SleepTime = 0;
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

		Texture.Update( pixels, 0, 0, Size.x, Size.y );
	}

	internal void SetVelocityCell( Vector2Int pos, Vector2Int vel )
	{
		var cell = GetCell( pos );
		cell.Velocity = vel;
		SetCell( pos, cell );
	}

	internal void SetCellVelocity( Vector2Int pos, Vector2Int vel )
	{
		var cell = GetCell( pos );
		cell.Velocity = vel;
		SetCell( pos, cell );
	}
}

